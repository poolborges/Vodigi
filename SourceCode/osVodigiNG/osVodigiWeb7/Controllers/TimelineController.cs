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
    public class TimelineController : Controller
    {
        ITimelineRepository repository;
        string firstimagefile = String.Empty;
        string selectedimagefile = String.Empty;
        string firstvideofile = String.Empty;
        string selectedvideofile = String.Empty;

        public TimelineController()
            : this(new EntityTimelineRepository())
        { }

        public TimelineController(ITimelineRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /Timeline/

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
                TimelinePageState pagestate = GetPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.TimelineName = Request.Form["txtTimelineName"].ToString().Trim();
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
                ViewData["TimelineName"] = pagestate.TimelineName;
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
                int recordcount = repository.GetTimelineRecordCount(pagestate.AccountID, pagestate.TimelineName, pagestate.Tag, pagestate.IncludeInactive);

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

                ViewResult result = View(repository.GetTimelinePage(pagestate.AccountID, pagestate.TimelineName, pagestate.Tag, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Timeline", "Index", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Timeline/Create

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

                // Get the Image, Music, and Video lists
                ViewData["ImageList"] = new SelectList(BuildImageList(), "Value", "Text", "");
                ViewData["MusicList"] = new SelectList(BuildMusicList(), "Value", "Text", "");
                ViewData["VideoList"] = new SelectList(BuildVideoList(), "Value", "Text", "");
                ViewData["TimelineMediaList"] = new SelectList(BuildTimelineMediaList(""), "Value", "Text", "");
                ViewData["TimelineMusicList"] = new SelectList(BuildTimelineMusicList(""), "Value", "Text", "");
                ViewData["TimelineMedia"] = String.Empty;
                ViewData["TimelineMusic"] = String.Empty;
                ViewData["ImageUrl"] = firstimagefile;
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";
                ViewData["VideoUrl"] = firstvideofile;
                ViewData["VideoFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Videos/";

                ViewData["ValidationMessage"] = String.Empty;

                return View(CreateNewTimeline());
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Timeline", "Create", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Timeline/Create

        [HttpPost]
        public ActionResult Create(Timeline timeline)
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
                    timeline = FillNulls(timeline);
                    timeline.AccountID = Convert.ToInt32(Session["UserAccountID"]);

                    string validation = ValidateInput(timeline);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["ImageList"] = new SelectList(BuildImageList(), "Value", "Text", "");
                        ViewData["MusicList"] = new SelectList(BuildMusicList(), "Value", "Text", "");
                        ViewData["VideoList"] = new SelectList(BuildVideoList(), "Value", "Text", "");
                        ViewData["TimelineMediaList"] = new SelectList(BuildTimelineMediaList(Request.Form["txtTimelineMedia"].ToString()), "Value", "Text", "");
                        ViewData["TimelineMusicList"] = new SelectList(BuildTimelineMusicList(Request.Form["txtTimelineMusic"].ToString()), "Value", "Text", "");
                        ViewData["TimelineMedia"] = Request.Form["txtTimelineMedia"].ToString(); ;
                        ViewData["TimelineMusic"] = Request.Form["txtTimelineMusic"].ToString(); ;
                        ViewData["ImageUrl"] = firstimagefile;
                        ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";
                        ViewData["VideoUrl"] = firstvideofile;
                        ViewData["VideoFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Videos/";

                        return View(timeline);
                    }
                    else
                    {
                        // Create the timeline
                        repository.CreateTimeline(timeline);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Timeline", "Add",
                            "Added timeline '" + timeline.TimelineName + "' - ID: " + timeline.TimelineID.ToString());

                        string[] guids;
                        int i = 0;

                        // Create the image xrefs
                        ITimelineImageXrefRepository imagexrefrep = new EntityTimelineImageXrefRepository();
                        IImageRepository imagerep = new EntityImageRepository();
                        guids = Request.Form["txtTimelineMedia"].ToString().Split('|');
                        i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Image image = imagerep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), guid);
                                if (image != null)
                                {
                                    TimelineImageXref xref = new TimelineImageXref();
                                    xref.DisplayOrder = i;
                                    xref.TimelineID = timeline.TimelineID;
                                    xref.ImageID = image.ImageID;
                                    imagexrefrep.CreateTimelineImageXref(xref);
                                }
                                i += 1;
                            }
                        }

                        // Create the video xrefs
                        ITimelineVideoXrefRepository videoxrefrep = new EntityTimelineVideoXrefRepository();
                        IVideoRepository videorep = new EntityVideoRepository();
                        guids = Request.Form["txtTimelineMedia"].ToString().Split('|');
                        i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Video video = videorep.GetVideoByGuid(Convert.ToInt32(Session["UserAccountID"]), guid);
                                if (video != null)
                                {
                                    TimelineVideoXref xref = new TimelineVideoXref();
                                    xref.DisplayOrder = i;
                                    xref.TimelineID = timeline.TimelineID;
                                    xref.VideoID = video.VideoID;
                                    videoxrefrep.CreateTimelineVideoXref(xref);
                                }
                                i += 1;
                            }
                        }

                        // Create the music xrefs
                        ITimelineMusicXrefRepository musicxrefrep = new EntityTimelineMusicXrefRepository();
                        IMusicRepository musicrep = new EntityMusicRepository();
                        guids = Request.Form["txtTimelineMusic"].ToString().Split('|');
                        i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Music music = musicrep.GetMusicByGuid(Convert.ToInt32(Session["UserAccountID"]), guid);
                                if (music != null)
                                {
                                    TimelineMusicXref xref = new TimelineMusicXref();
                                    xref.PlayOrder = i;
                                    xref.TimelineID = timeline.TimelineID;
                                    xref.MusicID = music.MusicID;
                                    musicxrefrep.CreateTimelineMusicXref(xref);
                                    i += 1;
                                }
                            }
                        }

                        return RedirectToAction("Index");
                    }
                }

                return View(timeline);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Timeline", "Create POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Timeline/Edit/5

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

                Timeline timeline = repository.GetTimeline(id);

                // Get the data to populate the lists and hidden fields
                ViewData["ImageList"] = new SelectList(BuildImageList(), "Value", "Text", "");
                ViewData["MusicList"] = new SelectList(BuildMusicList(), "Value", "Text", "");
                ViewData["VideoList"] = new SelectList(BuildVideoList(), "Value", "Text", "");
                ViewData["ImageUrl"] = firstimagefile;
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";
                ViewData["VideoUrl"] = firstvideofile;
                ViewData["VideoFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Videos/";

                List<TimelineMediaSort> media = new List<TimelineMediaSort>();

                // Get the images   
                ITimelineImageXrefRepository imagexrefrep = new EntityTimelineImageXrefRepository();
                IImageRepository imgrep = new EntityImageRepository();
                IEnumerable<TimelineImageXref> imagexrefs = imagexrefrep.GetTimelineImageXrefs(id);
                foreach (TimelineImageXref imagexref in imagexrefs)
                {
                    Image img = imgrep.GetImage(imagexref.ImageID);
                    TimelineMediaSort sort = new TimelineMediaSort();
                    sort.guid = img.StoredFilename;
                    sort.DisplayOrder = imagexref.DisplayOrder;
                    media.Add(sort);
                }

                // Get the videos   
                ITimelineVideoXrefRepository videoxrefrep = new EntityTimelineVideoXrefRepository();
                IVideoRepository vidrep = new EntityVideoRepository();
                IEnumerable<TimelineVideoXref> videoxrefs = videoxrefrep.GetTimelineVideoXrefs(id);
                foreach (TimelineVideoXref videoxref in videoxrefs)
                {
                    Video vid = vidrep.GetVideo(videoxref.VideoID);
                    TimelineMediaSort sort = new TimelineMediaSort();
                    sort.guid = vid.StoredFilename;
                    sort.DisplayOrder = videoxref.DisplayOrder;
                    media.Add(sort);
                }

                // Build a list of sorted media guids
                string mediaguids = String.Empty;
                media.Sort();
                foreach (TimelineMediaSort sort in media)
                {
                    mediaguids += "|" + sort.guid;
                }

                ViewData["TimelineMedia"] = mediaguids;
                ViewData["TimelineMediaList"] = new SelectList(BuildTimelineMediaList(mediaguids), "Value", "Text", "");

                // Get the music
                string musicguids = String.Empty;
                ITimelineMusicXrefRepository musicxrefrep = new EntityTimelineMusicXrefRepository();
                IMusicRepository musicrep = new EntityMusicRepository();
                IEnumerable<TimelineMusicXref> musicxrefs = musicxrefrep.GetTimelineMusicXrefs(id);
                foreach (TimelineMusicXref musicxref in musicxrefs)
                {
                    Music msc = musicrep.GetMusic(musicxref.MusicID);
                    musicguids += "|" + msc.StoredFilename;
                }

                ViewData["TimelineMusic"] = musicguids;
                ViewData["TimelineMusicList"] = new SelectList(BuildTimelineMusicList(musicguids), "Value", "Text", "");

                ViewData["ValidationMessage"] = String.Empty;

                return View(timeline);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Timeline", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Timeline/Edit/5

        [HttpPost]
        public ActionResult Edit(Timeline timeline)
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
                    timeline = FillNulls(timeline);
                    timeline.AccountID = Convert.ToInt32(Session["UserAccountID"]);

                    string validation = ValidateInput(timeline);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["ImageList"] = new SelectList(BuildImageList(), "Value", "Text", "");
                        ViewData["MusicList"] = new SelectList(BuildMusicList(), "Value", "Text", "");
                        ViewData["VideoList"] = new SelectList(BuildVideoList(), "Value", "Text", "");
                        ViewData["TimelineMediaList"] = new SelectList(BuildTimelineMediaList(Request.Form["txtTimelineMedia"].ToString()), "Value", "Text", "");
                        ViewData["TimelineMusicList"] = new SelectList(BuildTimelineMusicList(Request.Form["txtTimelineMusic"].ToString()), "Value", "Text", "");
                        ViewData["TimelineMedia"] = Request.Form["txtTimelineMedia"].ToString(); ;
                        ViewData["TimelineMusic"] = Request.Form["txtTimelineMusic"].ToString(); ;
                        ViewData["ImageUrl"] = firstimagefile;
                        ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";
                        ViewData["VideoUrl"] = firstvideofile;
                        ViewData["VideoFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Videos/";

                        return View(timeline);
                    }
                    else
                    {
                        // Update the timeline
                        repository.UpdateTimeline(timeline);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Timeline", "Edit",
                            "Edited timeline '" + timeline.TimelineName + "' - ID: " + timeline.TimelineID.ToString());

                        string[] guids;
                        int i = 0;

                        // Delete the image, video, and music xrefs, then create new ones
                        ITimelineImageXrefRepository imagexrefrep = new EntityTimelineImageXrefRepository();
                        ITimelineVideoXrefRepository videoxrefrep = new EntityTimelineVideoXrefRepository();
                        ITimelineMusicXrefRepository musicxrefrep = new EntityTimelineMusicXrefRepository();
                        imagexrefrep.DeleteTimelineImageXrefs(timeline.TimelineID);
                        videoxrefrep.DeleteTimelineVideoXrefs(timeline.TimelineID);
                        musicxrefrep.DeleteTimelineMusicXrefs(timeline.TimelineID);

                        // Create the image xrefs
                        IImageRepository imagerep = new EntityImageRepository();
                        guids = Request.Form["txtTimelineMedia"].ToString().Split('|');
                        i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Image image = imagerep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), guid);
                                if (image != null)
                                {
                                    TimelineImageXref xref = new TimelineImageXref();
                                    xref.DisplayOrder = i;
                                    xref.TimelineID = timeline.TimelineID;
                                    xref.ImageID = image.ImageID;
                                    imagexrefrep.CreateTimelineImageXref(xref);
                                }
                                i += 1;
                            }
                        }

                        // Create the video xrefs
                        IVideoRepository videorep = new EntityVideoRepository();
                        guids = Request.Form["txtTimelineMedia"].ToString().Split('|');
                        i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Video video = videorep.GetVideoByGuid(Convert.ToInt32(Session["UserAccountID"]), guid);
                                if (video != null)
                                {
                                    TimelineVideoXref xref = new TimelineVideoXref();
                                    xref.DisplayOrder = i;
                                    xref.TimelineID = timeline.TimelineID;
                                    xref.VideoID = video.VideoID;
                                    videoxrefrep.CreateTimelineVideoXref(xref);
                                }
                                i += 1;
                            }
                        }

                        // Create the music xrefs
                        IMusicRepository musicrep = new EntityMusicRepository();
                        guids = Request.Form["txtTimelineMusic"].ToString().Split('|');
                        i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Music music = musicrep.GetMusicByGuid(Convert.ToInt32(Session["UserAccountID"]), guid);
                                if (music != null)
                                {
                                    TimelineMusicXref xref = new TimelineMusicXref();
                                    xref.PlayOrder = i;
                                    xref.TimelineID = timeline.TimelineID;
                                    xref.MusicID = music.MusicID;
                                    musicxrefrep.CreateTimelineMusicXref(xref);
                                    i += 1;
                                }
                            }
                        }

                        return RedirectToAction("Index");
                    }
                }

                return View(timeline);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Timeline", "Edit POST", ex.Message);
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
            sortitem1.Text = "Timeline Name";
            sortitem1.Value = "TimelineName";

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

        private TimelinePageState GetPageState()
        {
            try
            {
                TimelinePageState pagestate = new TimelinePageState();


                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["TimelinePageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.TimelineName = String.Empty;
                    pagestate.Tag = String.Empty;
                    pagestate.IncludeInactive = false;
                    pagestate.SortBy = "TimelineName";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    Session["TimelinePageState"] = pagestate;
                }
                else
                {
                    pagestate = (TimelinePageState)Session["TimelinePageState"];
                }
                return pagestate;
            }
            catch { return new TimelinePageState(); }
        }

        private void SavePageState(TimelinePageState pagestate)
        {
            Session["TimelinePageState"] = pagestate;
        }

        private Timeline CreateNewTimeline()
        {
            Timeline timeline = new Timeline();
            timeline.TimelineID = 0;
            timeline.AccountID = 0;
            timeline.TimelineName = String.Empty;
            timeline.Tags = String.Empty;
            timeline.IsActive = true;
            timeline.DurationInSecs = 10;
            timeline.MuteMusicOnPlayback = true;

            return timeline;
        }

        private Timeline FillNulls(Timeline timeline)
        {
            if (timeline.Tags == null) timeline.Tags = String.Empty;

            return timeline;
        }

        private string ValidateInput(Timeline timeline)
        {
            if (timeline.AccountID == 0)
                return "Account ID is not valid.";

            if (String.IsNullOrEmpty(timeline.TimelineName))
                return "Timeline Name is required.";

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
                    firstimagefile = imagefolder + img.StoredFilename;
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

        private List<SelectListItem> BuildVideoList()
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Get the active videos
            IVideoRepository vidrep = new EntityVideoRepository();
            IEnumerable<Video> vids = vidrep.GetAllVideos(accountid);

            string videofolder = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Videos/";

            List<SelectListItem> items = new List<SelectListItem>();
            bool first = true;
            foreach (Video vid in vids)
            {
                if (first)
                {
                    first = false;
                    firstvideofile = videofolder + vid.StoredFilename;
                }

                SelectListItem item = new SelectListItem();
                item.Text = vid.VideoName;
                item.Value = vid.StoredFilename;

                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildTimelineMediaList(string mediaguids)
        {
            IImageRepository imgrep = new EntityImageRepository();
            IVideoRepository vidrep = new EntityVideoRepository();
            List<SelectListItem> items = new List<SelectListItem>();

            // Get the images and videos from the database
            string[] guids = mediaguids.Split('|');
            foreach (string guid in guids)
            {
                if (!String.IsNullOrEmpty(guid.Trim()))
                {
                    Image img = imgrep.GetImageByGuid(Convert.ToInt32(Session["UserAccountID"]), guid.Trim());
                    if (img != null)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = img.ImageName + " (Image)";
                        item.Value = img.StoredFilename;

                        items.Add(item);
                    }
                    else
                    {
                        Video vid = vidrep.GetVideoByGuid(Convert.ToInt32(Session["UserAccountID"]), guid.Trim());
                        if (vid != null)
                        {
                            SelectListItem item = new SelectListItem();
                            item.Text = vid.VideoName + " (Video)";
                            item.Value = vid.StoredFilename;

                            items.Add(item);
                        }
                    }
                }
            }

            return items;
        }

        private List<SelectListItem> BuildTimelineMusicList(string musicguids)
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

    }
}
