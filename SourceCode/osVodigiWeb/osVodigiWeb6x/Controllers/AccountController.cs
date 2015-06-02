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
using System.Web.Mvc;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class AccountController : Controller
    {

        IAccountRepository repository;

        public AccountController()
            : this(new EntityAccountRepository())
        { }

        public AccountController(IAccountRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /Account/

        public ActionResult Index()
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    throw new Exception("You are not authorized to access this page.");


                // Initialize or get the page state using session
                AccountPageState pagestate = GetPageState();

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountName = Request.Form["txtAccountName"].ToString().Trim();
                    pagestate.Description = Request.Form["txtDescription"].ToString().Trim();
                    if (Request.Form["chkIncludeInactive"].ToLower().StartsWith("true"))
                        pagestate.IncludeInactive = true;
                    else
                        pagestate.IncludeInactive = false;
                    pagestate.SortBy = Request.Form["lstSortBy"].ToString().Trim();
                    pagestate.AscDesc = Request.Form["lstAscDesc"].ToString().Trim();
                    pagestate.PageNumber = Convert.ToInt32(Request.Form["txtPageNumber"].ToString().Trim());
                    SavePageState(pagestate);
                }

                // Add the session values to the view data so they can be populated in the form
                ViewData["AccountName"] = pagestate.AccountName;
                ViewData["Description"] = pagestate.Description;
                ViewData["IncludeInactive"] = pagestate.IncludeInactive;
                ViewData["SortBy"] = pagestate.SortBy;
                ViewData["SortByList"] = new SelectList(BuildSortByList(), "Value", "Text", pagestate.SortBy);
                ViewData["AscDescList"] = new SelectList(BuildAscDescList(), "Value", "Text", pagestate.AscDesc);

                // Determine asc/desc
                bool isdescending = false;
                if (pagestate.AscDesc.ToLower().StartsWith("d"))
                    isdescending = true;

                // Get a Count of all filtered records
                int recordcount = repository.GetAccountRecordCount(pagestate.AccountName, pagestate.Description, pagestate.IncludeInactive);

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

                ViewResult result = View(repository.GetAccountPage(pagestate.AccountName, pagestate.Description, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Account", "Index", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Account/Create

        public ActionResult Create()
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    throw new Exception("You are not authorized to access this page.");

                ViewData["ValidationMessage"] = String.Empty;

                return View(CreateNewAccount());
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Account", "Create", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Account/Create

        [HttpPost]
        public ActionResult Create(Account account)
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    throw new Exception("You are not authorized to access this page.");

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    account = FillNulls(account);

                    string validation = ValidateInput(account);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(account);
                    }

                    repository.CreateAccount(account);
                    repository.CreateExampleData(account.AccountID);

                    CommonMethods.CreateActivityLog((User)Session["User"], "Account", "Add",
                                                    "Added account '" + account.AccountName + "' - ID: " + account.AccountID.ToString());

                    return RedirectToAction("Index");
                }

                return View(account);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Account", "Create POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Account/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    throw new Exception("You are not authorized to access this page.");

                Account account = repository.GetAccount(id);
                ViewData["ValidationMessage"] = String.Empty;

                return View(account);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Account", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Account/Edit/5

        [HttpPost]
        public ActionResult Edit(Account account)
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    throw new Exception("You are not authorized to access this page.");

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    account = FillNulls(account);

                    string validation = ValidateInput(account);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(account);
                    }

                    repository.UpdateAccount(account);

                    CommonMethods.CreateActivityLog((User)Session["User"], "Account", "Edit",
                                                    "Edited account '" + account.AccountName + "' - ID: " + account.AccountID.ToString());

                    return RedirectToAction("Index");
                }

                return View(account);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Account", "Edit POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // Support Methods

        private List<SelectListItem> BuildSortByList()
        {
            // Build the sort by list
            List<SelectListItem> sortitems = new List<SelectListItem>();

            SelectListItem sortitem1 = new SelectListItem();
            sortitem1.Text = "Account Name";
            sortitem1.Value = "AccountName";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Description";
            sortitem2.Value = "AccountDescription";

            SelectListItem sortitem3 = new SelectListItem();
            sortitem3.Text = "Is Active";
            sortitem3.Value = "IsActive";

            sortitems.Add(sortitem1);
            sortitems.Add(sortitem2);
            sortitems.Add(sortitem3);

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

        private AccountPageState GetPageState()
        {
            try
            {
                AccountPageState pagestate = new AccountPageState();


                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["AccountPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountName = String.Empty;
                    pagestate.Description = String.Empty;
                    pagestate.IncludeInactive = false;
                    pagestate.SortBy = "AccountName";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    Session["AccountPageState"] = pagestate;
                }
                else
                {
                    pagestate = (AccountPageState)Session["AccountPageState"];
                }
                return pagestate;
            }
            catch { return new AccountPageState(); }
        }

        private void SavePageState(AccountPageState pagestate)
        {
            Session["AccountPageState"] = pagestate;
        }

        private Account FillNulls(Account account)
        {
            if (account.AccountDescription == null) account.AccountDescription = String.Empty;
            if (account.FTPServer == null) account.FTPServer = String.Empty;
            if (account.FTPUsername == null) account.FTPUsername = String.Empty;
            if (account.FTPPassword == null) account.FTPPassword = String.Empty;

            return account;
        }

        private Account CreateNewAccount()
        {
            Account account = new Account();
            account.AccountID = 0;
            account.AccountName = String.Empty;
            account.AccountDescription = String.Empty;
            account.FTPServer = String.Empty;
            account.FTPUsername = String.Empty;
            account.FTPPassword = String.Empty;
            account.IsActive = true;

            return account;
        }

        private string ValidateInput(Account account)
        {
            if (String.IsNullOrEmpty(account.AccountName))
                return "Account Name is required.";

            if (!String.IsNullOrEmpty(account.FTPServer) && !account.FTPServer.ToLower().StartsWith(@"ftp://"))
                return "FTP Server must start with ftp://";

            return String.Empty;
        }
    
    }
}