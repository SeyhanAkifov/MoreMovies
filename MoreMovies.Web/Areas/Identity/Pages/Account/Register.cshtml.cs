using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MoreMovies.Web.Areas.Identity.Pages.Account;

/// <summary>Register an account and issue its email confirmation link.</summary>
[AllowAnonymous]
public class RegisterModel(
    UserManager<IdentityUser> users,
    IEmailSender emailSender,
    IWebHostEnvironment environment,
    IConfiguration configuration) : PageModel
{
    internal const string DevelopmentConfirmationKey = "DevelopmentRegistrationConfirmation";

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    private bool UseLocalConfirmation => environment.IsDevelopment() && emailSender is NoOpEmailSender;

    public bool CanRegister => UseLocalConfirmation ||
        (emailSender is not NoOpEmailSender && PublicOrigin != null);

    private Uri PublicOrigin =>
        Uri.TryCreate(configuration["Registration:PublicOrigin"], UriKind.Absolute, out var origin) &&
        origin.Scheme == Uri.UriSchemeHttps && origin.UserInfo.Length == 0 &&
        origin.PathAndQuery == "/" && origin.Fragment.Length == 0
            ? origin
            : null;

    /// <summary>Keep the destination within the site when opening registration.</summary>
    public void OnGet() => ReturnUrl = Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : null;

    /// <summary>Create an unconfirmed account and deliver its confirmation link.</summary>
    public async Task<IActionResult> OnPostAsync()
    {
        var returnUrl = Url.IsLocalUrl(ReturnUrl) ? ReturnUrl : null;
        if (!CanRegister)
        {
            Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            return Page();
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
        var result = await users.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        var token = await users.GenerateEmailConfirmationTokenAsync(user);
        var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var path = Url.Page("./ConfirmEmail", new { userId = user.Id, code });
        if (UseLocalConfirmation)
        {
            TempData[DevelopmentConfirmationKey] = path;
        }
        else
        {
            var link = HtmlEncoder.Default.Encode(new Uri(PublicOrigin, path).AbsoluteUri);
            await emailSender.SendEmailAsync(user.Email, "Confirm your MoreMovies email",
                $"Confirm your email address by <a href=\"{link}\">opening this link</a>.");
        }

        return RedirectToPage("./RegisterConfirmation", new { returnUrl });
    }

    /// <summary>Collect the email and password for a new account.</summary>
    public class RegisterInput
    {
        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
