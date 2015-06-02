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
using System.Text;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class ReportController : Controller
    {
        IReportRepository repository;

        public ReportController()
            : this(new EntityReportRepository())
        { }

        public ReportController(IReportRepository paramrepository)
        {
            repository = paramrepository;
        }

        //---------------------------------------------------------------------------
        // GET: /Report/

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
                    ViewData["txtIsAdmin"] = "false";

                return View();
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Report", "Index", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //----------------------------------------------------------------------------
        // GET: /Report/ReportLoginLog/

        public ActionResult ReportLoginLog()
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
                    ViewData["txtIsAdmin"] = "false";

                // Initialize or get the page state using session
                ReportLoginLogPageState pagestate = GetReportLoginLogPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.Username = Request.Form["txtUsername"].Trim();
                    DateTime startdate = DateTime.Today.AddDays(-30);
                    DateTime enddate = DateTime.Today;
                    try
                    {
                        startdate = Convert.ToDateTime(Request.Form["txtStartDate"]);
                    }
                    catch { }
                    try
                    {
                        enddate = Convert.ToDateTime(Request.Form["txtEndDate"]);
                    }
                    catch { }
                    pagestate.StartDate = startdate.ToShortDateString();
                    pagestate.EndDate = enddate.ToShortDateString();
                    pagestate.SortBy = Request.Form["lstSortBy"].ToString().Trim();
                    pagestate.AscDesc = Request.Form["lstAscDesc"].ToString().Trim();
                    pagestate.PageNumber = Convert.ToInt32(Request.Form["txtPageNumber"].ToString().Trim());
                    SavePageState(pagestate);
                }

                // Add the session values to the view data so they can be populated in the form
                ViewData["AccountID"] = pagestate.AccountID;
                ViewData["Username"] = pagestate.Username;
                ViewData["StartDate"] = pagestate.StartDate;
                ViewData["EndDate"] = pagestate.EndDate;
                ViewData["SortBy"] = pagestate.SortBy;
                ViewData["SortByList"] = new SelectList(BuildReportLoginLogSortByList(), "Value", "Text", pagestate.SortBy);
                ViewData["AscDescList"] = new SelectList(BuildAscDescList(), "Value", "Text", pagestate.AscDesc);

                // Determine asc/desc
                bool isdescending = false;
                if (pagestate.AscDesc.ToLower().StartsWith("d"))
                    isdescending = true;

                // Get a Count of all filtered records
                int recordcount = repository.ReportLoginLogRecordCount(pagestate.AccountID, pagestate.Username, Convert.ToDateTime(pagestate.StartDate), Convert.ToDateTime(pagestate.EndDate).AddDays(1));

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

                ViewResult result = View(repository.ReportLoginLogPage(pagestate.AccountID, pagestate.Username, Convert.ToDateTime(pagestate.StartDate), Convert.ToDateTime(pagestate.EndDate).AddDays(1), pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "ReportLoginLog";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Report", "ReportLoginLog", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        private ReportLoginLogPageState GetReportLoginLogPageState()
        {
            try
            {
                ReportLoginLogPageState pagestate = new ReportLoginLogPageState();

                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["ReportLoginLogPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.Username = String.Empty;
                    pagestate.StartDate = DateTime.Today.AddDays(-30).ToShortDateString();
                    pagestate.EndDate = DateTime.Today.ToShortDateString();
                    pagestate.SortBy = "LoginDateTime";
                    pagestate.AscDesc = "Desc";
                    pagestate.PageNumber = 1;
                    Session["ReportLoginLogPageState"] = pagestate;
                }
                else
                {
                    pagestate = (ReportLoginLogPageState)Session["ReportLoginLogPageState"];
                }
                return pagestate;
            }
            catch { return new ReportLoginLogPageState(); }
        }

        private void SavePageState(ReportLoginLogPageState pagestate)
        {
            Session["ReportLoginLogPageState"] = pagestate;
        }

        private List<SelectListItem> BuildReportLoginLogSortByList()
        {
            // Build the sort by list
            List<SelectListItem> sortitems = new List<SelectListItem>();

            SelectListItem sortitem1 = new SelectListItem();
            sortitem1.Text = "Username";
            sortitem1.Value = "Username";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Login Date Time";
            sortitem2.Value = "LoginDateTime";

            sortitems.Add(sortitem1);
            sortitems.Add(sortitem2);

            return sortitems;
        }


        //--------------------------------------------------------------------------------------
        // GET: /Report/ReportActivityLog/

        public ActionResult ReportActivityLog()
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
                    ViewData["txtIsAdmin"] = "false";

                // Initialize or get the page state using session
                ReportActivityLogPageState pagestate = GetReportActivityLogPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.Username = Request.Form["txtUsername"].Trim();
                    pagestate.EntityType = Request.Form["txtEntityType"].Trim();
                    pagestate.EntityAction = Request.Form["txtEntityAction"].Trim();
                    DateTime startdate = DateTime.Today.AddDays(-30);
                    DateTime enddate = DateTime.Today;
                    try
                    {
                        startdate = Convert.ToDateTime(Request.Form["txtStartDate"]);
                    }
                    catch { }
                    try
                    {
                        enddate = Convert.ToDateTime(Request.Form["txtEndDate"]);
                    }
                    catch { }
                    pagestate.StartDate = startdate.ToShortDateString();
                    pagestate.EndDate = enddate.ToShortDateString();
                    pagestate.SortBy = Request.Form["lstSortBy"].ToString().Trim();
                    pagestate.AscDesc = Request.Form["lstAscDesc"].ToString().Trim();
                    pagestate.PageNumber = Convert.ToInt32(Request.Form["txtPageNumber"].ToString().Trim());
                    SavePageState(pagestate);
                }

                // Add the session values to the view data so they can be populated in the form
                ViewData["AccountID"] = pagestate.AccountID;
                ViewData["Username"] = pagestate.Username;
                ViewData["EntityType"] = pagestate.EntityType;
                ViewData["EntityAction"] = pagestate.EntityAction;
                ViewData["StartDate"] = pagestate.StartDate;
                ViewData["EndDate"] = pagestate.EndDate;
                ViewData["SortBy"] = pagestate.SortBy;
                ViewData["SortByList"] = new SelectList(BuildReportActivityLogSortByList(), "Value", "Text", pagestate.SortBy);
                ViewData["AscDescList"] = new SelectList(BuildAscDescList(), "Value", "Text", pagestate.AscDesc);

                // Determine asc/desc
                bool isdescending = false;
                if (pagestate.AscDesc.ToLower().StartsWith("d"))
                    isdescending = true;

                // Get a Count of all filtered records
                int recordcount = repository.ReportActivityLogRecordCount(pagestate.AccountID, pagestate.Username, pagestate.EntityType, pagestate.EntityAction, Convert.ToDateTime(pagestate.StartDate), Convert.ToDateTime(pagestate.EndDate).AddDays(1));

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

                ViewResult result = View(repository.ReportActivityLogPage(pagestate.AccountID, pagestate.Username, pagestate.EntityType, pagestate.EntityAction, Convert.ToDateTime(pagestate.StartDate), Convert.ToDateTime(pagestate.EndDate).AddDays(1), pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "ReportActivityLog";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Report", "ReportLoginLog", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        private ReportActivityLogPageState GetReportActivityLogPageState()
        {
            try
            {
                ReportActivityLogPageState pagestate = new ReportActivityLogPageState();

                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["ReportActivityLogPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.Username = String.Empty;
                    pagestate.EntityType = String.Empty;
                    pagestate.EntityAction = String.Empty;
                    pagestate.StartDate = DateTime.Today.AddDays(-30).ToShortDateString();
                    pagestate.EndDate = DateTime.Today.ToShortDateString();
                    pagestate.SortBy = "ActivityDateTime";
                    pagestate.AscDesc = "Desc";
                    pagestate.PageNumber = 1;
                    Session["ReportActivityLogPageState"] = pagestate;
                }
                else
                {
                    pagestate = (ReportActivityLogPageState)Session["ReportActivityLogPageState"];
                }
                return pagestate;
            }
            catch { return new ReportActivityLogPageState(); }
        }

        private void SavePageState(ReportActivityLogPageState pagestate)
        {
            Session["ReportActivityLogPageState"] = pagestate;
        }

        private List<SelectListItem> BuildReportActivityLogSortByList()
        {
            // Build the sort by list
            List<SelectListItem> sortitems = new List<SelectListItem>();

            SelectListItem sortitem1 = new SelectListItem();
            sortitem1.Text = "Username";
            sortitem1.Value = "Username";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Module";
            sortitem2.Value = "EntityType";

            SelectListItem sortitem3 = new SelectListItem();
            sortitem3.Text = "Action";
            sortitem3.Value = "EntityAction";

            SelectListItem sortitem4 = new SelectListItem();
            sortitem4.Text = "Activity Date Time";
            sortitem4.Value = "ActivityDateTime";

            sortitems.Add(sortitem1);
            sortitems.Add(sortitem2);
            sortitems.Add(sortitem3);
            sortitems.Add(sortitem4);

            return sortitems;
        }


        //------------------------------------------------------------------------------------------------
        // GET: /Report/ReportPlayerScreenLog/

        public ActionResult ReportPlayerScreenLog()
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
                    ViewData["txtIsAdmin"] = "false";

                // Initialize or get the page state using session
                ReportPlayerScreenLogPageState pagestate = GetReportPlayerScreenLogPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.PlayerName = Request.Form["txtPlayerName"].Trim();
                    pagestate.ScreenName = Request.Form["txtScreenName"].Trim();
                    DateTime startdate = DateTime.Today.AddDays(-30);
                    DateTime enddate = DateTime.Today;
                    try
                    {
                        startdate = Convert.ToDateTime(Request.Form["txtStartDate"]);
                    }
                    catch { }
                    try
                    {
                        enddate = Convert.ToDateTime(Request.Form["txtEndDate"]);
                    }
                    catch { }
                    pagestate.StartDate = startdate.ToShortDateString();
                    pagestate.EndDate = enddate.ToShortDateString();
                    pagestate.SortBy = Request.Form["lstSortBy"].ToString().Trim();
                    pagestate.AscDesc = Request.Form["lstAscDesc"].ToString().Trim();
                    pagestate.PageNumber = Convert.ToInt32(Request.Form["txtPageNumber"].ToString().Trim());
                    SavePageState(pagestate);
                }

                // Add the session values to the view data so they can be populated in the form
                ViewData["AccountID"] = pagestate.AccountID;
                ViewData["PlayerName"] = pagestate.PlayerName;
                ViewData["ScreenName"] = pagestate.ScreenName;
                ViewData["StartDate"] = pagestate.StartDate;
                ViewData["EndDate"] = pagestate.EndDate;
                ViewData["SortBy"] = pagestate.SortBy;
                ViewData["SortByList"] = new SelectList(BuildReportPlayerScreenLogSortByList(), "Value", "Text", pagestate.SortBy);
                ViewData["AscDescList"] = new SelectList(BuildAscDescList(), "Value", "Text", pagestate.AscDesc);

                // Determine asc/desc
                bool isdescending = false;
                if (pagestate.AscDesc.ToLower().StartsWith("d"))
                    isdescending = true;

                // Get a Count of all filtered records
                int recordcount = repository.ReportPlayerScreenLogRecordCount(pagestate.AccountID, pagestate.PlayerName, pagestate.ScreenName, Convert.ToDateTime(pagestate.StartDate), Convert.ToDateTime(pagestate.EndDate).AddDays(1));

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

                ViewResult result = View(repository.ReportPlayerScreenLogPage(pagestate.AccountID, pagestate.PlayerName, pagestate.ScreenName, Convert.ToDateTime(pagestate.StartDate), Convert.ToDateTime(pagestate.EndDate).AddDays(1), pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "ReportPlayerScreenLog";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Report", "ReportPlayerScreenLog", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        private ReportPlayerScreenLogPageState GetReportPlayerScreenLogPageState()
        {
            try
            {
                ReportPlayerScreenLogPageState pagestate = new ReportPlayerScreenLogPageState();

                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["ReportPlayerScreenLogPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.PlayerName = String.Empty;
                    pagestate.ScreenName = String.Empty;
                    pagestate.StartDate = DateTime.Today.AddDays(-30).ToShortDateString();
                    pagestate.EndDate = DateTime.Today.ToShortDateString();
                    pagestate.SortBy = "DisplayDateTime";
                    pagestate.AscDesc = "Desc";
                    pagestate.PageNumber = 1;
                    Session["ReportPlayerScreenLogPageState"] = pagestate;
                }
                else
                {
                    pagestate = (ReportPlayerScreenLogPageState)Session["ReportPlayerScreenLogPageState"];
                }
                return pagestate;
            }
            catch { return new ReportPlayerScreenLogPageState(); }
        }

        private void SavePageState(ReportPlayerScreenLogPageState pagestate)
        {
            Session["ReportPlayerScreenLogPageState"] = pagestate;
        }

        private List<SelectListItem> BuildReportPlayerScreenLogSortByList()
        {
            // Build the sort by list
            List<SelectListItem> sortitems = new List<SelectListItem>();

            SelectListItem sortitem1 = new SelectListItem();
            sortitem1.Text = "Player Name";
            sortitem1.Value = "PlayerName";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Screen Name";
            sortitem2.Value = "ScreenName";

            SelectListItem sortitem3 = new SelectListItem();
            sortitem3.Text = "Display Date/Time";
            sortitem3.Value = "DisplayDateTime";

            SelectListItem sortitem4 = new SelectListItem();
            sortitem4.Text = "Close Date/Time";
            sortitem4.Value = "CloseDateTime";

            sortitems.Add(sortitem1);
            sortitems.Add(sortitem2);
            sortitems.Add(sortitem3);
            sortitems.Add(sortitem4);

            return sortitems;
        }

        //------------------------------------------------------------------------------------------------
        // GET: /Report/ReportPlayerScreenContentLog/

        public ActionResult ReportPlayerScreenContentLog()
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
                    ViewData["txtIsAdmin"] = "false";

                // Initialize or get the page state using session
                ReportPlayerScreenContentLogPageState pagestate = GetReportPlayerScreenContentLogPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.PlayerName = Request.Form["txtPlayerName"].Trim();
                    pagestate.ScreenName = Request.Form["txtScreenName"].Trim();
                    pagestate.ContentName = Request.Form["txtContentName"].Trim();
                    pagestate.ContentType = Request.Form["txtContentType"].Trim();
                    DateTime startdate = DateTime.Today.AddDays(-30);
                    DateTime enddate = DateTime.Today;
                    try
                    {
                        startdate = Convert.ToDateTime(Request.Form["txtStartDate"]);
                    }
                    catch { }
                    try
                    {
                        enddate = Convert.ToDateTime(Request.Form["txtEndDate"]);
                    }
                    catch { }
                    pagestate.StartDate = startdate.ToShortDateString();
                    pagestate.EndDate = enddate.ToShortDateString();
                    pagestate.SortBy = Request.Form["lstSortBy"].ToString().Trim();
                    pagestate.AscDesc = Request.Form["lstAscDesc"].ToString().Trim();
                    pagestate.PageNumber = Convert.ToInt32(Request.Form["txtPageNumber"].ToString().Trim());
                    SavePageState(pagestate);
                }

                // Add the session values to the view data so they can be populated in the form
                ViewData["AccountID"] = pagestate.AccountID;
                ViewData["PlayerName"] = pagestate.PlayerName;
                ViewData["ScreenName"] = pagestate.ScreenName;
                ViewData["ContentName"] = pagestate.ContentName;
                ViewData["ContentType"] = pagestate.ContentType;
                ViewData["StartDate"] = pagestate.StartDate;
                ViewData["EndDate"] = pagestate.EndDate;
                ViewData["SortBy"] = pagestate.SortBy;
                ViewData["SortByList"] = new SelectList(BuildReportPlayerScreenContentLogSortByList(), "Value", "Text", pagestate.SortBy);
                ViewData["AscDescList"] = new SelectList(BuildAscDescList(), "Value", "Text", pagestate.AscDesc);

                // Determine asc/desc
                bool isdescending = false;
                if (pagestate.AscDesc.ToLower().StartsWith("d"))
                    isdescending = true;

                // Get a Count of all filtered records
                int recordcount = repository.ReportPlayerScreenContentLogRecordCount(pagestate.AccountID, pagestate.PlayerName, pagestate.ScreenName, pagestate.ContentName, pagestate.ContentType, Convert.ToDateTime(pagestate.StartDate), Convert.ToDateTime(pagestate.EndDate).AddDays(1));

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

                ViewResult result = View(repository.ReportPlayerScreenContentLogPage(pagestate.AccountID, pagestate.PlayerName, pagestate.ScreenName, pagestate.ContentName, pagestate.ContentType, Convert.ToDateTime(pagestate.StartDate), Convert.ToDateTime(pagestate.EndDate).AddDays(1), pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "ReportPlayerScreenContentLog";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Report", "ReportPlayerScreenContentLog", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        private ReportPlayerScreenContentLogPageState GetReportPlayerScreenContentLogPageState()
        {
            try
            {
                ReportPlayerScreenContentLogPageState pagestate = new ReportPlayerScreenContentLogPageState();

                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["ReportPlayerScreenContentLogPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.PlayerName = String.Empty;
                    pagestate.ScreenName = String.Empty;
                    pagestate.ContentName = String.Empty;
                    pagestate.ContentType = String.Empty;
                    pagestate.StartDate = DateTime.Today.AddDays(-30).ToShortDateString();
                    pagestate.EndDate = DateTime.Today.ToShortDateString();
                    pagestate.SortBy = "DisplayDateTime";
                    pagestate.AscDesc = "Desc";
                    pagestate.PageNumber = 1;
                    Session["ReportPlayerScreenContentLogPageState"] = pagestate;
                }
                else
                {
                    pagestate = (ReportPlayerScreenContentLogPageState)Session["ReportPlayerScreenContentLogPageState"];
                }
                return pagestate;
            }
            catch { return new ReportPlayerScreenContentLogPageState(); }
        }

        private void SavePageState(ReportPlayerScreenContentLogPageState pagestate)
        {
            Session["ReportPlayerScreenContentLogPageState"] = pagestate;
        }

        private List<SelectListItem> BuildReportPlayerScreenContentLogSortByList()
        {
            // Build the sort by list
            List<SelectListItem> sortitems = new List<SelectListItem>();

            SelectListItem sortitem1 = new SelectListItem();
            sortitem1.Text = "Player Name";
            sortitem1.Value = "PlayerName";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Screen Name";
            sortitem2.Value = "ScreenName";

            SelectListItem sortitem3 = new SelectListItem();
            sortitem3.Text = "Content Name";
            sortitem3.Value = "ScreenContentName";

            SelectListItem sortitem4 = new SelectListItem();
            sortitem4.Text = "Content Type";
            sortitem4.Value = "ScreenContentTypeName";

            SelectListItem sortitem5 = new SelectListItem();
            sortitem5.Text = "Display Date/Time";
            sortitem5.Value = "DisplayDateTime";

            SelectListItem sortitem6 = new SelectListItem();
            sortitem6.Text = "Close Date/Time";
            sortitem6.Value = "CloseDateTime";

            sortitems.Add(sortitem1);
            sortitems.Add(sortitem2);
            sortitems.Add(sortitem3);
            sortitems.Add(sortitem4);
            sortitems.Add(sortitem5);
            sortitems.Add(sortitem6);

            return sortitems;
        }

        //------------------------------------------------------------------------------------------------
        // GET: /Report/ReportSurveyResults/

        public ActionResult ReportSurveyResults()
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
                    ViewData["txtIsAdmin"] = "false";

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Initialize or get the page state using session
                ReportSurveyResultsPageState pagestate = GetReportSurveyResultsPageState();

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstSurvey"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.SurveyID = Convert.ToInt32(Request.Form["lstSurvey"].Trim());
                    DateTime startdate = DateTime.Today.AddDays(-30);
                    DateTime enddate = DateTime.Today;
                    try
                    {
                        startdate = Convert.ToDateTime(Request.Form["txtStartDate"]);
                    }
                    catch { }
                    try
                    {
                        enddate = Convert.ToDateTime(Request.Form["txtEndDate"]);
                    }
                    catch { }
                    pagestate.StartDate = startdate.ToShortDateString();
                    pagestate.EndDate = enddate.ToShortDateString();
                    SavePageState(pagestate);
                }

                // Add the session values to the view data so they can be populated in the form
                ViewData["AccountID"] = pagestate.AccountID;
                ViewData["Surveys"] = new SelectList(BuildReportSurveyResultsSurveyList(), "Value", "Text", pagestate.SurveyID);
                ViewData["StartDate"] = pagestate.StartDate;
                ViewData["EndDate"] = pagestate.EndDate;

                if (pagestate.SurveyID != 0)
                    ViewData["SurveyTable"] = BuildSurveyResultsTable(pagestate.SurveyID);
                else
                    ViewData["SurveyTable"] = String.Empty;

                ViewResult result = View();
                result.ViewName = "ReportSurveyResults";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Report", "ReportSurveyResults", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        private ReportSurveyResultsPageState GetReportSurveyResultsPageState()
        {
            try
            {
                ReportSurveyResultsPageState pagestate = new ReportSurveyResultsPageState();

                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["ReportSurveyResultsPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.SurveyID = 0;
                    pagestate.StartDate = DateTime.Today.AddDays(-30).ToShortDateString();
                    pagestate.EndDate = DateTime.Today.ToShortDateString();
                    Session["ReportSurveyResultsPageState"] = pagestate;
                }
                else
                {
                    pagestate = (ReportSurveyResultsPageState)Session["ReportSurveyResultsPageState"];
                }
                return pagestate;
            }
            catch { return new ReportSurveyResultsPageState(); }
        }

        private void SavePageState(ReportSurveyResultsPageState pagestate)
        {
            Session["ReportSurveyResultsPageState"] = pagestate;
        }

        private List<SelectListItem> BuildReportSurveyResultsSurveyList()
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Get the approved surveys
            ISurveyRepository surveyrep = new EntitySurveyRepository();
            IEnumerable<Survey> surveys = surveyrep.GetApprovedSurveys(accountid);

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (Survey survey in surveys)
            {
                SelectListItem item = new SelectListItem();
                item.Text = survey.SurveyName;
                item.Value = survey.SurveyID.ToString();
                items.Add(item);
            }

            return items;
        }

        private string BuildSurveyResultsTable(int surveyid)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                ISurveyRepository srep = new EntitySurveyRepository();
                Survey survey = srep.GetSurvey(surveyid);
                if (survey == null) return String.Empty;

                // Get the answered surveys
                IAnsweredSurveyRepository answeredsurveyrespository = new EntityAnsweredSurveyRepository();
                IEnumerable<AnsweredSurvey> answeredsurveys = answeredsurveyrespository.GetBySurveyID(surveyid);

                sb.AppendLine("<table style=\"border-spacing:0;border-collapse:collapse;\" class=\"surveytable\">");
                sb.AppendLine("<tr class=\"surveyrow\">");
                sb.AppendLine("<td class=\"gridtext\">" + survey.SurveyName + "</td>");
                sb.AppendLine("<td class=\"gridtext\"></td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:100px;\"></td>");
                sb.AppendLine("<td class=\"gridtext\"></td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:100px;\"></td>");
                sb.AppendLine("<td class=\"gridtext\"></td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:100px;\"></td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr class=\"surveysubheadrow\">");
                sb.AppendLine("<td class=\"gridtext\">Total Answered Survey Count: " + answeredsurveys.Count().ToString() + "</td>");
                sb.AppendLine("<td class=\"gridtext\" colspan=\"2\">Selected</td>");
                sb.AppendLine("<td class=\"gridtext\" colspan=\"2\">Deselected</td>");
                sb.AppendLine("<td class=\"gridtext\" colspan=\"2\">Unanswered</td>");
                sb.AppendLine("</tr>");

                // Loop through each question and question option
                ISurveyQuestionRepository qrep = new EntitySurveyQuestionRepository();
                ISurveyQuestionOptionRepository orep = new EntitySurveyQuestionOptionRepository();
                IAnsweredSurveyQuestionOptionRepository aorep = new EntityAnsweredSurveyQuestionOptionRepository();
                List<SurveyQuestion> questions = qrep.GetSurveyQuestions(survey.SurveyID).ToList();

                foreach (SurveyQuestion question in questions)
                {
                    sb.AppendLine("<tr class=\"questionrow\">");
                    sb.AppendLine("<td class=\"gridtext\">" + question.SurveyQuestionText + "</td>");
                    sb.AppendLine("<td class=\"gridtext\"></td>");
                    sb.AppendLine("<td class=\"gridtext\"></td>");
                    sb.AppendLine("<td class=\"gridtext\"></td>");
                    sb.AppendLine("<td class=\"gridtext\"></td>");
                    sb.AppendLine("<td class=\"gridtext\"></td>");
                    sb.AppendLine("<td class=\"gridtext\"></td>");
                    sb.AppendLine("</tr>");

                    // Loop through each question option
                    List<SurveyQuestionOption> options = orep.GetSurveyQuestionOptions(question.SurveyQuestionID).ToList();
                    foreach (SurveyQuestionOption option in options)
                    {
                        // Add the number of selected/unselected
                        double selectedcount = 0;
                        double deselectedcount = 0;
                        double unansweredcount = 0;

                        IEnumerable<AnsweredSurveyQuestionOption> aoptions = aorep.GetBySurveyQuestionOptionId(option.SurveyQuestionOptionID);
                        foreach (AnsweredSurveyQuestionOption aoption in aoptions)
                        {
                            if (aoption.IsSelected)
                                selectedcount += 1.0;
                            else
                                deselectedcount += 1.0;
                        }

                        unansweredcount = Convert.ToDouble(answeredsurveys.Count()) - selectedcount - deselectedcount;

                        int selectedwidth = 0;
                        int deselectedwidth = 0;
                        int unansweredwidth = 0;
                        if (answeredsurveys.Count() > 0)
                        {
                            selectedwidth = Convert.ToInt32((selectedcount / Convert.ToDouble(answeredsurveys.Count())) * 90);
                            deselectedwidth = Convert.ToInt32((deselectedcount / Convert.ToDouble(answeredsurveys.Count())) * 90);
                            unansweredwidth = Convert.ToInt32((unansweredcount / Convert.ToDouble(answeredsurveys.Count())) * 90);
                        }

                        sb.AppendLine("<tr class=\"optionrow\">");
                        sb.AppendLine("<td class=\"gridtext\" style=\"padding-left:15px;\">" + option.SurveyQuestionOptionText + "</td>");
                        sb.AppendLine("<td class=\"gridtext\">" + selectedcount.ToString() + "</td>");
                        sb.AppendLine("<td class=\"gridtext\">");
                        sb.Append(@"<div style=""width:" + selectedwidth.ToString() + @"px;height:8px;background-color:#00CC00;"" /> ");
                        sb.AppendLine("</td>");
                        sb.AppendLine("<td class=\"gridtext\">" + deselectedcount.ToString() + "</td>");
                        sb.AppendLine("<td class=\"gridtext\">");
                        sb.Append(@"<div style=""width:" + deselectedwidth.ToString() + @"px;height:8px;background-color:#0000CC;"" /> ");
                        sb.AppendLine("<td class=\"gridtext\">" + unansweredcount.ToString() + "</td>");
                        sb.AppendLine("<td class=\"gridtext\">");
                        sb.Append(@"<div style=""width:" + unansweredwidth.ToString() + @"px;height:8px;background-color:#CC0000;"" /> ");

                        sb.AppendLine("</td>");
                        sb.AppendLine("</tr>");
                    }
                }

                sb.Append("</table>");

                return sb.ToString();
            }
            catch { return String.Empty; }
        }


        //--------------------------------------------------------------------------------------
        // GET: /Report/ReportPlayerHeartbeat/

        public ActionResult ReportPlayerHeartbeat()
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
                    ViewData["txtIsAdmin"] = "false";

                ViewData["PlayerHeartbeatTable"] = BuildPlayerHeartbeatTable();

                ViewResult result = View();
                result.ViewName = "ReportPlayerHeartbeat";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Report", "ReportPlayerHeartbeat", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        private string BuildPlayerHeartbeatTable()
        {
            try
            {
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);
                StringBuilder sb = new StringBuilder();

                IPlayerRepository playerrep = new EntityPlayerRepository();
                IEnumerable<Player> players = playerrep.GetAllPlayers(accountid);

                if (players == null || players.Count() == 0) return String.Empty;

                IActivityLogRepository alrep = new EntityActivityLogRepository();

                sb.AppendLine("<table style=\"border-spacing:0;border-collapse:collapse;\" class=\"gridtable\">");
                sb.AppendLine("<tr>");
                sb.AppendLine("<td class=\"gridheader\">Player Name</td>");
                sb.AppendLine("<td class=\"gridheader\">Last Heartbeat (UTC)</td>");
                sb.AppendLine("<td class=\"gridheader\"># Hours/Minutes Ago</td>");
                sb.AppendLine("</tr>");

                foreach (Player player in players)
                {
                    sb.AppendLine("<tr class=\"gridrow\">");
                    sb.AppendLine("<td class=\"gridtext\">" + player.PlayerName + "</td>");
                    DateTime dt = alrep.GetLastPlayerHeartbeat(player.PlayerID);
                    if (dt != DateTime.MinValue)
                    {
                        sb.AppendLine("<td class=\"gridtext\">" + dt.ToShortDateString() + " " + dt.ToShortTimeString() + "</td>");
                        TimeSpan ts = DateTime.UtcNow.Subtract(dt);
                        sb.AppendLine("<td class=\"gridtext\">" + String.Format("{0:00}", ts.TotalHours) + ":" + String.Format("{0:00}", ts.Minutes) + "</td>");
                    }
                    else
                    {
                        sb.AppendLine("<td class=\"gridtext\"></td>");
                        sb.AppendLine("<td class=\"gridtext\"></td>");
                    }
                    sb.AppendLine("</tr>");

                }

                sb.Append("</table>");

                return sb.ToString();
            }
            catch { return String.Empty; }
        }


        //------------------------------------------------------------------------------------------
        // Common Methods
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
    }
}
