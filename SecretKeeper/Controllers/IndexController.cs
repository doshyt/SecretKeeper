using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SecretKeeper.Pages;

namespace SecretKeeper.Controllers
{
    public class IndexController : Controller
    {
        public IActionResult Index(ShareModel model)
        {
           return View(model);
        }
    }
}