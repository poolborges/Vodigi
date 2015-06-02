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
using System.Web.Mvc;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class SystemMessageController : Controller
    {
        ISystemMessageRepository repository;

        public SystemMessageController()
            : this(new EntitySystemMessageRepository())
        { }

        public SystemMessageController(ISystemMessageRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /SystemMessage/

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
                SystemMessagePageState pagestate = GetPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.SystemMessageTitle = Request.Form["txtTitle"].ToString().Trim();
                    pagestate.SystemMessageBody = Request.Form["txtBody"].ToString().Trim();
                    pagestate.SortBy = Request.Form["lstSortBy"].ToString().Trim();
                    pagestate.AscDesc = Request.Form["lstAscDesc"].ToString().Trim();
                    pagestate.PageNumber = Convert.ToInt32(Request.Form["txtPageNumber"].ToString().Trim());
                    SavePageState(pagestate);
                }

                // Add the session values to the view data so they can be populated in the form
                ViewData["SystemMessageTitle"] = pagestate.SystemMessageTitle;
                ViewData["SystemMessageBody"] = pagestate.SystemMessageBody;
                ViewData["SortBy"] = pagestate.SortBy;
                ViewData["SortByList"] = new SelectList(BuildSortByList(), "Value", "Text", pagestate.SortBy);
                ViewData["AscDescList"] = new SelectList(BuildAscDescList(), "Value", "Text", pagestate.AscDesc);

                // Determine asc/desc
                bool isdescending = false;
                if (pagestate.AscDesc.ToLower().StartsWith("d"))
                    isdescending = true;

                // Get a Count of all filtered records
                int recordcount = repository.GetSystemMessageRecordCount(pagestate.SystemMessageBody, pagestate.SystemMessageBody);

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

                ViewResult result = View(repository.GetSystemMessagePage(pagestate.SystemMessageTitle, pagestate.SystemMessageBody, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SystemMessage", "Index", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /SystemMessage/Create

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

                return View(CreateNewSystemMessage());
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SystemMessage", "Create", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }


        // POST: /SystemMessage/Create

        [HttpPost]
        public ActionResult Create(SystemMessage systemmessage)
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
                    string validation = ValidateInput(systemmessage);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(systemmessage);
                    }
                    else
                    {
                        repository.CreateSystemMessage(systemmessage);

                        CommonMethods.CreateActivityLog((User)Session["User"], "SystemMessage", "Add",
                            "Added system message '" + systemmessage.SystemMessageTitle + "' - ID: " + systemmessage.SystemMessageID.ToString());

                        return RedirectToAction("Index", "SystemMessage", new { id = systemmessage.SystemMessageID });
                    }
                }
                return View(systemmessage);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SystemMessage", "Create POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /SystemMessage/Edit/5

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

                SystemMessage systemmessage = repository.GetSystemMessage(id);
                ViewData["ValidationMessage"] = String.Empty;

                return View(systemmessage);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SystemMessage", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /SystemMessage/Edit/5

        [HttpPost]
        public ActionResult Edit(SystemMessage systemmessage)
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
                    string validation = ValidateInput(systemmessage);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(systemmessage);
                    }

                    repository.UpdateSystemMessage(systemmessage);

                    CommonMethods.CreateActivityLog((User)Session["User"], "System Message", "Edit",
                                                    "Edited system message '" + systemmessage.SystemMessageTitle + "' - ID: " + systemmessage.SystemMessageID.ToString());

                    return RedirectToAction("Index");
                }

                return View(systemmessage);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SystemMessage", "Edit POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /SystemMessage/Delete/5

        public ActionResult Delete(int id)
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

                SystemMessage systemmessage = repository.GetSystemMessage(id);

                repository.DeleteSystemMessage(systemmessage);

                return RedirectToAction("Index", "SystemMessage");
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SystemMessage", "Delete", ex.Message);
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
            sortitem1.Text = "Title";
            sortitem1.Value = "SystemMessageTitle";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Body";
            sortitem2.Value = "SystemMessageBody";

            SelectListItem sortitem3 = new SelectListItem();
            sortitem3.Text = "Priority";
            sortitem3.Value = "Priority";

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

        private SystemMessagePageState GetPageState()
        {
            try
            {
                SystemMessagePageState pagestate = new SystemMessagePageState();

                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["SystemMessagePageState"] == null)
                {
                    pagestate.SystemMessageTitle = String.Empty;
                    pagestate.SystemMessageBody = String.Empty;
                    pagestate.SortBy = "SystemMessageTitle";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    Session["SystemMessagePageState"] = pagestate;
                }
                else
                {
                    pagestate = (SystemMessagePageState)Session["SystemMessagePageState"];
                }
                return pagestate;
            }
            catch { return new SystemMessagePageState(); }
        }

        private void SavePageState(SystemMessagePageState pagestate)
        {
            Session["SystemMessagePageState"] = pagestate;
        }

        private SystemMessage CreateNewSystemMessage()
        {
            SystemMessage systemmessage = new SystemMessage();
            systemmessage.SystemMessageID = 0;
            systemmessage.SystemMessageTitle = String.Empty;
            systemmessage.SystemMessageBody = String.Empty;
            systemmessage.DisplayDateStart = DateTime.Now;
            systemmessage.DisplayDateEnd = DateTime.Now;
            systemmessage.Priority = 1;

            return systemmessage;
        }

        private string ValidateInput(SystemMessage systemmessage)
        {
            if (String.IsNullOrEmpty(systemmessage.SystemMessageTitle))
                return "Title is required.";

            if (String.IsNullOrEmpty(systemmessage.SystemMessageBody))
                return "Body is required.";

            if (systemmessage.DisplayDateStart > systemmessage.DisplayDateEnd)
                return "Start Date must be before End Date.";

            return String.Empty;
        }
    }
}
