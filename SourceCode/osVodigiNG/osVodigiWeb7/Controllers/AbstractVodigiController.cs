using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using osVodigiWeb6x;
using osVodigiWeb7.Extensions;
using osVodigiWeb6x.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace osVodigiWeb6x.Controllers
{
    public abstract class AbstractVodigiController : Controller
    {
        protected readonly IWebHostEnvironment _hostingEnvironment;
        protected readonly IConfiguration _configuration;

        public AbstractVodigiController(IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _hostingEnvironment = webHostEnvironment;
            _configuration = configuration;
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
            //Where www folder is, public by defalt
            return _hostingEnvironment.WebRootPath;
        }

        protected string GetContentRootPath()
        {
            //Where binary and private file are
            return _hostingEnvironment.ContentRootPath;
        }

        protected string GetUploadFolder() {

            return System.IO.Path.Combine(GetWebRootPath(), "UploadedFiles");
        }

        protected string GetHostFolder(String folder)
        {

            return System.IO.Path.Combine(GetWebRootPath(), folder);
        }

    }
}
