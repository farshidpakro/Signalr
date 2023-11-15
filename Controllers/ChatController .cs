using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace signalr.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Index()
        {

            ViewBag.Username = User.Identity.Name;
            return View();
        }
    }
}