using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SecretKeeper.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Share()
        {
            return View(new ShareModel());
        }
    }
}