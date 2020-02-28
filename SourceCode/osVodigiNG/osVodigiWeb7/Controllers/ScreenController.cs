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
using System.Configuration;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class ScreenController : Controller
    {
        IScreenRepository repository;
        string firstfile = String.Empty;
        string selectedfile = String.Empty;

        public ScreenController()
            : this(new EntityScreenRepository())
        { }

        public ScreenController(IScreenRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /Screen/

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
                ScreenPageState pagestate = GetPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.ScreenName = Request.Form["txtScreenName"].ToString().Trim();
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
                ViewData["AccountID"] = pagestate.AccountID;
                ViewData["ScreenName"] = pagestate.ScreenName;
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
                int recordcount = repository.GetScreenRecordCount(pagestate.AccountID, pagestate.ScreenName, pagestate.Description, pagestate.IncludeInactive);

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

                // We need to add the Main Feature Type and Name, and the Screen Content Names
                IEnumerable<Screen> screens = repository.GetScreenPage(pagestate.AccountID, pagestate.ScreenName, pagestate.Description, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount);
                List<ScreenWizardView> screenwizardviews = new List<ScreenWizardView>();
                ISlideShowRepository ssrep = new EntitySlideShowRepository();
                IPlayListRepository plrep = new EntityPlayListRepository();
                ITimelineRepository tlrep = new EntityTimelineRepository();
                IScreenScreenContentXrefRepository sscxrep = new EntityScreenScreenContentXrefRepository();
                IScreenContentRepository screp = new EntityScreenContentRepository();
                IScreenContentTypeRepository sctrep = new EntityScreenContentTypeRepository();
                foreach (Screen screen in screens)
                {
                    ScreenWizardView swv = new ScreenWizardView();
                    swv.ScreenID = screen.ScreenID;
                    swv.AccountID = screen.AccountID;
                    swv.ScreenName = screen.ScreenName;
                    swv.ScreenDescription = screen.ScreenDescription;
                    if (screen.SlideShowID > 0)
                    {
                        swv.MainFeatureType = "Slide Show";
                        swv.MainFeatureName = ssrep.GetSlideShow(screen.SlideShowID).SlideShowName;
                    }
                    else if (screen.PlayListID > 0)
                    {
                        swv.MainFeatureType = "Play List";
                        swv.MainFeatureName = plrep.GetPlayList(screen.PlayListID).PlayListName;
                    }
                    else if (screen.TimelineID > 0)
                    {
                        swv.MainFeatureType = "Media Timeline";
                        swv.MainFeatureName = tlrep.GetTimeline(screen.TimelineID).TimelineName;
                    }
                    if (screen.IsInteractive)
                    {
                        IEnumerable<ScreenScreenContentXref> sscxs = sscxrep.GetScreenScreenContentXrefs(screen.ScreenID);
                        foreach (ScreenScreenContentXref sscx in sscxs)
                        {
                            string contentinfo = String.Empty;

                            ScreenContent sc = screp.GetScreenContent(sscx.ScreenContentID);
                            contentinfo = "'" + sc.ScreenContentName + "'";

                            ScreenContentType sctype = sctrep.GetScreenContentType(sc.ScreenContentTypeID);
                            contentinfo += " (" + sctype.ScreenContentTypeName + ")";

                            if (!String.IsNullOrEmpty(swv.InteractiveContent)) swv.InteractiveContent += ", ";
                            swv.InteractiveContent += contentinfo;
                        }
                    }
                    swv.IsActive = screen.IsActive;

                    screenwizardviews.Add(swv);
                }

                ViewResult result = View(screenwizardviews);
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Screen", "Index", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Screen/Create

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
                ViewData["SlideShowList"] = new SelectList(BuildSlideShowList(), "Value", "Text", "");
                ViewData["PlayListList"] = new SelectList(BuildPlayListList(), "Value", "Text", "");
                ViewData["ScreenContentList"] = new SelectList(BuildScreenContentList(), "Value", "Text", "");
                ViewData["ScreenScreenContentList"] = new SelectList(BuildScreenScreenContentList(0), "Value", "Text", "");
                ViewData["ScreenScreenContent"] = String.Empty;

                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                return View(CreateNewScreen());
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Screen", "Create", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Screen/Create

        [HttpPost]
        public ActionResult Create(Screen screen)
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
                    screen = FillNulls(screen);
                    screen.AccountID = Convert.ToInt32(Session["UserAccountID"]);
                    screen.SlideShowID = Convert.ToInt32(Request.Form["lstSlideShow"]);
                    screen.PlayListID = Convert.ToInt32(Request.Form["lstPlayList"]);
                    string buttonimageguid = Request.Form["lstButtonImage"];

                    IImageRepository imgrep = new EntityImageRepository();
                    Image img = imgrep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), buttonimageguid);
                    if (img != null)
                        screen.ButtonImageID = img.ImageID;
                    else
                        screen.ButtonImageID = 0;

                    string validation = ValidateInput(screen);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["ImageList"] = new SelectList(BuildImageList(Request.Form["lstButtonImage"]), "Value", "Text", Request.Form["lstButtonImage"]);
                        ViewData["ImageUrl"] = selectedfile;
                        ViewData["SlideShowList"] = new SelectList(BuildSlideShowList(), "Value", "Text", Request.Form["lstSlideShow"]);
                        ViewData["PlayListList"] = new SelectList(BuildPlayListList(), "Value", "Text", Request.Form["lstPlayList"]);
                        ViewData["ScreenContentList"] = new SelectList(BuildScreenContentList(), "Value", "Text", "");
                        ViewData["ScreenScreenContentList"] = new SelectList(BuildScreenScreenContentList(screen.ScreenID), "Value", "Text", "");
                        ViewData["ScreenScreenContent"] = Request.Form["txtScreenScreenContent"];

                        int accountid = 0;
                        if (Session["UserAccountID"] != null)
                            accountid = Convert.ToInt32(Session["UserAccountID"]);
                        ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                        return View(screen);
                    }
                    else
                    {
                        repository.CreateScreen(screen);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Screen", "Add",
                            "Added screen '" + screen.ScreenName + "' - ID: " + screen.ScreenID.ToString());

                        // Create a xref for each screen content in the screen
                        IScreenScreenContentXrefRepository sscrep = new EntityScreenScreenContentXrefRepository();
                        string[] ids = Request.Form["txtScreenScreenContent"].ToString().Split('|');
                        int i = 1;
                        foreach (string id in ids)
                        {
                            if (!String.IsNullOrEmpty(id.Trim()))
                            {
                                ScreenScreenContentXref ssc = new ScreenScreenContentXref();
                                ssc.DisplayOrder = i;
                                ssc.ScreenID = screen.ScreenID;
                                ssc.ScreenContentID = Convert.ToInt32(id);
                                sscrep.CreateScreenScreenContentXref(ssc);
                                i += 1;
                            }
                        }

                        return RedirectToAction("Index");
                    }
                }

                return View(screen);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Screen", "Create POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Screen/Edit/5

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

                Screen screen = repository.GetScreen(id);

                ViewData["ValidationMessage"] = String.Empty;
                IImageRepository imgrep = new EntityImageRepository();
                Image img = imgrep.GetImage(screen.ButtonImageID);
                ViewData["ImageList"] = new SelectList(BuildImageList(img.StoredFilename), "Value", "Text", img.StoredFilename);
                ViewData["ImageUrl"] = selectedfile;
                ViewData["SlideShowList"] = new SelectList(BuildSlideShowList(), "Value", "Text", screen.SlideShowID);
                ViewData["PlayListList"] = new SelectList(BuildPlayListList(), "Value", "Text", screen.PlayListID);
                ViewData["ScreenContentList"] = new SelectList(BuildScreenContentList(), "Value", "Text", "");
                ViewData["ScreenScreenContentList"] = new SelectList(BuildScreenScreenContentList(screen.ScreenID), "Value", "Text", "");

                // Get the content ids for the screen
                string ids = String.Empty;
                IScreenScreenContentXrefRepository sscrep = new EntityScreenScreenContentXrefRepository();
                IEnumerable<ScreenScreenContentXref> sscs = sscrep.GetScreenScreenContentXrefs(screen.ScreenID);
                foreach (ScreenScreenContentXref ssc in sscs)
                {
                    ids += "|" + ssc.ScreenContentID.ToString();
                }
                ViewData["ScreenScreenContent"] = ids;

                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                return View(screen);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Screen", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Screen/Edit/5

        [HttpPost]
        public ActionResult Edit(Screen screen)
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
                    screen = FillNulls(screen);
                    screen.SlideShowID = Convert.ToInt32(Request.Form["lstSlideShow"]);
                    screen.PlayListID = Convert.ToInt32(Request.Form["lstPlayList"]);
                    string buttonimageguid = Request.Form["lstButtonImage"];

                    IImageRepository imgrep = new EntityImageRepository();
                    Image img = imgrep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), buttonimageguid);
                    if (img != null)
                        screen.ButtonImageID = img.ImageID;
                    else
                        screen.ButtonImageID = 0;

                    string validation = ValidateInput(screen);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["ImageList"] = new SelectList(BuildImageList(Request.Form["lstButtonImage"]), "Value", "Text", Request.Form["lstButtonImage"]);
                        ViewData["ImageUrl"] = selectedfile;
                        ViewData["SlideShowList"] = new SelectList(BuildSlideShowList(), "Value", "Text", Request.Form["lstSlideShow"]);
                        ViewData["PlayListList"] = new SelectList(BuildPlayListList(), "Value", "Text", Request.Form["lstPlayList"]);

                        int accountid = 0;
                        if (Session["UserAccountID"] != null)
                            accountid = Convert.ToInt32(Session["UserAccountID"]);
                        ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                        return View(screen);
                    }
                    else
                    {
                        // Update the screen
                        repository.UpdateScreen(screen);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Screen", "Edit",
                            "Edited screen '" + screen.ScreenName + "' - ID: " + screen.ScreenID.ToString());

                        IScreenScreenContentXrefRepository xrefrep = new EntityScreenScreenContentXrefRepository();

                        // Delete existing xrefs for the screen
                        xrefrep.DeleteScreenScreenContentXrefs(screen.ScreenID);

                        // Create a xref for each screen content in the screen
                        IScreenScreenContentXrefRepository sscrep = new EntityScreenScreenContentXrefRepository();
                        string[] ids = Request.Form["txtScreenScreenContent"].ToString().Split('|');
                        int i = 1;
                        foreach (string id in ids)
                        {
                            if (!String.IsNullOrEmpty(id.Trim()))
                            {
                                ScreenScreenContentXref ssc = new ScreenScreenContentXref();
                                ssc.DisplayOrder = i;
                                ssc.ScreenID = screen.ScreenID;
                                ssc.ScreenContentID = Convert.ToInt32(id);
                                sscrep.CreateScreenScreenContentXref(ssc);
                                i += 1;
                            }
                        }

                        return RedirectToAction("Index");
                    }
                }

                return View(screen);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Screen", "Edit POST", ex.Message);
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
            sortitem1.Text = "Screen Name";
            sortitem1.Value = "ScreenName";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Description";
            sortitem2.Value = "ScreenDescription";

            SelectListItem sortitem3 = new SelectListItem();
            sortitem3.Text = "Is Interactive";
            sortitem3.Value = "IsInteractive";

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

        private List<SelectListItem> BuildSlideShowList()
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Build the slide show list
            List<SelectListItem> items = new List<SelectListItem>();

            // Create a blank item
            SelectListItem blank = new SelectListItem();
            blank.Text = String.Empty;
            blank.Value = "0";
            items.Add(blank);

            ISlideShowRepository ssrep = new EntitySlideShowRepository();
            IEnumerable<SlideShow> sss = ssrep.GetAllSlideShows(accountid);
            foreach (SlideShow ss in sss)
            {
                SelectListItem item = new SelectListItem();
                item.Text = ss.SlideShowName;
                item.Value = ss.SlideShowID.ToString();
                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildPlayListList()
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Build the play list list
            List<SelectListItem> items = new List<SelectListItem>();

            // Create a blank item
            SelectListItem blank = new SelectListItem();
            blank.Text = String.Empty;
            blank.Value = "0";
            items.Add(blank);

            IPlayListRepository plrep = new EntityPlayListRepository();
            IEnumerable<PlayList> pls = plrep.GetAllPlayLists(accountid);
            foreach (PlayList pl in pls)
            {
                SelectListItem item = new SelectListItem();
                item.Text = pl.PlayListName;
                item.Value = pl.PlayListID.ToString();
                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildScreenContentList()
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Build the screen content list
            List<SelectListItem> items = new List<SelectListItem>();

            IScreenContentTypeRepository sctrep = new EntityScreenContentTypeRepository();
            IScreenContentRepository screp = new EntityScreenContentRepository();
            IEnumerable<ScreenContent> scs = screp.GetAllScreenContents(accountid);
            foreach (ScreenContent sc in scs)
            {
                ScreenContentType sct = sctrep.GetScreenContentType(sc.ScreenContentTypeID);

                SelectListItem item = new SelectListItem();
                item.Text = sc.ScreenContentName + " (" + sct.ScreenContentTypeName + ")";
                item.Value = sc.ScreenContentID.ToString();
                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildScreenScreenContentList(int screenid)
        {
            List<SelectListItem> items = new List<SelectListItem>();

            IScreenContentRepository screp = new EntityScreenContentRepository();
            IScreenContentTypeRepository sctrep = new EntityScreenContentTypeRepository();
            IScreenScreenContentXrefRepository sscrep = new EntityScreenScreenContentXrefRepository();
            IEnumerable<ScreenScreenContentXref> sscs = sscrep.GetScreenScreenContentXrefs(screenid);

            foreach (ScreenScreenContentXref ssc in sscs)
            {
                ScreenContent sc = screp.GetScreenContent(ssc.ScreenContentID);
                ScreenContentType sct = sctrep.GetScreenContentType(sc.ScreenContentTypeID);

                SelectListItem item = new SelectListItem();
                item.Text = sc.ScreenContentName + " (" + sct.ScreenContentTypeName + ")";
                item.Value = sc.ScreenContentID.ToString();
                items.Add(item);
            }

            return items;
        }

        private ScreenPageState GetPageState()
        {
            try
            {
                ScreenPageState pagestate = new ScreenPageState();


                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["ScreenPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.ScreenName = String.Empty;
                    pagestate.Description = String.Empty;
                    pagestate.IncludeInactive = false;
                    pagestate.SortBy = "ScreenName";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    Session["ScreenPageState"] = pagestate;
                }
                else
                {
                    pagestate = (ScreenPageState)Session["ScreenPageState"];
                }
                return pagestate;
            }
            catch { return new ScreenPageState(); }
        }

        private void SavePageState(ScreenPageState pagestate)
        {
            Session["ScreenPageState"] = pagestate;
        }

        private Screen FillNulls(Screen screen)
        {
            if (screen.ScreenDescription == null) screen.ScreenDescription = String.Empty;

            return screen;
        }

        private Screen CreateNewScreen()
        {
            Screen screen = new Screen();
            screen.ScreenID = 0;
            screen.AccountID = 0;
            screen.ScreenName = String.Empty;
            screen.ScreenDescription = String.Empty;
            screen.SlideShowID = 0;
            screen.PlayListID = 0;
            screen.IsInteractive = false;
            screen.ButtonImageID = 0;
            screen.IsActive = true;
            screen.TimelineID = 0;

            return screen;
        }

        private string ValidateInput(Screen screen)
        {
            if (screen.AccountID == 0)
                return "Account ID is not valid.";

            if (String.IsNullOrEmpty(screen.ScreenName))
                return "Screen Name is required.";

            if (screen.SlideShowID == 0 && screen.PlayListID == 0)
                return "You must select a Slide Show or Play List.";

            if (screen.SlideShowID != 0 && screen.PlayListID != 0)
                return "Select a Slide Show or Play List. You cannot select both.";

            if (screen.IsInteractive && screen.ButtonImageID == 0)
                return "You must select a Button Image.";

            return String.Empty;
        }
    }
}
