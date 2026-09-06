using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoreMovies.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    [Authorize]
    public class UserController : Controller
    {

        private readonly IUserService userService;


        public UserController(IUserService userService)
        {
            this.userService = userService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Become()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Become")]
        public async Task<IActionResult> BecomeConfirmed()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            await userService.Become(userId);

            return RedirectToAction("Index", "Home");
        }
    }
}
