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
using osVodigiWeb7x.Models;

namespace osVodigiWeb7x.Controllers
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
                User user = AuthUtils.CheckAuthUser();

                return View();
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Report", "Index", ex);
                
            }
        }

        //----------------------------------------------------------------------------
        // GET: /Report/ReportLoginLog/

        public ActionResult ReportLoginLog()
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                // Initialize or get the page state using session
                ReportLoginLogPageState pagestate = GetReportLoginLogPageState();

                // Get the account id
                int accountid = AuthUtils.GetAccountId();

                // Set and save the page state to the submitted form values if any values are passed
                if (!String.IsNullOrEmpty(Request.Form["lstAscDesc"]))
                {
                    pagestate.AccountID = accountid;
                    pagestate.Username = Request.Form["txtUsername"].ToString().Trim();
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
                throw new Exceptions.AppControllerException("Report", "ReportLoginLog", ex);
                
            }
        }

        private ReportLoginLogPageState GetReportLoginLogPageState()
        {
            try
            {
                ReportLoginLogPageState pagestate = new ReportLoginLogPageState();

                return pagestate;
            }
            catch { return new ReportLoginLogPageState(); }
        }

        private void SavePageState(ReportLoginLogPageState pagestate)
        {
            
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
                User user = AuthUtils.CheckAuthUser();

                // Initialize or get the page state using session
                ReportActivityLogPageState pagestate = GetReportActivityLogPageState();

                // Get the account id
                int accountid = AuthUtils.GetAccountId();

                // Set and save the page state to the submitted form values if any values are passed
                if (!String.IsNullOrEmpty(Request.Form["lstAscDesc"]))
                {
                    pagestate.AccountID = accountid;
                    pagestate.Username = Request.Form["txtUsername"].ToString().Trim();
                    pagestate.EntityType = Request.Form["txtEntityType"].ToString().Trim();
                    pagestate.EntityAction = Request.Form["txtEntityAction"].ToString().Trim();
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
                throw new Exceptions.AppControllerException("Report", "ReportLoginLog", ex);
                
            }
        }

        private ReportActivityLogPageState GetReportActivityLogPageState()
        {
            try
            {
                ReportActivityLogPageState pagestate = new ReportActivityLogPageState();

                return pagestate;
            }
            catch { return new ReportActivityLogPageState(); }
        }

        private void SavePageState(ReportActivityLogPageState pagestate)
        {
            
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
                User user = AuthUtils.CheckAuthUser();

                // Initialize or get the page state using session
                ReportPlayerScreenLogPageState pagestate = GetReportPlayerScreenLogPageState();

                // Get the account id
                int accountid = AuthUtils.GetAccountId();

                // Set and save the page state to the submitted form values if any values are passed
                if (!String.IsNullOrEmpty(Request.Form["lstAscDesc"]))
                {
                    pagestate.AccountID = accountid;
                    pagestate.PlayerName = Request.Form["txtPlayerName"].ToString().Trim();
                    pagestate.ScreenName = Request.Form["txtScreenName"].ToString().Trim();
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
                throw new Exceptions.AppControllerException("Report", "ReportPlayerScreenLog", ex);
                
            }
        }

        private ReportPlayerScreenLogPageState GetReportPlayerScreenLogPageState()
        {
            try
            {
                ReportPlayerScreenLogPageState pagestate = new ReportPlayerScreenLogPageState();
                return pagestate;
            }
            catch { return new ReportPlayerScreenLogPageState(); }
        }

        private void SavePageState(ReportPlayerScreenLogPageState pagestate)
        {
            
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
                User user = AuthUtils.CheckAuthUser();

                // Initialize or get the page state using session
                ReportPlayerScreenContentLogPageState pagestate = GetReportPlayerScreenContentLogPageState();

                // Get the account id
                int accountid = AuthUtils.GetAccountId();

                // Set and save the page state to the submitted form values if any values are passed
                if (!String.IsNullOrEmpty(Request.Form["lstAscDesc"]))
                {
                    pagestate.AccountID = accountid;
                    pagestate.PlayerName = Request.Form["txtPlayerName"].ToString().Trim();
                    pagestate.ScreenName = Request.Form["txtScreenName"].ToString().Trim();
                    pagestate.ContentName = Request.Form["txtContentName"].ToString().Trim();
                    pagestate.ContentType = Request.Form["txtContentType"].ToString().Trim();
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
                throw new Exceptions.AppControllerException("Report", "ReportPlayerScreenContentLog", ex);
                
            }
        }

        private ReportPlayerScreenContentLogPageState GetReportPlayerScreenContentLogPageState()
        {
            try
            {
                ReportPlayerScreenContentLogPageState pagestate = new ReportPlayerScreenContentLogPageState();

                return pagestate;
            }
            catch { return new ReportPlayerScreenContentLogPageState(); }
        }

        private void SavePageState(ReportPlayerScreenContentLogPageState pagestate)
        {
          
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
                User user = AuthUtils.CheckAuthUser();

                // Get the account id
                int accountid = AuthUtils.GetAccountId();

                // Initialize or get the page state using session
                ReportSurveyResultsPageState pagestate = GetReportSurveyResultsPageState();

                // Set and save the page state to the submitted form values if any values are passed
                if (String.IsNullOrEmpty(Request.Form["lstSurvey"]))
                {
                    pagestate.AccountID = accountid;
                    pagestate.SurveyID = Convert.ToInt32(Request.Form["lstSurvey"].ToString().Trim());
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
                throw new Exceptions.AppControllerException("Report", "ReportSurveyResults", ex);
                
            }
        }

        private ReportSurveyResultsPageState GetReportSurveyResultsPageState()
        {
            try
            {
                ReportSurveyResultsPageState pagestate = new ReportSurveyResultsPageState();


                return pagestate;
            }
            catch { return new ReportSurveyResultsPageState(); }
        }

        private void SavePageState(ReportSurveyResultsPageState pagestate)
        {
           
        }

        private List<SelectListItem> BuildReportSurveyResultsSurveyList()
        {
            // Get the account id
            int accountid = AuthUtils.GetAccountId();

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
                User user = AuthUtils.CheckAuthUser();

                ViewData["PlayerHeartbeatTable"] = BuildPlayerHeartbeatTable();

                ViewResult result = View();
                result.ViewName = "ReportPlayerHeartbeat";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Report", "ReportPlayerHeartbeat", ex);
                
            }
        }

        private string BuildPlayerHeartbeatTable()
        {
            try
            {
                int accountid = AuthUtils.GetAccountId();
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
