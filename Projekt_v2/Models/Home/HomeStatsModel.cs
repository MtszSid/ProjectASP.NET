using Projekt_v2.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekt_v2.Models.Home
{
    public class Info
    {
        public string Crosses { get; set; }
        public string Noughts { get; set; }
        public string Result { get; set; }
    
        public Info(string crosses, string noughts, string result)
        {
            Crosses = crosses;
            Noughts = noughts;
            Result = result;
        }
    }



    public class HomeStatsModel
    {
        public int Won;
        public int Lost;
        public int Undecided;
        public PagedEnumerable<Info> Info { get; set; }
    }

    public class PagedEnumerable<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public IEnumerable<T> Items { get; set; }
    }

    public class DataLayer
    {
        public IEnumerable<Info> GetData(string OrderBy, int StartRow, int RowCount)
        {
            var dataContext = CustomUserDataContext.GetDataContext();
            var orderSplit = OrderBy.Split(' ');
            var key = (orderSplit.Length >= 2 ? orderSplit[0] : "Crosses");
            var order = (orderSplit.Length >= 2 ? orderSplit[1] : "ASC");

            var usr = (from us in dataContext.USERs
                       where us.UserName == HttpContext.Current.User.Identity.Name
                       select us.ID).FirstOrDefault();

            if (order == "ASC")
            {
                switch (key)
                {
                    case "Crosses":
                        return (from st in dataContext.STATs
                                where st.CrossesID == usr || st.NoughtsID == usr
                                orderby st.USER.UserName
                                select new Info(st.USER.UserName,
                                                st.USER1.UserName,
                                                (st.Result == usr ? "Won" :
                                                (st.Result == 0 ? "Undecided" : "Lost")))).Skip(StartRow).Take(RowCount);
                    case "Noughts":
                        return (from st in dataContext.STATs
                                where st.CrossesID == usr || st.NoughtsID == usr
                                orderby st.USER1.UserName
                                select new Info(st.USER.UserName,
                                                st.USER1.UserName,
                                                (st.Result == usr ? "Won" :
                                                (st.Result == 0 ? "Undecided" : "Lost")))).Skip(StartRow).Take(RowCount);
                    case "Result":
                        return (from st in dataContext.STATs
                                where st.CrossesID == usr || st.NoughtsID == usr
                                orderby st.Result
                                select new Info(st.USER.UserName,
                                                st.USER1.UserName,
                                                (st.Result == usr ? "Won" :
                                                (st.Result == 0 ? "Undecided" : "Lost")))).Skip(StartRow).Take(RowCount);
                }
            }
            else
            {
                switch (key)
                {
                    case "Crosses":
                        return (from st in dataContext.STATs
                                where st.CrossesID == usr || st.NoughtsID == usr
                                orderby st.USER.UserName descending
                                select new Info(st.USER.UserName,
                                                st.USER1.UserName,
                                                (st.Result == usr ? "Won" :
                                                (st.Result == 0 ? "Undecided" : "Lost")))).Skip(StartRow).Take(RowCount);
                    case "Noughts":
                        return (from st in dataContext.STATs
                                where st.CrossesID == usr || st.NoughtsID == usr
                                orderby st.USER1.UserName descending
                                select new Info(st.USER.UserName,
                                                st.USER1.UserName,
                                                (st.Result == usr ? "Won" :
                                                (st.Result == 0 ? "Undecided" : "Lost")))).Skip(StartRow).Take(RowCount);
                    case "Result":
                        return (from st in dataContext.STATs
                                where st.CrossesID == usr || st.NoughtsID == usr
                                orderby st.Result descending
                                select new Info(st.USER.UserName,
                                                st.USER1.UserName,
                                                (st.Result == usr ? "Won" :
                                                (st.Result == 0 ? "Undecided" : "Lost")))).Skip(StartRow).Take(RowCount);
                }
            }

            return (from st in dataContext.STATs
                                            where st.CrossesID == usr || st.NoughtsID == usr
                                            orderby st.USER.UserName 
                                            select new Info(st.USER.UserName, 
                                                            st.USER1.UserName,
                                                            (st.Result == usr ? "Won" :
                                                            (st.Result == 0 ? "Undecided" : "Lost")))).Skip(StartRow).Take(RowCount);
        }

        public int TotalUsers()
        {
            var dataContext = CustomUserDataContext.GetDataContext();

            
            var usr = (from us in dataContext.USERs
                       where us.UserName == HttpContext.Current.User.Identity.Name
                       select us.ID).FirstOrDefault();

            var t = (from st in dataContext.STATs 
                     where st.CrossesID == usr || st.NoughtsID == usr
                     select st).Count();
            return t;
        }
    }
}