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
    public class ScreenContentController : Controller
    {
        IScreenContentRepository repository;
        string firstfile = String.Empty;
        string selectedfile = String.Empty;

        public ScreenContentController()
            : this(new EntityScreenContentRepository())
        { }

        public ScreenContentController(IScreenContentRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /ScreenContent/

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
                ScreenContentPageState pagestate = GetPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.ScreenContentName = Request.Form["txtScreenContentName"].ToString().Trim();
                    pagestate.ScreenContentTypeID = Convert.ToInt32(Request.Form["lstScreenContentTypeList"]);
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
                ViewData["ScreenContentName"] = pagestate.ScreenContentName;
                ViewData["ScreenContentTypeID"] = pagestate.ScreenContentTypeID;
                ViewData["IncludeInactive"] = pagestate.IncludeInactive;
                ViewData["SortBy"] = pagestate.SortBy;
                ViewData["SortByList"] = new SelectList(BuildSortByList(), "Value", "Text", pagestate.SortBy);
                ViewData["AscDescList"] = new SelectList(BuildAscDescList(), "Value", "Text", pagestate.AscDesc);
                ViewData["ScreenContentTypeList"] = new SelectList(BuildScreenContentTypeList(true), "Value", "Text", pagestate.ScreenContentTypeID);

                // Determine asc/desc
                bool isdescending = false;
                if (pagestate.AscDesc.ToLower().StartsWith("d"))
                    isdescending = true;

                // Get a Count of all filtered records
                int recordcount = repository.GetScreenContentRecordCount(pagestate.AccountID, pagestate.ScreenContentName, pagestate.ScreenContentTypeID, pagestate.IncludeInactive);

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
                ViewData["ImageFolder"] = @"~/Media/" + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                // Need to return the stored filename and content type name
                IEnumerable<ScreenContent> screencontents = repository.GetScreenContentPage(pagestate.AccountID, pagestate.ScreenContentName, pagestate.ScreenContentTypeID, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount);
                IScreenContentTypeRepository sctrep = new EntityScreenContentTypeRepository();
                IImageRepository imgrep = new EntityImageRepository();
                List<ScreenContentView> contentviews = new List<ScreenContentView>();
                foreach (ScreenContent screencontent in screencontents)
                {
                    ScreenContentView contentview = new ScreenContentView();
                    contentview.ScreenContentID = screencontent.ScreenContentID;
                    contentview.AccountID = screencontent.AccountID;
                    contentview.ScreenContentTypeID = screencontent.ScreenContentTypeID;
                    ScreenContentType sctype = sctrep.GetScreenContentType(screencontent.ScreenContentTypeID);
                    contentview.ScreenContentTypeName = sctype.ScreenContentTypeName;
                    contentview.ScreenContentName = screencontent.ScreenContentName;
                    contentview.ScreenContentTitle = screencontent.ScreenContentTitle;
                    contentview.ThumbnailImageID = screencontent.ThumbnailImageID;
                    Image img = imgrep.GetImage(screencontent.ThumbnailImageID);
                    contentview.StoredFilename = img.StoredFilename;
                    contentview.CustomField1 = screencontent.CustomField1;
                    contentview.CustomField2 = screencontent.CustomField2;
                    contentview.CustomField3 = screencontent.CustomField3;
                    contentview.CustomField4 = screencontent.CustomField4;
                    contentview.IsActive = screencontent.IsActive;
                    contentviews.Add(contentview);
                }

                ViewResult result = View(contentviews);
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenContent", "Index", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /ScreenContent/Create

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
                ViewData["ImageList"] = new SelectList(BuildThumbnailImageList(""), "Value", "Text", "");
                ViewData["ImageUrl"] = firstfile;
                ViewData["ScreenContentTypeList"] = new SelectList(BuildScreenContentTypeList(false), "Value", "Text", "");

                ViewData["ScreenContentImages"] = new SelectList(BuildImageList(0), "Value", "Text", "");
                ViewData["ScreenContentSlideShows"] = new SelectList(BuildSlideShowList(0), "Value", "Text", "");
                ViewData["ScreenContentVideos"] = new SelectList(BuildVideoList(0), "Value", "Text", "");
                ViewData["ScreenContentPlayLists"] = new SelectList(BuildPlayListList(0), "Value", "Text", "");
                ViewData["ScreenContentSurveys"] = new SelectList(BuildSurveyList(0), "Value", "Text", "");
                ViewData["ScreenContentTimelines"] = new SelectList(BuildTimelineList(0), "Value", "Text", "");

                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                return View(CreateNewScreenContent());
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenContent", "Create", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /ScreenContent/Create

        [HttpPost]
        public ActionResult Create(ScreenContent screencontent)
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
                    screencontent = FillNulls(screencontent);
                    screencontent.AccountID = Convert.ToInt32(Session["UserAccountID"]);
                    screencontent.ScreenContentTypeID = Convert.ToInt32(Request.Form["lstScreenContentTypeList"]);

                    IImageRepository imgrep = new EntityImageRepository();
                    Image img = imgrep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), Request.Form["lstImage"]);
                    if (img != null)
                        screencontent.ThumbnailImageID = img.ImageID;
                    else
                        screencontent.ThumbnailImageID = 0;

                    string validation = ValidateInput(screencontent);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["ImageList"] = new SelectList(BuildThumbnailImageList(Request.Form["lstImage"]), "Value", "Text", Request.Form["lstImage"]);
                        ViewData["ImageUrl"] = selectedfile;
                        ViewData["ScreenContentTypeList"] = new SelectList(BuildScreenContentTypeList(false), "Value", "Text", Request.Form["lstScreenContentTypeList"]);

                        ViewData["ScreenContentImages"] = new SelectList(BuildImageList(0), "Value", "Text", Request.Form["lstScreenContentImages"]);
                        ViewData["ScreenContentSlideShows"] = new SelectList(BuildSlideShowList(0), "Value", "Text", Request.Form["lstScreenContentSlideShows"]);
                        ViewData["ScreenContentVideos"] = new SelectList(BuildVideoList(0), "Value", "Text", Request.Form["lstScreenContentVideos"]);
                        ViewData["ScreenContentPlayLists"] = new SelectList(BuildPlayListList(0), "Value", "Text", Request.Form["lstScreenContentPlayLists"]);
                        ViewData["ScreenContentSurveys"] = new SelectList(BuildSurveyList(0), "Value", "Text", "");
                        ViewData["ScreenContentTimelines"] = new SelectList(BuildTimelineList(0), "Value", "Text", Request.Form["lstScreenContentTimelines"]);

                        ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                        return View(screencontent);
                    }
                    else
                    {
                        int screencontenttypeid = Convert.ToInt32(Request.Form["lstScreenContentTypeList"]);
                        if (screencontenttypeid == 1000000)
                            screencontent.CustomField1 = Request.Form["lstScreenContentImages"];
                        else if (screencontenttypeid == 1000001)
                            screencontent.CustomField1 = Request.Form["lstScreenContentSlideShows"];
                        else if (screencontenttypeid == 1000002)
                            screencontent.CustomField1 = Request.Form["lstScreenContentVideos"];
                        else if (screencontenttypeid == 1000003)
                            screencontent.CustomField1 = Request.Form["lstScreenContentPlayLists"];
                        else if (screencontenttypeid == 1000007)
                            screencontent.CustomField1 = Request.Form["lstScreenContentSurveys"];
                        else if (screencontenttypeid == 1000008)
                            screencontent.CustomField1 = Request.Form["lstScreenContentTimelines"];

                        repository.CreateScreenContent(screencontent);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Screen Content", "Add",
                            "Added screen content '" + screencontent.ScreenContentName + "' - ID: " + screencontent.ScreenContentID.ToString());

                        return RedirectToAction("Index");
                    }
                }

                return View(screencontent);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenContent", "Create POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /ScreenContent/Edit/5

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

                ScreenContent screencontent = repository.GetScreenContent(id);
                ViewData["ValidationMessage"] = String.Empty;

                IImageRepository imgrep = new EntityImageRepository();
                Image img = imgrep.GetImage(screencontent.ThumbnailImageID);
                ViewData["ImageList"] = new SelectList(BuildThumbnailImageList(img.StoredFilename), "Value", "Text", img.StoredFilename);
                ViewData["ImageUrl"] = selectedfile;
                ViewData["ScreenContentTypeList"] = new SelectList(BuildScreenContentTypeList(false), "Value", "Text", screencontent.ScreenContentTypeID);

                int currentimageid = 0;
                int currentslideshowid = 0;
                int currentvideoid = 0;
                int currentplaylistid = 0;
                int currentsurveyid = 0;
                int currenttimelineid = 0;

                if (screencontent.ScreenContentTypeID == 1000000)
                    currentimageid = Convert.ToInt32(screencontent.CustomField1);
                else if (screencontent.ScreenContentTypeID == 1000001)
                    currentslideshowid = Convert.ToInt32(screencontent.CustomField1);
                else if (screencontent.ScreenContentTypeID == 1000002)
                    currentvideoid = Convert.ToInt32(screencontent.CustomField1);
                else if (screencontent.ScreenContentTypeID == 1000003)
                    currentplaylistid = Convert.ToInt32(screencontent.CustomField1);
                else if (screencontent.ScreenContentTypeID == 1000007)
                    currentsurveyid = Convert.ToInt32(screencontent.CustomField1);
                else if (screencontent.ScreenContentTypeID == 1000008)
                    currenttimelineid = Convert.ToInt32(screencontent.CustomField1);

                ViewData["ScreenContentImages"] = new SelectList(BuildImageList(currentimageid), "Value", "Text", "");
                ViewData["ScreenContentSlideShows"] = new SelectList(BuildSlideShowList(currentslideshowid), "Value", "Text", "");
                ViewData["ScreenContentVideos"] = new SelectList(BuildVideoList(currentvideoid), "Value", "Text", "");
                ViewData["ScreenContentPlayLists"] = new SelectList(BuildPlayListList(currentplaylistid), "Value", "Text", "");
                ViewData["ScreenContentSurveys"] = new SelectList(BuildSurveyList(currentsurveyid), "Value", "Text", "");
                ViewData["ScreenContentTimelines"] = new SelectList(BuildTimelineList(currenttimelineid), "Value", "Text", "");

                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                return View(screencontent);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenContent", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /ScreenContent/Edit/5

        [HttpPost]
        public ActionResult Edit(ScreenContent screencontent)
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
                    screencontent = FillNulls(screencontent);

                    screencontent.AccountID = Convert.ToInt32(Session["UserAccountID"]);
                    screencontent.ScreenContentTypeID = Convert.ToInt32(Request.Form["lstScreenContentTypeList"]);

                    IImageRepository imgrep = new EntityImageRepository();
                    Image img = imgrep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), Request.Form["lstImage"]);
                    if (img != null)
                        screencontent.ThumbnailImageID = img.ImageID;
                    else
                        screencontent.ThumbnailImageID = 0;

                    string validation = ValidateInput(screencontent);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["ImageList"] = new SelectList(BuildThumbnailImageList(Request.Form["lstImage"]), "Value", "Text", Request.Form["lstImage"]);
                        ViewData["ImageUrl"] = selectedfile;
                        ViewData["ScreenContentTypeList"] = new SelectList(BuildScreenContentTypeList(false), "Value", "Text", Request.Form["lstScreenContentTypeList"]);

                        ViewData["ScreenContentImages"] = new SelectList(BuildImageList(0), "Value", "Text", Request.Form["lstScreenContentImages"]);
                        ViewData["ScreenContentSlideShows"] = new SelectList(BuildSlideShowList(0), "Value", "Text", Request.Form["lstScreenContentSlideShows"]);
                        ViewData["ScreenContentVideos"] = new SelectList(BuildVideoList(0), "Value", "Text", Request.Form["lstScreenContentVideos"]);
                        ViewData["ScreenContentPlayLists"] = new SelectList(BuildPlayListList(0), "Value", "Text", Request.Form["lstScreenContentPlayLists"]);
                        ViewData["ScreenContentSurveys"] = new SelectList(BuildSurveyList(0), "Value", "Text", "");
                        ViewData["ScreenContentTimelines"] = new SelectList(BuildTimelineList(0), "Value", "Text", Request.Form["lstScreenContentTimelines"]);

                        ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                        return View(screencontent);
                    }
                    else
                    {
                        int screencontenttypeid = Convert.ToInt32(Request.Form["lstScreenContentTypeList"]);
                        if (screencontenttypeid == 1000000)
                            screencontent.CustomField1 = Request.Form["lstScreenContentImages"];
                        else if (screencontenttypeid == 1000001)
                            screencontent.CustomField1 = Request.Form["lstScreenContentSlideShows"];
                        else if (screencontenttypeid == 1000002)
                            screencontent.CustomField1 = Request.Form["lstScreenContentVideos"];
                        else if (screencontenttypeid == 1000003)
                            screencontent.CustomField1 = Request.Form["lstScreenContentPlayLists"];
                        else if (screencontenttypeid == 1000007)
                            screencontent.CustomField1 = Request.Form["lstScreenContentSurveys"];
                        else if (screencontenttypeid == 1000008)
                            screencontent.CustomField1 = Request.Form["lstScreenContentTimelines"];

                        repository.UpdateScreenContent(screencontent);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Screen Content", "Edit",
                            "Edited screen content '" + screencontent.ScreenContentName + "' - ID: " + screencontent.ScreenContentID.ToString());

                        return RedirectToAction("Index");
                    }
                }

                return View(screencontent);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenContent", "Edit POST", ex.Message);
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
            sortitem1.Text = "Screen Content Name";
            sortitem1.Value = "ScreenContentName";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Screen Content Title";
            sortitem2.Value = "ScreenContentTitle";

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

        private List<SelectListItem> BuildScreenContentTypeList(bool includealloption)
        {
            // Build the screen content type list
            List<SelectListItem> items = new List<SelectListItem>();

            if (includealloption)
            {
                SelectListItem all = new SelectListItem();
                all.Text = "All Types";
                all.Value = "0";
                items.Add(all);
            }

            IScreenContentTypeRepository sctrep = new EntityScreenContentTypeRepository();
            IEnumerable<ScreenContentType> scts = sctrep.GetAllScreenContentTypes();
            foreach (ScreenContentType sct in scts)
            {
                SelectListItem item = new SelectListItem();
                item.Text = sct.ScreenContentTypeName;
                item.Value = sct.ScreenContentTypeID.ToString();
                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildThumbnailImageList(string currentfile)
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

        private List<SelectListItem> BuildImageList(int currentimageid)
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Get the active images
            IImageRepository imgrep = new EntityImageRepository();
            IEnumerable<Image> imgs = imgrep.GetAllImages(accountid);

            List<SelectListItem> items = new List<SelectListItem>();
            if (currentimageid > 0)
            {
                Image currentimage = imgrep.GetImage(currentimageid);

                SelectListItem item = new SelectListItem();
                item.Text = currentimage.ImageName;
                item.Value = currentimage.ImageID.ToString();

                items.Add(item);
            }
            foreach (Image img in imgs)
            {
                SelectListItem item = new SelectListItem();
                item.Text = img.ImageName;
                item.Value = img.ImageID.ToString();

                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildVideoList(int currentvideoid)
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Get the active videos
            IVideoRepository vidrep = new EntityVideoRepository();
            IEnumerable<Video> vids = vidrep.GetAllVideos(accountid);

            List<SelectListItem> items = new List<SelectListItem>();
            if (currentvideoid > 0)
            {
                Video currentvideo = vidrep.GetVideo(currentvideoid);

                SelectListItem item = new SelectListItem();
                item.Text = currentvideo.VideoName;
                item.Value = currentvideo.VideoID.ToString();

                items.Add(item);
            }
            foreach (Video vid in vids)
            {
                SelectListItem item = new SelectListItem();
                item.Text = vid.VideoName;
                item.Value = vid.VideoID.ToString();

                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildSlideShowList(int currentslideshowid)
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Get the active slide shows
            ISlideShowRepository ssrep = new EntitySlideShowRepository();
            IEnumerable<SlideShow> sss = ssrep.GetAllSlideShows(accountid);

            List<SelectListItem> items = new List<SelectListItem>();
            if (currentslideshowid > 0)
            {
                SlideShow currentslideshow = ssrep.GetSlideShow(currentslideshowid);

                SelectListItem item = new SelectListItem();
                item.Text = currentslideshow.SlideShowName;
                item.Value = currentslideshow.SlideShowID.ToString();

                items.Add(item);
            }
            foreach (SlideShow ss in sss)
            {
                SelectListItem item = new SelectListItem();
                item.Text = ss.SlideShowName;
                item.Value = ss.SlideShowID.ToString();

                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildPlayListList(int currentplaylistid)
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Get the active play lists
            IPlayListRepository plrep = new EntityPlayListRepository();
            IEnumerable<PlayList> pls = plrep.GetAllPlayLists(accountid);

            List<SelectListItem> items = new List<SelectListItem>();
            if (currentplaylistid > 0)
            {
                PlayList currentplaylist = plrep.GetPlayList(currentplaylistid);

                SelectListItem item = new SelectListItem();
                item.Text = currentplaylist.PlayListName;
                item.Value = currentplaylist.PlayListID.ToString();

                items.Add(item);
            }
            foreach (PlayList pl in pls)
            {
                SelectListItem item = new SelectListItem();
                item.Text = pl.PlayListName;
                item.Value = pl.PlayListID.ToString();

                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildSurveyList(int currentsurveyid)
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Get the approved surveys
            ISurveyRepository srep = new EntitySurveyRepository();
            IEnumerable<Survey> ss = srep.GetApprovedSurveys(accountid);

            List<SelectListItem> items = new List<SelectListItem>();
            if (currentsurveyid > 0)
            {
                Survey currentsurvey = srep.GetSurvey(currentsurveyid);

                SelectListItem item = new SelectListItem();
                item.Text = currentsurvey.SurveyName;
                item.Value = currentsurvey.SurveyID.ToString();

                items.Add(item);
            }
            foreach (Survey s in ss)
            {
                SelectListItem item = new SelectListItem();
                item.Text = s.SurveyName;
                item.Value = s.SurveyID.ToString();

                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildTimelineList(int currenttimelineid)
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Get the active timelines
            ITimelineRepository tlrep = new EntityTimelineRepository();
            IEnumerable<Timeline> tls = tlrep.GetAllTimelines(accountid);

            List<SelectListItem> items = new List<SelectListItem>();
            if (currenttimelineid > 0)
            {
                Timeline currenttimeline = tlrep.GetTimeline(currenttimelineid);

                SelectListItem item = new SelectListItem();
                item.Text = currenttimeline.TimelineName;
                item.Value = currenttimeline.TimelineID.ToString();

                items.Add(item);
            }
            foreach (Timeline tl in tls)
            {
                SelectListItem item = new SelectListItem();
                item.Text = tl.TimelineName;
                item.Value = tl.TimelineID.ToString();

                items.Add(item);
            }

            return items;
        }

        private ScreenContentPageState GetPageState()
        {
            try
            {
                ScreenContentPageState pagestate = new ScreenContentPageState();


                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["ScreenContentPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.ScreenContentName = String.Empty;
                    pagestate.ScreenContentTypeID = 0;
                    pagestate.IncludeInactive = false;
                    pagestate.SortBy = "ScreenContentName";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    Session["ScreenContentPageState"] = pagestate;
                }
                else
                {
                    pagestate = (ScreenContentPageState)Session["ScreenContentPageState"];
                }
                return pagestate;
            }
            catch { return new ScreenContentPageState(); }
        }

        private void SavePageState(ScreenContentPageState pagestate)
        {
            Session["ScreenContentPageState"] = pagestate;
        }

        private ScreenContent FillNulls(ScreenContent screencontent)
        {
            if (screencontent.CustomField1 == null) screencontent.CustomField1 = String.Empty;
            if (screencontent.CustomField2 == null) screencontent.CustomField2 = String.Empty;
            if (screencontent.CustomField3 == null) screencontent.CustomField3 = String.Empty;
            if (screencontent.CustomField4 == null) screencontent.CustomField4 = String.Empty;

            return screencontent;
        }

        private ScreenContent CreateNewScreenContent()
        {
            ScreenContent sc = new ScreenContent();
            sc.ScreenContentID = 0;
            sc.AccountID = 0;
            sc.ScreenContentTypeID = 0;
            sc.ScreenContentName = String.Empty;
            sc.ScreenContentTitle = String.Empty;
            sc.ThumbnailImageID = 0;
            sc.CustomField1 = String.Empty;
            sc.CustomField2 = String.Empty;
            sc.CustomField3 = String.Empty;
            sc.CustomField4 = String.Empty;
            sc.IsActive = true;

            return sc;
        }

        private string ValidateInput(ScreenContent screencontent)
        {
            if (screencontent.AccountID == 0)
                return "Account ID is not valid.";

            if (String.IsNullOrEmpty(screencontent.ScreenContentName))
                return "Screen Content Name is required.";

            if (String.IsNullOrEmpty(screencontent.ScreenContentTitle))
                return "Screen Content Title is required.";

            if (screencontent.ScreenContentTypeID == 0)
                return "You must select a Screen Content Type.";

            if (String.IsNullOrEmpty(screencontent.ScreenContentName))
                return "Screen Content Name is required.";

            if (screencontent.ThumbnailImageID == 0)
                return "You must select a Thumbnail Image.";

            // Validate based on selected type


            return String.Empty;
        }
    }
}
