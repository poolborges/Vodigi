using System;
using osVodigiWeb6x.Models;
using System.Web;
using osVodigiWeb6x.Exceptions;
using Microsoft.AspNetCore.Http;

namespace osVodigiWeb6x
{
    public class AuthUtils
    {

        public static User CheckIfAdmin()
        {
            User user = CheckAuthUser();

            if (user == null || !user.IsAdmin)
            {
                throw new NotAuthzException("Admin");
            }
            return user;
        }

        public static User CheckAuthUser()
        {
            User user = null;

            if (user == null)
            {
                throw new NotAuthcException();
            }

            return user;
        }

        public static string GetLoginInfo()
        {
            return "ANONIMO";
        }

        public static int GetAccountId()
        {
            return 0;
        }

        public static string GetAccountName()
        {
            return "ANONIMO";
        }
    }
}
