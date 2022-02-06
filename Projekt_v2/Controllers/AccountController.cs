
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Projekt_v2.DB;
using Projekt_v2.Models.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
            if (Membership.ValidateUser(model.UserName, model.Password))
            {
                FormsAuthentication.RedirectFromLoginPage(model.UserName, false);
                return new EmptyResult();
            }
            model.Message = "Nieprawidłowe dane logowania";
            return View(model);
        }
        [HttpPost]
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
        [HttpPost]
        public ActionResult Register(AccountLogOnMdel model)
        {
            if (String.IsNullOrEmpty(model.UserName) ||
                String.IsNullOrEmpty(model.Password) ||
                String.IsNullOrEmpty(model.PasswordRetyped) ||
                String.IsNullOrEmpty(model.Email))
            {
                model.Message = "Invalid input.";
                return RedirectToAction("LogOn", "Account", model);
            }

            if (model.Password != model.PasswordRetyped)
            {
                model.Message = "Passwords must match.";
                return RedirectToAction("LogOn", "Account", model);
            }

            var dataContext = CustomUserDataContext.GetDataContext();

            var usr_name = from user in dataContext.USERs
                           where user.UserName == model.UserName
                           select user;

            if (usr_name.FirstOrDefault() != null)
            {
                model.Message = "UserName already taken.";
                return RedirectToAction("LogOn", "Account", model);
            }

            var usr_email = from user in dataContext.USERs
                            where user.Email == model.Email
                            select user;

            if (usr_email.FirstOrDefault() != null)
            {
                model.Message = "Email already taken!";
                return RedirectToAction("LogOn", "Account", model);
            }

            var usr = new USER();
            usr.UserName = model.UserName;
            usr.Email = model.Email;
            usr.PASSWORD = createPassword(model.Password);

            dataContext.USERs.InsertOnSubmit(usr);

            dataContext.SubmitChanges();

            FormsAuthentication.RedirectFromLoginPage(model.UserName, false);
            return new EmptyResult();
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();
            return Redirect("/");
        }

        private PASSWORD createPassword(String password)
        {
            // generate a 128-bit salt using a cryptographically strong random sequence of nonzero values
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }


            // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            var pass = new PASSWORD();
            pass.Hash = hashed;
            pass.Salt = Convert.ToBase64String(salt);
            pass.NumberOfHashes = 100000;
            return pass;
        }
    }
}