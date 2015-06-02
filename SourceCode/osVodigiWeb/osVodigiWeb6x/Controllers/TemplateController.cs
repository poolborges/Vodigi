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
    public class TemplateController : Controller
    {
        ITemplateRepository repository;

        public TemplateController()
            : this(new EntityTemplateRepository())
        { }

        public TemplateController(ITemplateRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /Template/

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
                TemplatePageState pagestate = GetPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.TemplateName = Request.Form["txtTemplateName"].ToString().Trim();
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
                ViewData["TemplateName"] = pagestate.TemplateName;
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
                int recordcount = repository.GetTemplateRecordCount(pagestate.AccountID, pagestate.TemplateName, pagestate.OnlyApproved, pagestate.IncludeInactive);

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

                IEnumerable<Template> templates = repository.GetTemplatePage(pagestate.AccountID, pagestate.TemplateName, pagestate.OnlyApproved, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount);
                ITemplateContentRepository templatecontentrepository = new EntityTemplateContentRepository();
                List<TemplateView> templateviews = new List<TemplateView>();
                foreach (Template template in templates)
                {
                    TemplateView templateview = new TemplateView();
                    templateview.TemplateID = template.TemplateID;
                    templateview.AccountID = template.AccountID;
                    templateview.TemplateGUID = template.TemplateGUID;
                    templateview.TemplateName = template.TemplateName;
                    templateview.TemplateDescription = template.TemplateDescription;
                    templateview.Rows = template.Rows;
                    templateview.Columns = template.Columns;
                    templateview.IsApproved = template.IsApproved;
                    templateview.IsActive = template.IsActive;
                    IEnumerable<TemplateContent> templatecontents = templatecontentrepository.GetTemplateContents(template.TemplateID);
                    if (templatecontents != null && templatecontents.Count() > 0)
                        templateview.ContentCount = templatecontents.Count();
                    else
                        templateview.ContentCount = 0;
                    
                    templateviews.Add(templateview);
                }

                ViewResult result = View(templateviews);
                result.ViewName = "Index";

                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Template", "Index", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Template/Create

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
                ViewData["RowList"] = new SelectList(BuildRowList(), "Value", "Text", "1");
                ViewData["ColumnList"] = new SelectList(BuildColumnList(), "Value", "Text", "1");
                ViewData["Row1Height"] = "100";
                ViewData["Row2Height"] = "";
                ViewData["Row3Height"] = "";
                ViewData["Row4Height"] = "";
                ViewData["Row5Height"] = "";
                ViewData["Row6Height"] = "";
                ViewData["Column1Width"] = "100";
                ViewData["Column2Width"] = "";
                ViewData["Column3Width"] = "";
                ViewData["Column4Width"] = "";
                ViewData["Column5Width"] = "";
                ViewData["Column6Width"] = "";

                return View(CreateNewTemplate());
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Template", "Create", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Template/Create

        [HttpPost]
        public ActionResult Create(Template template)
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
                    template = FillNulls(template);
                    template.AccountID = Convert.ToInt32(Session["UserAccountID"]);
                    template.TemplateGUID = Guid.NewGuid();

                    // Set the rows and columns from the drop downs
                    template.Rows = Convert.ToInt32(Request.Form["lstRow"].ToString().Trim());
                    template.Columns = Convert.ToInt32(Request.Form["lstColumn"].ToString().Trim());

                    string validation = ValidateInput(template);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["RowList"] = new SelectList(BuildRowList(), "Value", "Text", Request.Form["lstRow"].ToString().Trim());
                        ViewData["ColumnList"] = new SelectList(BuildColumnList(), "Value", "Text", Request.Form["lstColumn"].ToString().Trim());
                        ViewData["Row1Height"] = Request.Form["txtRow1Height"].ToString().Trim();
                        ViewData["Row2Height"] = Request.Form["txtRow2Height"].ToString().Trim();
                        ViewData["Row3Height"] = Request.Form["txtRow3Height"].ToString().Trim();
                        ViewData["Row4Height"] = Request.Form["txtRow4Height"].ToString().Trim();
                        ViewData["Row5Height"] = Request.Form["txtRow5Height"].ToString().Trim();
                        ViewData["Row6Height"] = Request.Form["txtRow6Height"].ToString().Trim();
                        ViewData["Column1Width"] = Request.Form["txtColumn1Width"].ToString().Trim();
                        ViewData["Column2Width"] = Request.Form["txtColumn2Width"].ToString().Trim();
                        ViewData["Column3Width"] = Request.Form["txtColumn3Width"].ToString().Trim();
                        ViewData["Column4Width"] = Request.Form["txtColumn4Width"].ToString().Trim();
                        ViewData["Column5Width"] = Request.Form["txtColumn5Width"].ToString().Trim();
                        ViewData["Column6Width"] = Request.Form["txtColumn6Width"].ToString().Trim();

                        return View(template);
                    }
                    else
                    {
                        repository.CreateTemplate(template);

                        // Create all the rows and columns
                        ITemplateRowRepository rowrepository = new EntityTemplateRowRepository();
                        for (int r = 1; r <= template.Rows; r += 1)
                        {
                            TemplateRow trow = new TemplateRow();
                            trow.TemplateID = template.TemplateID;
                            trow.RowNumber = r;
                            if (r == 1) trow.RowHeightPercentage = Convert.ToInt32(Request.Form["txtRow1Height"]);
                            else if (r == 2) trow.RowHeightPercentage = Convert.ToInt32(Request.Form["txtRow2Height"]);
                            else if (r == 3) trow.RowHeightPercentage = Convert.ToInt32(Request.Form["txtRow3Height"]);
                            else if (r == 4) trow.RowHeightPercentage = Convert.ToInt32(Request.Form["txtRow4Height"]);
                            else if (r == 5) trow.RowHeightPercentage = Convert.ToInt32(Request.Form["txtRow5Height"]);
                            else if (r == 6) trow.RowHeightPercentage = Convert.ToInt32(Request.Form["txtRow6Height"]);
                            rowrepository.CreateTemplateRow(trow);
                        }

                        ITemplateColumnRepository colrepository = new EntityTemplateColumnRepository();
                        for (int c = 1; c <= template.Columns; c += 1)
                        {
                            TemplateColumn tcol = new TemplateColumn();
                            tcol.TemplateID = template.TemplateID;
                            tcol.ColumnNumber = c;
                            if (c == 1) tcol.ColumnWidthPercentage = Convert.ToInt32(Request.Form["txtColumn1Width"]);
                            else if (c == 2) tcol.ColumnWidthPercentage = Convert.ToInt32(Request.Form["txtColumn2Width"]);
                            else if (c == 3) tcol.ColumnWidthPercentage = Convert.ToInt32(Request.Form["txtColumn3Width"]);
                            else if (c == 4) tcol.ColumnWidthPercentage = Convert.ToInt32(Request.Form["txtColumn4Width"]);
                            else if (c == 5) tcol.ColumnWidthPercentage = Convert.ToInt32(Request.Form["txtColumn5Width"]);
                            else if (c == 6) tcol.ColumnWidthPercentage = Convert.ToInt32(Request.Form["txtColumn6Width"]);
                            colrepository.CreateTemplateColumn(tcol);
                        }

                        CommonMethods.CreateActivityLog((User)Session["User"], "Template", "Add",
                            "Added screen template '" + template.TemplateName + "' - ID: " + template.TemplateID.ToString());

                        return RedirectToAction("Index", "Template");
                    }
                }
                return View(template);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Template", "Create POST", ex.Message);
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
            sortitem1.Text = "Template Name";
            sortitem1.Value = "TemplateName";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Description";
            sortitem2.Value = "TemplateDescription";

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

        private TemplatePageState GetPageState()
        {
            try
            {
                TemplatePageState pagestate = new TemplatePageState();

                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["TemplatePageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.TemplateName = String.Empty;
                    pagestate.OnlyApproved = false;
                    pagestate.IncludeInactive = false;
                    pagestate.SortBy = "TemplateName";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    Session["TemplatePageState"] = pagestate;
                }
                else
                {
                    pagestate = (TemplatePageState)Session["TemplatePageState"];
                }
                return pagestate;
            }
            catch { return new TemplatePageState(); }
        }

        private void SavePageState(TemplatePageState pagestate)
        {
            Session["TemplatePageState"] = pagestate;
        }

        private Template CreateNewTemplate()
        {
            Template template = new Template();
            template.TemplateID = 0;
            template.AccountID = 0;
            template.TemplateGUID = Guid.NewGuid();
            template.TemplateName = String.Empty;
            template.TemplateDescription = String.Empty;
            template.Rows = 1;
            template.Columns = 1;
            template.IsApproved = false;
            template.IsActive = true;

            return template;
        }

        private Template FillNulls(Template template)
        {
            if (template.TemplateDescription == null) template.TemplateDescription = String.Empty;

            return template;
        }

        private List<SelectListItem> BuildRowList()
        {
            // Build the row list
            List<SelectListItem> rowitems = new List<SelectListItem>();

            for (int i = 1; i <= 6; i += 1)
            {
                SelectListItem rowitem = new SelectListItem();
                if (i == 1)
                    rowitem.Text = i.ToString() + " row";
                else
                    rowitem.Text = i.ToString() + " rows";
                rowitem.Value = i.ToString();
                rowitems.Add(rowitem);
            }

            return rowitems;
        }

        private List<SelectListItem> BuildColumnList()
        {
            // Build the column list
            List<SelectListItem> columnitems = new List<SelectListItem>();

            for (int i = 1; i <= 6; i += 1)
            {
                SelectListItem columnitem = new SelectListItem();
                if (i == 1)
                    columnitem.Text = i.ToString() + " column";
                else
                    columnitem.Text = i.ToString() + " columns";
                columnitem.Value = i.ToString();
                columnitems.Add(columnitem);
            }

            return columnitems;
        }

        private string ValidateInput(Template template)
        {
            if (template.AccountID == 0)
                return "Account ID is not valid.";

            if (String.IsNullOrEmpty(template.TemplateName))
                return "Template Name is required.";

            return String.Empty;
        }

    }
}
