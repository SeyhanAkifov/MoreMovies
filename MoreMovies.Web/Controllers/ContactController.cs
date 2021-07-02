using Microsoft.AspNetCore.Mvc;

namespace MoreMovies.Web.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult ContactPage()
        {
            return this.View();
        }
    }
}
