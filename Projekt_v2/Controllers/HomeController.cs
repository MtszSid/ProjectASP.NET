using Projekt_v2.DB;
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
                model.Authenticated = "auth";
                return View(model);
            }
            if(cookie != null)
            {
                var model = new HomeIndexModel();
                model.UserName = cookie.Value;
                model.Authenticated = "not";
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
        [Authorize]
        public ActionResult Stats(int page = 1,
            string sort = "GivenName", string sortdir = "ASC")
        {
            var model = new HomeStatsModel();
            var dataLayer = new DataLayer();

            model.Info = new PagedEnumerable<Info>()
            {
                Items = dataLayer.GetData(string.Format("{0} {1}", sort, sortdir), (page - 1) * 10, 10),
                TotalCount = dataLayer.TotalUsers()
            };

            var dataContext = CustomUserDataContext.GetDataContext();
            var usr = (from us in dataContext.USERs
                       where us.UserName == User.Identity.Name
                       select us.ID).FirstOrDefault();

            model.Won = (from st in dataContext.STATs
                         where (st.CrossesID == usr || st.NoughtsID == usr) && st.Result == usr
                         select st).Count();
            model.Lost = (from st in dataContext.STATs
                         where (st.CrossesID == usr || st.NoughtsID == usr) && st.Result != usr && st.Result != 0
                         select st).Count();
            model.Undecided = (from st in dataContext.STATs
                         where (st.CrossesID == usr || st.NoughtsID == usr) && st.Result == 0
                         select st).Count();
            return View(model);
        }
    }
}