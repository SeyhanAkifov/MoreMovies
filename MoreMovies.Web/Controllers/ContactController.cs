using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
