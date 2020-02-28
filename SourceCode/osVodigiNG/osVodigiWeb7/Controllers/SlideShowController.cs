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
    public class SlideShowController : Controller
    {
        ISlideShowRepository repository;
        string firstfile = String.Empty;
        string selectedfile = String.Empty;

        public SlideShowController()
            : this(new EntitySlideShowRepository())
        { }

        public SlideShowController(ISlideShowRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /SlideShow/

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
                SlideShowPageState pagestate = GetPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.SlideShowName = Request.Form["txtSlideShowName"].ToString().Trim();
                    pagestate.Tag = Request.Form["txtTag"].ToString().Trim();
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
                ViewData["SlideShowName"] = pagestate.SlideShowName;
                ViewData["Tag"] = pagestate.Tag;
                ViewData["IncludeInactive"] = pagestate.IncludeInactive;
                ViewData["SortBy"] = pagestate.SortBy;
                ViewData["SortByList"] = new SelectList(BuildSortByList(), "Value", "Text", pagestate.SortBy);
                ViewData["AscDescList"] = new SelectList(BuildAscDescList(), "Value", "Text", pagestate.AscDesc);

                // Determine asc/desc
                bool isdescending = false;
                if (pagestate.AscDesc.ToLower().StartsWith("d"))
                    isdescending = true;

                // Get a Count of all filtered records
                int recordcount = repository.GetSlideShowRecordCount(pagestate.AccountID, pagestate.SlideShowName, pagestate.Tag, pagestate.IncludeInactive);

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

                ViewResult result = View(repository.GetSlideShowPage(pagestate.AccountID, pagestate.SlideShowName, pagestate.Tag, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SlideShow", "Index", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /SlideShow/Create

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
                ViewData["ImageList"] = new SelectList(BuildImageList(), "Value", "Text", "");
                ViewData["MusicList"] = new SelectList(BuildMusicList(), "Value", "Text", "");
                ViewData["ImageUrl"] = firstfile;
                ViewData["SlideShowImages"] = String.Empty;
                ViewData["SlideShowMusic"] = String.Empty;
                ViewData["SlideShowImageList"] = new SelectList(BuildSlideShowImageList(""), "Value", "Text", "");
                ViewData["SlideShowMusicList"] = new SelectList(BuildSlideShowMusicList(""), "Value", "Text", "");
                ViewData["TransitionTypeList"] = new SelectList(BuildTransitionTypeList(), "Value", "Text", "");

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                return View(CreateNewSlideShow());
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SlideShow", "Create", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /SlideShow/Create

        [HttpPost]
        public ActionResult Create(SlideShow slideshow)
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
                    slideshow = FillNulls(slideshow);
                    slideshow.AccountID = Convert.ToInt32(Session["UserAccountID"]);

                    string validation = ValidateInput(slideshow, Request.Form["txtSlideShowImages"].ToString());
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["ImageList"] = new SelectList(BuildImageList(), "Value", "Text", "");
                        ViewData["MusicList"] = new SelectList(BuildMusicList(), "Value", "Text", "");
                        ViewData["ImageUrl"] = firstfile;
                        ViewData["SlideShowImages"] = Request.Form["txtSlideShowImages"].ToString();
                        ViewData["MusicImages"] = Request.Form["txtSlideShowMusic"].ToString();
                        ViewData["SlideShowImageList"] = new SelectList(BuildSlideShowImageList(Request.Form["txtSlideShowImages"].ToString()), "Value", "Text", "");
                        ViewData["SlideShowMusicList"] = new SelectList(BuildSlideShowMusicList(Request.Form["txtSlideShowMusic"].ToString()), "Value", "Text", "");
                        ViewData["TransitionTypeList"] = new SelectList(BuildTransitionTypeList(), "Value", "Text", Request.Form["lstTransitionType"].ToString());

                        // Get the account id
                        int accountid = 0;
                        if (Session["UserAccountID"] != null)
                            accountid = Convert.ToInt32(Session["UserAccountID"]);
                        ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                        return View(slideshow);
                    }
                    else
                    {
                        // Create the slideshow
                        slideshow.TransitionType = Request.Form["lstTransitionType"].ToString();
                        repository.CreateSlideShow(slideshow);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Slide Show", "Add",
                            "Added slide show '" + slideshow.SlideShowName + "' - ID: " + slideshow.SlideShowID.ToString());

                        ISlideShowImageXrefRepository xrefrep = new EntitySlideShowImageXrefRepository();
                        ISlideShowMusicXrefRepository musicxrefrep = new EntitySlideShowMusicXrefRepository();
                        IImageRepository imgrep = new EntityImageRepository();
                        IMusicRepository musicrep = new EntityMusicRepository();

                        // Create a xref for each image in the slideshow
                        string[] guids = Request.Form["txtSlideShowImages"].ToString().Split('|');
                        int i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Image img = imgrep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), guid);
                                if (img != null)
                                {
                                    SlideShowImageXref xref = new SlideShowImageXref();
                                    xref.PlayOrder = i;
                                    xref.SlideShowID = slideshow.SlideShowID;
                                    xref.ImageID = img.ImageID;
                                    xrefrep.CreateSlideShowImageXref(xref);
                                    i += 1;
                                }
                            }
                        }

                        // Create a xref for each music file in the slideshow
                        guids = Request.Form["txtSlideShowMusic"].ToString().Split('|');
                        i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Music music = musicrep.GetMusicByGuid(Convert.ToInt32(Session["UserAccountID"]), guid);
                                if (music != null)
                                {
                                    SlideShowMusicXref xref = new SlideShowMusicXref();
                                    xref.PlayOrder = i;
                                    xref.SlideShowID = slideshow.SlideShowID;
                                    xref.MusicID = music.MusicID;
                                    musicxrefrep.CreateSlideShowMusicXref(xref);
                                    i += 1;
                                }
                            }
                        }

                        return RedirectToAction("Index");
                    }
                }

                return View(slideshow);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SlideShow", "Create POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /SlideShow/Edit/5

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

                SlideShow slideshow = repository.GetSlideShow(id);
                ViewData["ValidationMessage"] = String.Empty;
                ViewData["ImageList"] = new SelectList(BuildImageList(), "Value", "Text", "");
                ViewData["MusicList"] = new SelectList(BuildMusicList(), "Value", "Text", "");
                ViewData["ImageUrl"] = firstfile;

                // Get the image guids for the slideshow
                string guids = String.Empty;
                ISlideShowImageXrefRepository xrefrep = new EntitySlideShowImageXrefRepository();
                IImageRepository imgrep = new EntityImageRepository();
                IEnumerable<SlideShowImageXref> xrefs = xrefrep.GetSlideShowImageXrefs(id);
                foreach (SlideShowImageXref xref in xrefs)
                {
                    Image img = imgrep.GetImage(xref.ImageID);
                    guids += "|" + img.StoredFilename;
                }

                ViewData["SlideShowImages"] = guids;
                ViewData["SlideShowImageList"] = new SelectList(BuildSlideShowImageList(guids), "Value", "Text", "");

                // Get the music guids for the slideshow
                string musicguids = String.Empty;
                ISlideShowMusicXrefRepository musicxrefrep = new EntitySlideShowMusicXrefRepository();
                IMusicRepository musicrep = new EntityMusicRepository();
                IEnumerable<SlideShowMusicXref> musicxrefs = musicxrefrep.GetSlideShowMusicXrefs(id);
                foreach (SlideShowMusicXref musicxref in musicxrefs)
                {
                    Music msc = musicrep.GetMusic(musicxref.MusicID);
                    musicguids += "|" + msc.StoredFilename;
                }

                ViewData["SlideShowMusic"] = musicguids;
                ViewData["SlideShowMusicList"] = new SelectList(BuildSlideShowMusicList(musicguids), "Value", "Text", "");

                ViewData["TransitionTypeList"] = new SelectList(BuildTransitionTypeList(), "Value", "Text", slideshow.TransitionType);

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                return View(slideshow);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SlideShow", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /SlideShow/Edit/5

        [HttpPost]
        public ActionResult Edit(SlideShow slideshow)
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
                    slideshow = FillNulls(slideshow);

                    string validation = ValidateInput(slideshow, Request.Form["txtSlideShowImages"].ToString());
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["ImageList"] = new SelectList(BuildImageList(), "Value", "Text", "");
                        ViewData["ImageUrl"] = firstfile;
                        ViewData["SlideShowImages"] = Request.Form["txtSlideShowImages"].ToString();
                        ViewData["SlideShowMusic"] = Request.Form["txtSlideShowMusic"].ToString();
                        ViewData["SlideShowImageList"] = new SelectList(BuildSlideShowImageList(Request.Form["txtSlideShowImages"].ToString()), "Value", "Text", "");
                        ViewData["SlideShowMusicList"] = new SelectList(BuildSlideShowMusicList(Request.Form["txtSlideShowMusic"].ToString()), "Value", "Text", "");
                        ViewData["TransitionTypeList"] = new SelectList(BuildTransitionTypeList(), "Value", "Text", slideshow.TransitionType);

                        // Get the account id
                        int accountid = 0;
                        if (Session["UserAccountID"] != null)
                            accountid = Convert.ToInt32(Session["UserAccountID"]);
                        ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                        return View(slideshow);
                    }
                    else
                    {
                        // Update the slideshow
                        slideshow.TransitionType = Request.Form["lstTransitionType"].ToString();
                        repository.UpdateSlideShow(slideshow);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Slide Show", "Edit",
                            "Edited slide show '" + slideshow.SlideShowName + "' - ID: " + slideshow.SlideShowID.ToString());

                        ISlideShowImageXrefRepository xrefrep = new EntitySlideShowImageXrefRepository();
                        ISlideShowMusicXrefRepository musicxrefrep = new EntitySlideShowMusicXrefRepository();
                        IImageRepository imgrep = new EntityImageRepository();
                        IMusicRepository musicrep = new EntityMusicRepository();

                        // Delete existing xrefs for the slideshow
                        xrefrep.DeleteSlideShowImageXrefs(slideshow.SlideShowID);
                        musicxrefrep.DeleteSlideShowMusicXrefs(slideshow.SlideShowID);

                        // Create a xref for each image in the slideshow
                        string[] guids = Request.Form["txtSlideShowImages"].ToString().Split('|');
                        int i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Image img = imgrep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), guid);
                                if (img != null)
                                {
                                    SlideShowImageXref xref = new SlideShowImageXref();
                                    xref.PlayOrder = i;
                                    xref.SlideShowID = slideshow.SlideShowID;
                                    xref.ImageID = img.ImageID;
                                    xrefrep.CreateSlideShowImageXref(xref);
                                    i += 1;
                                }
                            }
                        }

                        // Create a xref for each music file in the slideshow
                        guids = Request.Form["txtSlideShowMusic"].ToString().Split('|');
                        i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Music music = musicrep.GetMusicByGuid(Convert.ToInt32(Session["UserAccountID"]), guid);
                                if (music != null)
                                {
                                    SlideShowMusicXref xref = new SlideShowMusicXref();
                                    xref.PlayOrder = i;
                                    xref.SlideShowID = slideshow.SlideShowID;
                                    xref.MusicID = music.MusicID;
                                    musicxrefrep.CreateSlideShowMusicXref(xref);
                                    i += 1;
                                }
                            }
                        }

                        return RedirectToAction("Index");
                    }
                }

                return View(slideshow);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("SlideShow", "Edit POST", ex.Message);
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
            sortitem1.Text = "Slide Show Name";
            sortitem1.Value = "SlideShowName";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Tags";
            sortitem2.Value = "Tags";

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

        private SlideShowPageState GetPageState()
        {
            try
            {
                SlideShowPageState pagestate = new SlideShowPageState();


                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["SlideShowPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.SlideShowName = String.Empty;
                    pagestate.Tag = String.Empty;
                    pagestate.IncludeInactive = false;
                    pagestate.SortBy = "SlideShowName";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    Session["SlideShowPageState"] = pagestate;
                }
                else
                {
                    pagestate = (SlideShowPageState)Session["SlideShowPageState"];
                }
                return pagestate;
            }
            catch { return new SlideShowPageState(); }
        }

        private void SavePageState(SlideShowPageState pagestate)
        {
            Session["SlideShowPageState"] = pagestate;
        }

        private SlideShow CreateNewSlideShow()
        {
            SlideShow slideshow = new SlideShow();
            slideshow.SlideShowID = 0;
            slideshow.AccountID = 0;
            slideshow.SlideShowName = String.Empty;
            slideshow.Tags = String.Empty;
            slideshow.IntervalInSecs = 10;
            slideshow.IsActive = true;

            return slideshow;
        }

        private SlideShow FillNulls(SlideShow slideshow)
        {
            if (slideshow.Tags == null) slideshow.Tags = String.Empty;

            return slideshow;
        }

        private string ValidateInput(SlideShow slideshow, string sImages)
        {
            if (slideshow.AccountID == 0)
                return "Account ID is not valid.";

            if (String.IsNullOrEmpty(slideshow.SlideShowName))
                return "Slide Show Name is required.";

            if (String.IsNullOrEmpty(sImages.Replace("|", "")))
                return "You must select at least one image for this slide show.";

            return String.Empty;
        }

        private List<SelectListItem> BuildImageList()
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

                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildMusicList()
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Get the active music files
            IMusicRepository musicrep = new EntityMusicRepository();
            IEnumerable<Music> musics = musicrep.GetAllMusics(accountid);

            string musicfolder = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Music/";

            List<SelectListItem> items = new List<SelectListItem>();
            foreach (Music music in musics)
            {
                SelectListItem item = new SelectListItem();
                item.Text = music.MusicName;
                item.Value = music.StoredFilename;

                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildSlideShowImageList(string imageguids)
        {
            IImageRepository imgrep = new EntityImageRepository();
            List<SelectListItem> items = new List<SelectListItem>();

            // Get the image from the database
            string[] guids = imageguids.Split('|');
            foreach (string guid in guids)
            {
                if (!String.IsNullOrEmpty(guid.Trim()))
                {
                    Image img = imgrep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), guid.Trim());
                    if (img != null)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = img.ImageName;
                        item.Value = img.StoredFilename;

                        items.Add(item);
                    }
                }
            }

            return items;
        }

        private List<SelectListItem> BuildSlideShowMusicList(string musicguids)
        {
            IMusicRepository musicrep = new EntityMusicRepository();
            List<SelectListItem> items = new List<SelectListItem>();

            // Get the music from the database
            string[] guids = musicguids.Split('|');
            foreach (string guid in guids)
            {
                if (!String.IsNullOrEmpty(guid.Trim()))
                {
                    Music music = musicrep.GetMusicByGuid(Convert.ToInt32(Session["UserAccountID"]), guid.Trim());
                    if (music != null)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = music.MusicName;
                        item.Value = music.StoredFilename;

                        items.Add(item);
                    }
                }
            }

            return items;
        }

        private List<SelectListItem> BuildTransitionTypeList()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            string[] types = { "Fade", "Drop From Top", "Slide From Right", "Pan Zoom", "Zoom In" };
            foreach (string type in types)
            {
                SelectListItem item = new SelectListItem();
                item.Text = type;
                item.Value = type;

                items.Add(item);
            }

            return items;
        }
    }
}
