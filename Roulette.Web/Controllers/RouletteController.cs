using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roulette.Web.Controllers
{
    public class RouletteController : Controller
    {
        public IActionResult Game()
        {
            return View();
        }
    }
}
