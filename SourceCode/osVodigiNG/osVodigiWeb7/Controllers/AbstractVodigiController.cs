using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using osVodigiWeb6x;
using osVodigiWeb7.Extensions;
using osVodigiWeb6x.Models;
using Microsoft.AspNetCore.Hosting;

namespace osVodigiWeb6x.Controllers
{
    public abstract class AbstractVodigiController : Controller
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public AbstractVodigiController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        protected User checkIfAdmin()
        {
            User user = CheckAuthUser();

            if (user == null || !user.IsAdmin)
            {
                throw new Exception("You are not authorized (Admin role) to access this page.");
            }
            return user;
        }

        protected User CheckAuthUser()
        {
            var userAccountId = HttpContext.Session.GetInt32("UserAccountID");
            var userAccountName = HttpContext.Session.GetString("UserAccountName");
            User user = HttpContext.Session.Get<User>("User");

            if (user != null)
            {
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, userAccountName);

                if (user.IsAdmin)
                {
                    ViewData["txtIsAdmin"] = "true";
                }
                else
                {
                    ViewData["txtIsAdmin"] = "false";
                }
            }
            else
            {
                throw new Exception("You must be authenticate to access this page.");
            }

            return user;
        }

        protected int GetAuthAccountId(){
            int accountid = HttpContext.Session.GetInt32("UserAccountID") ?? 0;
            return accountid;
        }

        protected string GetWebRootPath() 
        {
            return _hostingEnvironment.WebRootPath;
        }

        protected string GetContentRootPath()
        {
            return _hostingEnvironment.ContentRootPath;
        }
    }
}
