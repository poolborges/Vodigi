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
using System.Configuration;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class SurveyController : Controller
    {
        ISurveyRepository repository;
        string firstfile = String.Empty;
        string selectedfile = String.Empty;

        public SurveyController()
            : this(new EntitySurveyRepository())
        { }

        public SurveyController(ISurveyRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /Survey/

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

                // Initialize or get the page state using session
                SurveyPageState pagestate = GetPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.SurveyName = Request.Form["txtSurveyName"].ToString().Trim();
                    if (Request.Form["chkOnlyApproved"].ToLower().StartsWith("true"))
                        pagestate.OnlyApproved = true;
                    else
                        pagestate.OnlyApproved = false;
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
                ViewData["AccountID"] = pagestate.AccountID;
                ViewData["SurveyName"] = pagestate.SurveyName;
                ViewData["OnlyApproved"] = pagestate.OnlyApproved;
                ViewData["IncludeInactive"] = pagestate.IncludeInactive;
                ViewData["SortBy"] = pagestate.SortBy;
                ViewData["SortByList"] = new SelectList(BuildSortByList(), "Value", "Text", pagestate.SortBy);
                ViewData["AscDescList"] = new SelectList(BuildAscDescList(), "Value", "Text", pagestate.AscDesc);

                // Determine asc/desc
                bool isdescending = false;
                if (pagestate.AscDesc.ToLower().StartsWith("d"))
                    isdescending = true;

                // Get a Count of all filtered records
                int recordcount = repository.GetSurveyRecordCount(pagestate.AccountID, pagestate.SurveyName, pagestate.OnlyApproved, pagestate.IncludeInactive);

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

                // Set the image folder 
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                ViewResult result = View(repository.GetSurveyPage(pagestate.AccountID, pagestate.SurveyName, pagestate.OnlyApproved, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Survey", "Index", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }


        //
        // GET: /Survey/Create

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
                    ViewData["txtIsAdmin"] = "false";

                ViewData["ValidationMessage"] = String.Empty;
                ViewData["ImageList"] = new SelectList(BuildImageList(""), "Value", "Text", "");
                ViewData["ImageUrl"] = firstfile;
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                return View(CreateNewSurvey());
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Survey", "Create", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }


        //
        // POST: /Survey/Create

        [HttpPost]
        public ActionResult Create(Survey survey)
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

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    survey = FillNulls(survey);
                    survey.AccountID = Convert.ToInt32(Session["UserAccountID"]);

                    IImageRepository imgrep = new EntityImageRepository();
                    Image img = imgrep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), Request.Form["lstImage"]);
                    if (img != null)
                        survey.SurveyImageID = img.ImageID;
                    else
                        survey.SurveyImageID = 0;

                    string validation = ValidateInput(survey);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["ImageList"] = new SelectList(BuildImageList(Request.Form["lstImage"]), "Value", "Text", Request.Form["lstImage"]);
                        ViewData["ImageUrl"] = selectedfile;
                        ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";
                        return View(survey);
                    }
                    else
                    {
                        repository.CreateSurvey(survey);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Survey", "Add",
                            "Added survey '" + survey.SurveyName + "' - ID: " + survey.SurveyID.ToString());

                        return RedirectToAction("Edit", "Survey", new { id = survey.SurveyID });
                    }
                }
                return View(survey);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Survey", "Create POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Survey/Edit/5

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
                    ViewData["txtIsAdmin"] = "false";

                Survey survey = repository.GetSurvey(id);
                ViewData["ValidationMessage"] = String.Empty;

                IImageRepository imgrep = new EntityImageRepository();
                Image img = imgrep.GetImage(survey.SurveyImageID);
                ViewData["ImageList"] = new SelectList(BuildImageList(img.StoredFilename), "Value", "Text", img.StoredFilename);
                ViewData["ImageUrl"] = selectedfile;
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                ViewData["SurveyTable"] = BuildSurveyTable(survey);

                return View(survey);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Survey", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Survey/Edit/5

        [HttpPost]
        public ActionResult Edit(Survey survey)
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

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    survey = FillNulls(survey);

                    IImageRepository imgrep = new EntityImageRepository();
                    Image img = imgrep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), Request.Form["lstImage"]);
                    if (img != null)
                        survey.SurveyImageID = img.ImageID;
                    else
                        survey.SurveyImageID = 0;

                    string validation = ValidateInput(survey);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["ImageList"] = new SelectList(BuildImageList(Request.Form["lstImage"]), "Value", "Text", Request.Form["lstImage"]);
                        ViewData["ImageUrl"] = selectedfile;
                        ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                        ViewData["SurveyTable"] = BuildSurveyTable(survey);
                        return View(survey);
                    }

                    repository.UpdateSurvey(survey);

                    CommonMethods.CreateActivityLog((User)Session["User"], "Survey", "Edit",
                                                    "Edited survey '" + survey.SurveyName + "' - ID: " + survey.SurveyID.ToString());

                    return RedirectToAction("Index");
                }

                return View(survey);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Survey", "Edit POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Survey/Approve/5

        public ActionResult Approve(int id)
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

                Survey survey = repository.GetSurvey(id);
                ViewData["ValidationMessage"] = String.Empty;

                IImageRepository imgrep = new EntityImageRepository();
                Image img = imgrep.GetImage(survey.SurveyImageID);
                ViewData["ImageList"] = new SelectList(BuildImageList(img.StoredFilename), "Value", "Text", img.StoredFilename);
                ViewData["ImageUrl"] = selectedfile;
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                ViewData["SurveyTable"] = BuildSurveyTableNoLinks(survey);

                return View(survey);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Survey", "Approve", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Survey/Approve/5

        [HttpPost]
        public ActionResult Approve(Survey survey)
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

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    survey = FillNulls(survey);

                    survey.IsApproved = true;
                    repository.UpdateSurvey(survey);

                    CommonMethods.CreateActivityLog((User)Session["User"], "Survey", "Approve",
                                                    "Approved survey '" + survey.SurveyName + "' - ID: " + survey.SurveyID.ToString());

                    return RedirectToAction("Index");
                }

                return View(survey);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Survey", "Approve POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Survey/View/5

        public ActionResult View(int id)
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

                Survey survey = repository.GetSurvey(id);
                ViewData["ValidationMessage"] = String.Empty;

                IImageRepository imgrep = new EntityImageRepository();
                Image img = imgrep.GetImage(survey.SurveyImageID);
                ViewData["ImageList"] = new SelectList(BuildImageList(img.StoredFilename), "Value", "Text", img.StoredFilename);
                ViewData["ImageUrl"] = selectedfile;
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                ViewData["SurveyTable"] = BuildSurveyTableNoLinks(survey);

                return View(survey);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Survey", "View", ex.Message);
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
            sortitem1.Text = "Survey Name";
            sortitem1.Value = "SurveyName";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Description";
            sortitem2.Value = "SurveyDescription";

            SelectListItem sortitem3 = new SelectListItem();
            sortitem3.Text = "Is Approved";
            sortitem3.Value = "IsApproved";

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

        private SurveyPageState GetPageState()
        {
            try
            {
                SurveyPageState pagestate = new SurveyPageState();

                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["SurveyPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.SurveyName = String.Empty;
                    pagestate.OnlyApproved = false;
                    pagestate.IncludeInactive = false;
                    pagestate.SortBy = "SurveyName";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    Session["SurveyPageState"] = pagestate;
                }
                else
                {
                    pagestate = (SurveyPageState)Session["SurveyPageState"];
                }
                return pagestate;
            }
            catch { return new SurveyPageState(); }
        }

        private void SavePageState(SurveyPageState pagestate)
        {
            Session["SurveyPageState"] = pagestate;
        }

        private Survey CreateNewSurvey()
        {
            Survey survey = new Survey();
            survey.SurveyID = 0;
            survey.AccountID = 0;
            survey.SurveyName = String.Empty;
            survey.SurveyDescription = String.Empty;
            survey.IsApproved = false;
            survey.IsActive = true;

            return survey;
        }

        private Survey FillNulls(Survey survey)
        {
            if (survey.SurveyDescription == null) survey.SurveyDescription = String.Empty;

            return survey;
        }

        private string ValidateInput(Survey survey)
        {
            if (survey.AccountID == 0)
                return "Account ID is not valid.";

            if (String.IsNullOrEmpty(survey.SurveyName))
                return "Survey Name is required.";

            if (survey.SurveyImageID == 0)
                return "You must select a survey image.";

            return String.Empty;
        }

        private string BuildSurveyTable(Survey survey)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                string root = Request.Url.OriginalString.Substring(0, Request.Url.OriginalString.ToLower().LastIndexOf("/survey/"));
                if (!root.EndsWith("/")) root += "/";

                sb.AppendLine("<table style=\"border-spacing:0;border-collapse:collapse;\" class=\"surveytable\">");
                sb.AppendLine("<tr class=\"surveyrow\">");
                sb.AppendLine("<td class=\"gridtext\">" + survey.SurveyName + "</td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:110px;\"><span class=\"surveyquestionlink\" onclick=\"window.location='" + root + "SurveyQuestion/Create/" + survey.SurveyID.ToString() + "'\">Add Question</span></td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:25px;\"></td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:45px;\"></td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:35px;\"></td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:55px;\"></td>");
                sb.AppendLine("</tr>");

                // Loop through each question and question option
                ISurveyQuestionRepository qrep = new EntitySurveyQuestionRepository();
                ISurveyQuestionOptionRepository orep = new EntitySurveyQuestionOptionRepository();
                List<SurveyQuestion> questions = qrep.GetSurveyQuestions(survey.SurveyID).ToList();

                foreach (SurveyQuestion question in questions)
                {
                    sb.AppendLine("<tr class=\"questionrow\">");
                    string selectionmode = " (Single Select)";
                    if (question.AllowMultiSelect) selectionmode = " (Multi Select)";
                    sb.AppendLine("<td class=\"gridtext\">" + question.SurveyQuestionText + selectionmode + "</td>");
                    sb.AppendLine("<td class=\"gridtext\"><span class=\"surveyquestionoptionlink\" onclick=\"window.location='" + root + "SurveyQuestionOption/Create/" + question.SurveyQuestionID.ToString() + "'\">Add Option</span></td>");
                    sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"><span class=\"surveyquestionoptionlink\" onclick=\"window.location='" + root + "SurveyQuestion/MoveUp/" + question.SurveyQuestionID.ToString() + "'\">Up</span></td>");
                    sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"><span class=\"surveyquestionoptionlink\" onclick=\"window.location='" + root + "SurveyQuestion/MoveDown/" + question.SurveyQuestionID.ToString() + "'\">Down</span></td>");
                    sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"><span class=\"surveyquestionoptionlink\" onclick=\"window.location='" + root + "SurveyQuestion/Edit/" + question.SurveyQuestionID.ToString() + "'\">Edit</span></td>");
                    sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"><span class=\"surveyquestionoptionlink\" onclick=\"window.location='" + root + "SurveyQuestion/Delete/" + question.SurveyQuestionID.ToString() + "'\">Delete</span></td>");
                    sb.AppendLine("</tr>");

                    // Loop through each question option
                    List<SurveyQuestionOption> options = orep.GetSurveyQuestionOptions(question.SurveyQuestionID).ToList();
                    foreach (SurveyQuestionOption option in options)
                    {
                        sb.AppendLine("<tr class=\"optionrow\">");
                        sb.AppendLine("<td class=\"gridtext\" style=\"padding-left:15px;\">" + option.SurveyQuestionOptionText + "</td>");
                        sb.AppendLine("<td class=\"gridtext\"></td>");
                        sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"><span class=\"surveyquestionoptionlink\" onclick=\"window.location='" + root + "SurveyQuestionOption/MoveUp/" + option.SurveyQuestionOptionID.ToString() + "'\">Up</span></td>");
                        sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"><span class=\"surveyquestionoptionlink\" onclick=\"window.location='" + root + "SurveyQuestionOption/MoveDown/" + option.SurveyQuestionOptionID.ToString() + "'\">Down</span></td>");
                        sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"><span class=\"surveyquestionoptionlink\" onclick=\"window.location='" + root + "SurveyQuestionOption/Edit/" + option.SurveyQuestionOptionID.ToString() + "'\">Edit</span></td>");
                        sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"><span class=\"surveyquestionoptionlink\" onclick=\"window.location='" + root + "SurveyQuestionOption/Delete/" + option.SurveyQuestionOptionID.ToString() + "'\">Delete</span></td>");
                        sb.AppendLine("</tr>");
                    }
                }

                sb.Append("</table>");

                return sb.ToString();
            }
            catch { return String.Empty; }
        }

        private string BuildSurveyTableNoLinks(Survey survey)
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                string root = Request.Url.OriginalString.Substring(0, Request.Url.OriginalString.ToLower().LastIndexOf("/survey/"));
                if (!root.EndsWith("/")) root += "/";

                sb.AppendLine("<table style=\"border-spacing:0;border-collapse:collapse;\" class=\"surveytable\">");
                sb.AppendLine("<tr class=\"surveyrow\">");
                sb.AppendLine("<td class=\"gridtext\">" + survey.SurveyName + "</td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:110px;\"></td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:25px;\"></td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:45px;\"></td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:35px;\"></td>");
                sb.AppendLine("<td class=\"gridtext\" style=\"width:55px;\"></td>");
                sb.AppendLine("</tr>");

                // Loop through each question and question option
                ISurveyQuestionRepository qrep = new EntitySurveyQuestionRepository();
                ISurveyQuestionOptionRepository orep = new EntitySurveyQuestionOptionRepository();
                List<SurveyQuestion> questions = qrep.GetSurveyQuestions(survey.SurveyID).ToList();

                foreach (SurveyQuestion question in questions)
                {
                    sb.AppendLine("<tr class=\"questionrow\">");
                    string selectionmode = " (Single Select)";
                    if (question.AllowMultiSelect) selectionmode = " (Multi Select)";
                    sb.AppendLine("<td class=\"gridtext\">" + question.SurveyQuestionText + selectionmode + "</td>");
                    sb.AppendLine("<td class=\"gridtext\"></td>");
                    sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"></td>");
                    sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"></td>");
                    sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"></td>");
                    sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"></td>");
                    sb.AppendLine("</tr>");

                    // Loop through each question option
                    List<SurveyQuestionOption> options = orep.GetSurveyQuestionOptions(question.SurveyQuestionID).ToList();
                    foreach (SurveyQuestionOption option in options)
                    {
                        sb.AppendLine("<tr class=\"optionrow\">");
                        sb.AppendLine("<td class=\"gridtext\" style=\"padding-left:15px;\">" + option.SurveyQuestionOptionText + "</td>");
                        sb.AppendLine("<td class=\"gridtext\"></td>");
                        sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"></td>");
                        sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"></td>");
                        sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"></td>");
                        sb.AppendLine("<td class=\"gridtext\" style=\"text-align:center;\"></td>");
                        sb.AppendLine("</tr>");
                    }
                }

                sb.Append("</table>");

                return sb.ToString();
            }
            catch { return String.Empty; }
        }

        private List<SelectListItem> BuildImageList(string currentfile)
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Get the active images
            IImageRepository imgrep = new EntityImageRepository();
            IEnumerable<Image> imgs = imgrep.GetAllImages(accountid);

            string imagefolder = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

            List<SelectListItem> items = new List<SelectListItem>();
            bool first = true;
            foreach (Image img in imgs)
            {
                if (first)
                {
                    first = false;
                    firstfile = imagefolder + img.StoredFilename;
                }

                SelectListItem item = new SelectListItem();
                item.Text = img.ImageName;
                item.Value = img.StoredFilename;
                if (item.Value == currentfile)
                    selectedfile = imagefolder + img.StoredFilename;

                items.Add(item);
            }

            return items;
        }
    }
}
