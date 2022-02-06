
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekt_v2.DB
{
    public class CustomUserDataContext
    {
        private const string str = "USER_DATA_CONTEXT";
        public static GameUsersDataContext GetDataContext()
        {
            if (HttpContext.Current.Items[str] == null)
            {
                HttpContext.Current.Items[str] = new GameUsersDataContext();
            }
            return (GameUsersDataContext)HttpContext.Current.Items[str];
        }
    }
}