using Microsoft.AspNetCore.Mvc;
using MoreMovies.Services.Interfaces;

namespace MoreMovies.Web.Controllers
{
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
        public IActionResult Become(string userId)
        {
            userService.Become(userId);

            return RedirectToAction("Index", "Home");
        }
    }
}
