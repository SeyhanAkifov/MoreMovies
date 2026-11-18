using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MoreMovies.Data;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;

namespace MoreMovies.Web.Tests.Integration;

public class RegistrationTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private const string Registration = "/Identity/Account/Register";
    private const string Login = "/Identity/Account/Login";
    private const string Confirmation = "/Identity/Account/RegisterConfirmation";
    private const string ProtectedPage = "/Movie/MyMovies";
    private const string Email = "registration-test@example.invalid";
    private const string Password = "Registration-Test!49";

    /// <summary>Verify registration needs confirmation, with either local or delivered links.</summary>
    [Theory]
    [InlineData("Development")]
    [InlineData("Production")]
    public async Task NewAccountCanSignInOnlyAfterConfirmation(string environment)
    {
        var sender = environment == "Production" ? new CapturingEmailSender() : null;
        using var app = CreateApplication(environment, sender);
        using var client = CreateClient(app);
        using var otherBrowser = CreateClient(app);

        using var previousRoute = await client.GetAsync($"/Home/Register?returnUrl={ProtectedPage}");
        var registrationUrl = previousRoute.Headers.Location;
        Assert.Equal(HttpStatusCode.Redirect, previousRoute.StatusCode);
        Assert.StartsWith(Registration + "?", registrationUrl.OriginalString);

        var registerPage = await client.GetStringAsync(registrationUrl);
        var fields = GetCredentials(registerPage);
        fields["Input.ConfirmPassword"] = Password;
        fields["ReturnUrl"] = ProtectedPage;

        using var legacyPost = await client.PostAsync("/Home/Register", new FormUrlEncodedContent(fields));
        Assert.Equal(HttpStatusCode.MethodNotAllowed, legacyPost.StatusCode);

        using var posted = await client.PostAsync(registrationUrl, new FormUrlEncodedContent(fields));
        Assert.Equal(HttpStatusCode.Redirect, posted.StatusCode);
        Assert.StartsWith(Confirmation + "?", posted.Headers.Location.OriginalString);
        Assert.DoesNotContain(Email, posted.Headers.Location.OriginalString);

        using var unauthenticated = await client.GetAsync(ProtectedPage);
        Assert.Equal(HttpStatusCode.Redirect, unauthenticated.StatusCode);
        Assert.Equal(Login, unauthenticated.Headers.Location.AbsolutePath);

        var loginUrl = unauthenticated.Headers.Location;
        var signIn = GetCredentials(await client.GetStringAsync(loginUrl));
        signIn["ReturnUrl"] = ProtectedPage;
        using var refused = await client.PostAsync(loginUrl, new FormUrlEncodedContent(signIn));
        Assert.Equal(HttpStatusCode.OK, refused.StatusCode);
        Assert.Contains("Sign-in failed.", await refused.Content.ReadAsStringAsync());

        var directVisit = await otherBrowser.GetStringAsync($"{Confirmation}?email={Email}");
        Assert.DoesNotContain("/Account/ConfirmEmail?", directVisit);

        using var instructions = await client.GetAsync(posted.Headers.Location);
        Assert.True(instructions.Headers.CacheControl?.NoStore);
        var instructionsHtml = await instructions.Content.ReadAsStringAsync();
        string confirmationUrl;
        if (sender is null)
        {
            confirmationUrl = ExtractLink(instructionsHtml, "Confirm test account");
            Assert.StartsWith("/Identity/Account/ConfirmEmail?", confirmationUrl);
            var refreshed = await client.GetStringAsync(posted.Headers.Location);
            Assert.DoesNotContain("/Account/ConfirmEmail?", refreshed);
        }
        else
        {
            Assert.Contains("Open the confirmation link in your email", instructionsHtml);
            Assert.DoesNotContain("/Account/ConfirmEmail?", instructionsHtml);
            Assert.Equal(Email, sender.Recipient);
            var mailLink = ExtractLink(sender.Html, "opening this link");
            Assert.StartsWith("https://movies.example.invalid/Identity/Account/ConfirmEmail?", mailLink);
            confirmationUrl = new Uri(mailLink).PathAndQuery;
        }

        using var confirmed = await otherBrowser.GetAsync(confirmationUrl);
        Assert.Equal(HttpStatusCode.OK, confirmed.StatusCode);
        Assert.Contains("Thank you for confirming your email.", await confirmed.Content.ReadAsStringAsync());

        using var accepted = await client.PostAsync(loginUrl, new FormUrlEncodedContent(signIn));
        Assert.Equal(HttpStatusCode.Redirect, accepted.StatusCode);
        Assert.Equal(ProtectedPage, accepted.Headers.Location.OriginalString);

        using var authenticated = await client.GetAsync(ProtectedPage);
        Assert.Equal(HttpStatusCode.OK, authenticated.StatusCode);
    }

    /// <summary>Verify production registration waits for a working delivery configuration.</summary>
    [Fact]
    public async Task RegistrationWithoutMailDoesNotCreateAccounts()
    {
        using var app = CreateApplication("Production");
        using var client = CreateClient(app);

        Assert.Contains("Registration is temporarily unavailable", await client.GetStringAsync(Registration));
        var fields = GetCredentials(await client.GetStringAsync(Login));
        fields["Input.ConfirmPassword"] = Password;
        using var posted = await client.PostAsync(Registration, new FormUrlEncodedContent(fields));
        Assert.Equal(HttpStatusCode.ServiceUnavailable, posted.StatusCode);

        using var scope = app.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        Assert.Null(await users.FindByEmailAsync(Email));
    }

    /// <summary>Host the full application with an isolated database and optional mail transport.</summary>
    private WebApplicationFactory<Program> CreateApplication(string environment, CapturingEmailSender sender = null)
    {
        var databaseName = $"MoreMoviesRegistration-{Guid.NewGuid():N}";
        return factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment(environment);
            builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(
                new Dictionary<string, string> { ["Registration:PublicOrigin"] = "https://movies.example.invalid" }));
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(databaseName));
                if (sender is not null)
                {
                    services.RemoveAll<IEmailSender>();
                    services.AddSingleton<IEmailSender>(sender);
                }
            });
        });
    }

    /// <summary>Follow redirects explicitly and keep a separate set of browser cookies.</summary>
    private static HttpClient CreateClient(WebApplicationFactory<Program> app) =>
        app.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://test-host.invalid")
        });

    /// <summary>Build a credential form with the antiforgery token from its rendered page.</summary>
    private static Dictionary<string, string> GetCredentials(string html)
    {
        var token = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"").Groups[1].Value;
        Assert.NotEmpty(token);
        return new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Input.Email"] = Email,
            ["Input.Password"] = Password
        };
    }

    /// <summary>Extract the destination of the named confirmation link.</summary>
    private static string ExtractLink(string html, string label)
    {
        var match = Regex.Match(html, $"href=\"([^\"]+)\">{Regex.Escape(label)}</a>");
        Assert.True(match.Success, $"Expected a link labeled '{label}'.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }

    private sealed class CapturingEmailSender : IEmailSender
    {
        public string Recipient { get; private set; }

        public string Html { get; private set; }

        /// <summary>Capture the message without sending external mail.</summary>
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            Recipient = email;
            Html = htmlMessage;
            return Task.CompletedTask;
        }
    }
}
