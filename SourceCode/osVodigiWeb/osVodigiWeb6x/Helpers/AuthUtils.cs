using System;
using osVodigiWeb6x.Models;
using System.Web;

namespace osVodigiWeb6x
{
    public class AuthUtils
    {
        
        public static User CheckIfAdmin()
        {
            User user = CheckAuthUser();

            if (user == null || !user.IsAdmin)
            {
                throw new Exception("You are not authorized (Admin role) to access this page.");
            }
            return user;
        }

        public static User CheckAuthUser()
        {
            HttpContext context = HttpContext.Current;
            User user = (User)context.Session["User"];

            if (user == null)
            {
                throw new Exception("You must be authenticate to access this page.");
            }

            return user;
        }

        public static string GetLoginInfo() {

            HttpContext context = HttpContext.Current;
            string loginInfo = (string)context.Session["LoginInfo"];
            return loginInfo;
        }

        public static int GetAccountId()
        {
            HttpContext context = HttpContext.Current;
            int accountid = (context.Session["UserAccountID"] != null) ? (Int32)context.Session["UserAccountID"] : 0;
            return accountid;
        }

        public static string GetAccountName()
        {
            HttpContext context = HttpContext.Current;
            string userAccountName = (string)context.Session["UserAccountName"];
            return userAccountName;
        }
    }
}
