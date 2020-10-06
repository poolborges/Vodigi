/* ----------------------------------------------------------------------------------------
    Vodigi - Open Source Interactive Digital Signage
    Copyright (C) 2005-2013  JMC Publications, LLC

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
---------------------------------------------------------------------------------------- */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using osVodigiWeb7.Extensions;
using System.Text;
using System.Configuration;
using osVodigiWeb6x.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace osVodigiWeb6x.Controllers
{
    public class LoginController : AbstractVodigiController
    {
        ILoginRepository repository;

        public LoginController(ILoginRepository paramrepository, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
            repository = paramrepository;
        }


        //
        // GET: /Login/

        public ActionResult Validate()
        {
            try
            {
//TODO check if alreday logged

                // Display the free links if appropriate
                ViewData["FreeLinks"] = "";
                if (_configuration.GetValue<bool>("ShowFreeLinks") == true)
                    ViewData["FreeLinks"] = BuildFreeLinks();

                // Display the system messages, if any
                ViewData["SystemMessages"] = BuildSystemMessages();

                ViewData["Username"] = String.Empty;
                ViewData["Password"] = String.Empty;
                ViewData["ValidationMessage"] = String.Empty;
                ViewData["LoginInfo"] = "Please log in.";

                return View();
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Login", "Validate", ex);
            }
        }

        //
        // POST: /Login/

        [HttpPost]
        public ActionResult Validate(FormCollection collection)
        {
            try
            {
                // Validate the login
                User user = repository.ValidateLogin(Request.Form["txtUsername"].ToString(), Request.Form["txtPassword"].ToString());

                ViewData["FreeLinks"] = "";
                if (_configuration.GetValue<bool>("ShowFreeLinks") == true)
                    ViewData["FreeLinks"] = BuildFreeLinks();

                // Display the system messages, if any
                ViewData["SystemMessages"] = BuildSystemMessages();

                if (user == null)
                {
                    ViewData["Username"] = Request.Form["txtUsername"].ToString();
                    ViewData["Password"] = String.Empty;
                    ViewData["ValidationMessage"] = "Invalid Login. Please try again.";
                    ViewData["LoginInfo"] = "Please log in.";

                    return View();
                }
                else
                {
                    HttpContext.Session.Set<User>("User", user);
                    HttpContext.Session.SetInt32("UserAccountID", user.AccountID);

                    IAccountRepository acctrep = new EntityAccountRepository();
                    Account account = acctrep.GetAccount(user.AccountID);
                    HttpContext.Session.SetString("UserAccountName", account.AccountName);


                    HttpContext.Session.SetString("LoginInfo", Utility.BuildUserAccountString(user.Username, account.AccountName));

                    if (user.IsAdmin)
                    {
                        HttpContext.Session.SetString("txtIsAdmin", true.ToString());
                    }
                    else
                    {
                        HttpContext.Session.SetString("txtIsAdmin", false.ToString());
                    }

                    // Make sure the Account Folders exist
                    string serverpath = GetHostFolder("~/UploadedFiles/");
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"/Images");
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"/Videos");
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"/Music");

                    serverpath = GetHostFolder("~/Media/");
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"/Images");
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"/Videos");
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"/Music");

                    // Create example data for the account (if appropriate)
                    IPlayerGroupRepository pgrep = new EntityPlayerGroupRepository();
                    IEnumerable<PlayerGroup> groups = pgrep.GetAllPlayerGroups(account.AccountID);

                    // Log the login
                    ILoginLogRepository llrep = new EntityLoginLogRepository();
                    LoginLog loginlog = new LoginLog();
                    loginlog.AccountID = user.AccountID;
                    loginlog.UserID = user.UserID;
                    loginlog.Username = user.Username;
                    loginlog.LoginDateTime = DateTime.Now.ToUniversalTime();
                    llrep.CreateLoginLog(loginlog);

                    return RedirectToAction("Index", "PlayerGroup");

                }
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Login", "Validate POST", ex);
                
            }
        }

        private void setSession() {

        }

        private string BuildFreeLinks()
        {
            try
            {
                StringBuilder links = new StringBuilder();

                links.Append("<span id=\"freelink\"><a href=\"http://www.vodigi.com/VodigiAccountSignUp.aspx\" target=\"_blank\">New to Vodigi? Click to Request a New Account.</a></span>");
                links.Append("<br />");
                links.Append("<span id=\"freelink\"><a href=\"http://free.vodigi.com/osVodigiPlayerSetup52.msi\" target=\"_blank\">Download the Vodigi Player Installer</a></span>");
                links.Append("<br />");
                links.Append("<span id=\"freelink\"><a href=\"http://www.vodigi.com/VodigiPlayerDeviceConfiguration.pdf\" target=\"_blank\">View the Vodigi Player Configuration Guide</a></span>");
                links.Append("<br />");
                links.Append("<span id=\"freelink\"><a href=\"http://www.youtube.com/user/VodigiDigitalSignage?ob=0&feature=results_main\" target=\"_blank\">View Videos to Help You Get Started</a></span>");

                return links.ToString();
            }
            catch { return String.Empty; }

        }

        private string BuildSystemMessages()
        {
            try
            {
                StringBuilder msgstext = new StringBuilder();
                msgstext.Append("");

                ISystemMessageRepository msgrep = new EntitySystemMessageRepository();
                IEnumerable<SystemMessage> msgs = msgrep.GetSystemMessagesByDate(DateTime.Today);
                if (msgs != null && msgs.Any())
                {
                    msgstext.Append("<b>System Messages:</b><br>");
                    msgstext.Append("<ul>");
                    foreach (SystemMessage msg in msgs)
                    {
                        msgstext.Append("<li>");
                        msgstext.Append("<b>" + msg.SystemMessageTitle + ": " + "</b>" + msg.SystemMessageBody);
                        msgstext.Append("</li>");
                    }
                    msgstext.Append("</ul>");
                }

                return msgstext.ToString();
            }
            catch { return String.Empty; }

        }
    }
}
