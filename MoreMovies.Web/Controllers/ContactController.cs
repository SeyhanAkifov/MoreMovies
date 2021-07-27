using Microsoft.AspNetCore.Mvc;

namespace MoreMovies.Web.Controllers
{
    public class ContactController : Controller
    {
        [HttpGet]
        public IActionResult ContactPage()
        {
            return this.View();
        }
    }
}
