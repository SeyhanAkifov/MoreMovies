using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoreMovies.Web.Controllers
{
    public class NewsController : Controller
    {
        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Delete()
        {
            return View();
        }
    }
}
