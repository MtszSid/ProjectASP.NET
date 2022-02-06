using Projekt_v2.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projekt_v2.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        [HttpGet]
        public ActionResult LogOn()
        {
            return View(new AccountLogOnMdel());
        }

        [HttpPost]
        public ActionResult LogOn(AccountLogOnMdel model)
        {  
            return View(model);
        }

        public ActionResult LogOnAnonymous(AccountLogOnMdel model)
        {
            if (!string.IsNullOrEmpty(model.UserName))
            {
                var returnUrl = HttpContext.Request.QueryString["ReturnUrl"];
                var myCookie = new HttpCookie("AnonymousUserName", model.UserName);
                this.HttpContext.Response.Cookies.Add(myCookie);
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return Redirect("/");
            }
            else
            {
                return RedirectToAction("LogOn");
            }
        }
    }
}