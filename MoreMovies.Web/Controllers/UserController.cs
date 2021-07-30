using Microsoft.AspNetCore.Mvc;

namespace MoreMovies.Web.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
