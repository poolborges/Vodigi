﻿/* ----------------------------------------------------------------------------------------
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
using System.Text.RegularExpressions;
using osVodigiWeb7x.Models;
using osVodigiWeb7x.Controllers;

namespace osVodigiWeb7x.Areas.Backoffice.Controllers
{
    [Area("Backoffice")]
    [Route("Backoffice/[controller]")]
    public class UserController : Controller
    {
        readonly IUserRepository userRepository;
        readonly IAccountRepository accountRepository;

        public UserController(IUserRepository _userRepository, IAccountRepository _accountRepository)
        {
            userRepository = _userRepository;
            accountRepository = _accountRepository;
        }

        //
        // GET: /User/

        public ActionResult Index()
        {
            try
            {
                AuthUtils.CheckIfAdmin();

                // Initialize or get the page state using session
                UserPageState pagestate = GetPageState();

                // Set and save the page state to the submitted form values if any values are passed
                if (!String.IsNullOrEmpty(Request.Form["lstAscDesc"]))
                {
                    pagestate.AccountID = Convert.ToInt32(Request.Form["lstAccount"]);
                    pagestate.Username = Request.Form["txtUsername"].ToString().Trim();
                    if (Request.Form["chkIncludeInactive"].ToString().ToLower().StartsWith("true"))
                        pagestate.IncludeInactive = true;
                    else
                        pagestate.IncludeInactive = false;
                    pagestate.SortBy = Request.Form["lstSortBy"].ToString().Trim();
                    pagestate.AscDesc = Request.Form["lstAscDesc"].ToString().Trim();
                    pagestate.PageNumber = Convert.ToInt32(Request.Form["txtPageNumber"].ToString().Trim());
                    SavePageState(pagestate);
                }

                // Add the session values to the view data so they can be populated in the form
                ViewData["AccountID"] = pagestate.AccountID;
                ViewData["Username"] = pagestate.Username;
                ViewData["IncludeInactive"] = pagestate.IncludeInactive;
                ViewData["SortBy"] = pagestate.SortBy;
                ViewData["SortByList"] = new SelectList(BuildSortByList(), "Value", "Text", pagestate.SortBy);
                ViewData["AscDescList"] = new SelectList(BuildAscDescList(), "Value", "Text", pagestate.AscDesc);
                ViewData["AccountList"] = new SelectList(BuildAccountList(true), "Value", "Text", pagestate.AccountID);

                // Determine asc/desc
                bool isdescending = false;
                if (pagestate.AscDesc.ToLower().StartsWith("d"))
                    isdescending = true;

                // Get a Count of all filtered records
                int recordcount = userRepository.GetUserRecordCount(pagestate.AccountID, pagestate.Username, pagestate.IncludeInactive);

                // Determine the page count
                int pagecount = 1;
                if (recordcount > 0)
                {
                    pagecount = recordcount / Constants.PageSize;
                    if (recordcount % Constants.PageSize != 0) // Add a page if there are more records
                    {
                        pagecount = pagecount + 1;
                    }
                }

                // Make sure the current page is not greater than the page count
                if (pagestate.PageNumber > pagecount)
                {
                    pagestate.PageNumber = pagecount;
                    SavePageState(pagestate);
                }

                // Set the page number and account in viewdata
                ViewData["PageNumber"] = Convert.ToString(pagestate.PageNumber);
                ViewData["PageCount"] = Convert.ToString(pagecount);
                ViewData["RecordCount"] = Convert.ToString(recordcount);

                // We need to add the account name
                IEnumerable<User> users = userRepository.GetUserPage(pagestate.AccountID, pagestate.Username, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount);
                List<UserView> userviews = new List<UserView>();
                
                foreach (User user in users)
                {
                    UserView userview = new UserView();
                    userview.UserID = user.UserID;
                    userview.AccountID = user.AccountID;
                    Account acct = accountRepository.GetAccount(user.AccountID);
                    userview.AccountName = acct.AccountName;
                    userview.Username = user.Username;
                    userview.FirstName = user.FirstName;
                    userview.LastName = user.LastName;
                    userview.EmailAddress = user.EmailAddress;
                    userview.IsAdmin = user.IsAdmin;
                    userview.IsActive = user.IsActive;

                    userviews.Add(userview);
                }

                ViewResult result = View(userviews);
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("User", "Index", ex);
                
            }
        }

        //
        // GET: /User/Create

        public ActionResult Create()
        {
            try
            {
                AuthUtils.CheckIfAdmin();

                ViewData["AccountList"] = new SelectList(BuildAccountList(0), "Value", "Text", "");
                ViewData["ValidationMessage"] = String.Empty;

                return View(CreateNewUser());
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("User", "Create", ex);
                
            }
        }

        //
        // POST: /User/Create

        [HttpPost]
        public ActionResult Create(User user)
        {
            try
            {
                AuthUtils.CheckIfAdmin();

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    user = FillNulls(user);

                    user.AccountID = Convert.ToInt32(Request.Form["lstAllAccounts"]);

                    string confirmpassword = Convert.ToString(Request.Form["txtConfirmPassword"]);
                    string validation = ValidateInput(user, confirmpassword, false, true);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["AccountList"] = new SelectList(BuildAccountList(Convert.ToInt32(Request.Form["lstAllAccounts"])), "Value", "Text", Request.Form["lstAllAccounts"]);
                        ViewData["ValidationMessage"] = validation;
                        return View(user);
                    }
                    else
                    {
                        userRepository.CreateUser(user);

                        CommonMethods.CreateActivityLog(HttpContext.Session.Get<User>("User"), "User", "Add",
                            "Added user '" + user.Username + "' - ID: " + user.UserID.ToString());

                        return RedirectToAction("Index");
                    }
                }
                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("User", "Create POST", ex);
                
            }
        }

        //
        // GET: /User/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                AuthUtils.CheckIfAdmin();

                User user = userRepository.GetUser(id);
                ViewData["AccountList"] = new SelectList(BuildAccountList(user.AccountID), "Value", "Text", user.AccountID.ToString());
                ViewData["ValidationMessage"] = String.Empty;

                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("User", "Edit", ex);
                
            }
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        public ActionResult Edit(User user)
        {
            try
            {
                AuthUtils.CheckIfAdmin();

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    user = FillNulls(user);

                    user.AccountID = Convert.ToInt32(Request.Form["lstAllAccounts"]);

                    string validation = ValidateInput(user, user.Password, true, false);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["AccountList"] = new SelectList(BuildAccountList(Convert.ToInt32(Request.Form["lstAllAccounts"])), "Value", "Text", Request.Form["lstAllAccounts"]);
                        ViewData["ValidationMessage"] = validation;
                        return View(user);
                    }

                    userRepository.UpdateUser(user);

                    CommonMethods.CreateActivityLog(HttpContext.Session.Get<User>("User"), "User", "Edit",
                        "Edited user '" + user.Username + "' - ID: " + user.UserID.ToString());

                    return RedirectToAction("Index");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("User", "Edit POST", ex);
                
            }
        }

        //
        // GET: /User/UpdatePassword/5

        public ActionResult UpdatePassword(int id)
        {
            try
            {
                AuthUtils.CheckIfAdmin();

                User user = userRepository.GetUser(id);
                ViewData["ValidationMessage"] = String.Empty;

                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("User", "Update Password", ex);
                
            }
        }

        //
        // POST: /User/UpdatePassword/5

        [HttpPost]
        public ActionResult UpdatePassword(User user)
        {
            try
            {
                AuthUtils.CheckIfAdmin();

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    user = FillNulls(user);

                    string confirmpassword = Convert.ToString(Request.Form["txtConfirmPassword"]);
                    string validation = ValidateInput(user, confirmpassword, true, true);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(user);
                    }

                    userRepository.UpdateUser(user);

                    CommonMethods.CreateActivityLog(HttpContext.Session.Get<User>("User"), "User", "Change Password",
                        "Changed password for '" + user.Username + "' - ID: " + user.UserID.ToString());

                    return RedirectToAction("Index");
                }

                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("User", "UpdatePassword POST", ex);
                
            }
        }

        //
        // Support Methods

        private List<SelectListItem> BuildSortByList()
        {
            // Build the sort by list
            List<SelectListItem> sortitems = new List<SelectListItem>();

            SelectListItem sortitem1 = new SelectListItem();
            sortitem1.Text = "Username";
            sortitem1.Value = "Username";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "First Name";
            sortitem2.Value = "FirstName";

            SelectListItem sortitem3 = new SelectListItem();
            sortitem3.Text = "Last Name";
            sortitem3.Value = "LastName";

            SelectListItem sortitem4 = new SelectListItem();
            sortitem4.Text = "Is Active";
            sortitem4.Value = "IsActive";

            sortitems.Add(sortitem1);
            sortitems.Add(sortitem2);
            sortitems.Add(sortitem3);
            sortitems.Add(sortitem4);

            return sortitems;
        }

        private List<SelectListItem> BuildAscDescList()
        {
            // Build the asc desc list
            List<SelectListItem> ascdescitems = new List<SelectListItem>();

            SelectListItem ascdescitem1 = new SelectListItem();
            ascdescitem1.Text = "Asc";
            ascdescitem1.Value = "Asc";

            SelectListItem ascdescitem2 = new SelectListItem();
            ascdescitem2.Text = "Desc";
            ascdescitem2.Value = "Desc";

            ascdescitems.Add(ascdescitem1);
            ascdescitems.Add(ascdescitem2);

            return ascdescitems;
        }

        private List<SelectListItem> BuildAccountList(int accountid)
        {
            // Build the account list
            List<SelectListItem> accountitems = new List<SelectListItem>();

            IEnumerable<Account> accounts = accountRepository.GetAllAccounts();
            foreach (Account account in accounts)
            {
                SelectListItem item = new SelectListItem();
                item.Text = account.AccountName;
                item.Value = account.AccountID.ToString();
                accountitems.Add(item);
            }

            return accountitems;
        }

        private List<SelectListItem> BuildAccountList(bool addAllItem)
        {
            // Build the player group list
            List<SelectListItem> acctitems = new List<SelectListItem>();

            // Add an 'All' item at the top
            if (addAllItem)
            {
                SelectListItem all = new SelectListItem();
                all.Text = "All Accounts";
                all.Value = "0";
                acctitems.Add(all);
            }

            IEnumerable<Account> accts = accountRepository.GetAllAccounts();
            foreach (Account acct in accts)
            {
                SelectListItem item = new SelectListItem();
                item.Text = acct.AccountName;
                item.Value = acct.AccountID.ToString();

                acctitems.Add(item);
            }

            return acctitems;
        }

        private UserPageState GetPageState()
        {
            try
            {
                UserPageState pagestate = HttpContext.Session.Get<UserPageState>("UserPageState");


                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (pagestate == null)
                {
                    int accountid = 0;
                    pagestate = new UserPageState
                    {
                        AccountID = accountid,
                        Username = String.Empty,
                        IncludeInactive = false,
                        SortBy = "Username",
                        AscDesc = "Ascending",
                        PageNumber = 1
                    };
                    SavePageState(pagestate);
                }
                return pagestate;
            }
            catch { return new UserPageState(); }
        }

        private void SavePageState(UserPageState pagestate)
        {
            HttpContext.Session.Set<UserPageState>("UserPageState", pagestate);
        }

        private User FillNulls(User user)
        {
            if (user.EmailAddress == null) user.EmailAddress = String.Empty;

            return user;
        }

        private User CreateNewUser()
        {
            User user = new User();
            user.UserID = 0;
            user.AccountID = 0;
            user.Username = String.Empty;
            user.Password = String.Empty;
            user.FirstName = String.Empty;
            user.LastName = String.Empty;
            user.EmailAddress = String.Empty;
            user.IsActive = true;

            return user;
        }

        private string ValidateInput(User user, string confirmpassword, bool isEdit, bool passwordchanged)
        {
            if (user.AccountID == 0)
                return "Account ID is not valid.";

            if (String.IsNullOrEmpty(user.Username))
                return "Username is required.";

            if (user.Username.Length < 6)
                return "Username must be at least 6 characters.";

            if (!isEdit)
            {
                User usercheck = userRepository.GetUserByUsername(user.Username);
                if (usercheck != null)
                    return "This username already exists.";
            }

            if (String.IsNullOrEmpty(user.Password))
                return "Password is required.";

            if (!isEdit || (isEdit && passwordchanged))
            {
                if (user.Password != confirmpassword)
                    return "Passwords do not match.";

                if (user.Password.Length < 6)
                    return "Password must be at least 6 characters.";
            }

            if (String.IsNullOrEmpty(user.FirstName) || String.IsNullOrEmpty(user.LastName))
                return "First Name and Last Name are required.";

            if (String.IsNullOrEmpty(user.EmailAddress))
                return "Email address is required.";

            Regex regex = new Regex(@"^[a-z0-9,!#\$%&'\*\+/=\?\^_`\{\|}~-]+(\.[a-z0-9,!#\$%&'\*\+/=\?\^_`\{\|}~-]+)*@[a-z0-9-]+(\.[a-z0-9-]+)*\.([a-z]{2,})$");
            if (!regex.IsMatch(user.EmailAddress))
                return "Email address is invalid.";


            return String.Empty;
        }
    }
}
