using Projekt_v2.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projekt_v2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var cookie = HttpContext.Request.Cookies["AnonymousUserName"];
            if (User.Identity.IsAuthenticated)
            {
                var model = new HomeIndexModel();
                model.UserName = User.Identity.Name;
                return View(model);
            }
            if(cookie != null)
            {
                var model = new HomeIndexModel();
                model.UserName = cookie.Value;
                return View(model);
            }

            return RedirectToAction("LogOn", "Account", new { ReturnUrl = "/Home/Index"});
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}