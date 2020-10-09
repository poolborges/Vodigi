using System;
using osVodigiWeb7x.Models;
using System.Web;
using osVodigiWeb7x.Exceptions;
using Microsoft.AspNetCore.Http;

namespace osVodigiWeb7x
{
    public class AuthUtils
    {

        public static User CheckIfAdmin()
        {
            User user = CheckAuthUser();

            if (user == null || !user.IsAdmin)
            {
                throw new NotAuthzException("Current user does not has 'Admin' Authorization");
            }
            return user;
        }

        public static User CheckAuthUser()
        {
            User user = new User { UserID = 1,
                EmailAddress = "user@example.com",
                IsAdmin= true, IsActive = true,
                FirstName = "John",
                LastName = "Due",
                Username = "admin",
                Password = "admn",
                AccountID = 1
            };

            if (user == null)
            {
                throw new NotAuthcException("User is not authenticated");
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
