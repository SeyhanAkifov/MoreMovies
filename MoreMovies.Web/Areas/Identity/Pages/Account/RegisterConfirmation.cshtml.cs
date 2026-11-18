using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;

namespace MoreMovies.Web.Areas.Identity.Pages.Account;

/// <summary>Show the confirmation instructions after registration.</summary>
[AllowAnonymous]
[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
public class RegisterConfirmationModel(IWebHostEnvironment environment) : PageModel
{
    public string DevelopmentConfirmationUrl { get; private set; }

    public string ReturnUrl { get; private set; }

    /// <summary>Read the one-time development link from the registering browser.</summary>
    public void OnGet(string returnUrl)
    {
        ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : null;
        if (environment.IsDevelopment())
        {
            DevelopmentConfirmationUrl = TempData[RegisterModel.DevelopmentConfirmationKey] as string;
        }
    }
}
