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
    public class PlayListController : Controller
    {
        IPlayListRepository repository;
        string firstfile = String.Empty;
        string selectedfile = String.Empty;

        public PlayListController()
            : this(new EntityPlayListRepository())
        { }

        public PlayListController(IPlayListRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /PlayList/

        public ActionResult Index()
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                // Initialize or get the page state using session
                PlayListPageState pagestate = GetPageState();

                // Get the account id
                int accountid = AuthUtils.GetAccountId();

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.PlayListName = Request.Form["txtPlayListName"].ToString().Trim();
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
                ViewData["PlayListName"] = pagestate.PlayListName;
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
                int recordcount = repository.GetPlayListRecordCount(pagestate.AccountID, pagestate.PlayListName, pagestate.Tag, pagestate.IncludeInactive);

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

                ViewResult result = View(repository.GetPlayListPage(pagestate.AccountID, pagestate.PlayListName, pagestate.Tag, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("PlayList", "Index", ex);
                
            }
        }

        //
        // GET: /PlayList/Create

        public ActionResult Create()
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                ViewData["ValidationMessage"] = String.Empty;
                ViewData["VideoList"] = new SelectList(BuildVideoList(), "Value", "Text", "");
                ViewData["VideoUrl"] = firstfile;
                ViewData["PlayListVideos"] = String.Empty;
                ViewData["PlayListVideoList"] = new SelectList(BuildPlayListVideoList(""), "Value", "Text", "");

                // Get the account id
                int accountid = AuthUtils.GetAccountId();
                ViewData["VideoFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/";

                return View(CreateNewPlayList());
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("PlayList", "Create", ex);
                
            }
        }

        //
        // POST: /PlayList/Create

        [HttpPost]
        public ActionResult Create(PlayList playlist)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    playlist = FillNulls(playlist);
                    playlist.AccountID = AuthUtils.GetAccountId();

                    string validation = ValidateInput(playlist, Request.Form["txtPlayListVideos"].ToString());
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["VideoList"] = new SelectList(BuildVideoList(), "Value", "Text", "");
                        ViewData["VideoUrl"] = firstfile;
                        ViewData["PlayListVideos"] = Request.Form["txtPlayListVideos"].ToString();
                        ViewData["PlayListVideoList"] = new SelectList(BuildPlayListVideoList(Request.Form["txtPlayListVideos"].ToString()), "Value", "Text", "");

                        // Get the account id
                        int accountid = AuthUtils.GetAccountId();
                        ViewData["VideoFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/";

                        return View(playlist);
                    }
                    else
                    {
                        // Create the playlist
                        repository.CreatePlayList(playlist);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Play List", "Add",
                                "Added play list '" + playlist.PlayListName + "' - ID: " + playlist.PlayListID.ToString());

                        IPlayListVideoXrefRepository xrefrep = new EntityPlayListVideoXrefRepository();
                        IVideoRepository vidrep = new EntityVideoRepository();

                        // Create a xref for each video in the playlist
                        string[] guids = Request.Form["txtPlayListVideos"].ToString().Split('|');
                        int i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Video vid = vidrep.GetVideoByGuid(AuthUtils.GetAccountId(), guid);
                                if (vid != null)
                                {
                                    PlayListVideoXref xref = new PlayListVideoXref();
                                    xref.PlayOrder = i;
                                    xref.PlayListID = playlist.PlayListID;
                                    xref.VideoID = vid.VideoID;
                                    xrefrep.CreatePlayListVideoXref(xref);
                                    i += 1;
                                }
                            }
                        }

                        return RedirectToAction("Index");
                    }
                }

                return View(playlist);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("PlayList", "Create POST", ex);
                
            }
        }

        //
        // GET: /PlayList/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                PlayList playlist = repository.GetPlayList(id);
                ViewData["ValidationMessage"] = String.Empty;
                ViewData["VideoList"] = new SelectList(BuildVideoList(), "Value", "Text", "");
                ViewData["VideoUrl"] = firstfile;

                // Get the video guids for the playlist
                string guids = String.Empty;
                IPlayListVideoXrefRepository xrefrep = new EntityPlayListVideoXrefRepository();
                IVideoRepository imgrep = new EntityVideoRepository();
                IEnumerable<PlayListVideoXref> xrefs = xrefrep.GetPlayListVideoXrefs(id);
                foreach (PlayListVideoXref xref in xrefs)
                {
                    Video vid = imgrep.GetVideo(xref.VideoID);
                    guids += "|" + vid.StoredFilename;
                }

                ViewData["PlayListVideos"] = guids;
                ViewData["PlayListVideoList"] = new SelectList(BuildPlayListVideoList(guids), "Value", "Text", "");

                // Get the account id
                int accountid = AuthUtils.GetAccountId();
                ViewData["VideoFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/";

                return View(playlist);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("PlayList", "Edit", ex);
                
            }
        }

        //
        // POST: /PlayList/Edit/5

        [HttpPost]
        public ActionResult Edit(PlayList playlist)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    playlist = FillNulls(playlist);

                    string validation = ValidateInput(playlist, Request.Form["txtPlayListVideos"].ToString());
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["VideoList"] = new SelectList(BuildVideoList(), "Value", "Text", "");
                        ViewData["VideoUrl"] = firstfile;
                        ViewData["PlayListVideos"] = Request.Form["txtPlayListVideos"].ToString();
                        ViewData["PlayListVideoList"] = new SelectList(BuildPlayListVideoList(Request.Form["txtPlayListVideos"].ToString()), "Value", "Text", "");

                        // Get the account id
                        int accountid = AuthUtils.GetAccountId();
                        ViewData["VideoFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/";
                        return View(playlist);
                    }
                    else
                    {
                        // Update the playlist
                        repository.UpdatePlayList(playlist);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Play List", "Edit",
                                "Edited play list '" + playlist.PlayListName + "' - ID: " + playlist.PlayListID.ToString());

                        IPlayListVideoXrefRepository xrefrep = new EntityPlayListVideoXrefRepository();
                        IVideoRepository vidrep = new EntityVideoRepository();

                        // Delete existing xrefs for the playlist
                        xrefrep.DeletePlayListVideoXrefs(playlist.PlayListID);

                        // Create a xref for each video in the playlist
                        string[] guids = Request.Form["txtPlayListVideos"].ToString().Split('|');
                        int i = 1;
                        foreach (string guid in guids)
                        {
                            if (!String.IsNullOrEmpty(guid.Trim()))
                            {
                                Video vid = vidrep.GetVideoByGuid(AuthUtils.GetAccountId(), guid);
                                if (vid != null)
                                {
                                    PlayListVideoXref xref = new PlayListVideoXref();
                                    xref.PlayOrder = i;
                                    xref.PlayListID = playlist.PlayListID;
                                    xref.VideoID = vid.VideoID;
                                    xrefrep.CreatePlayListVideoXref(xref);
                                    i += 1;
                                }
                            }
                        }
                        return RedirectToAction("Index");
                    }
                }

                return View(playlist);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("PlayList", "Edit POST", ex);
                
            }
        }

        //
        // Support Methods

        private List<SelectListItem> BuildSortByList()
        {
            // Build the sort by list
            List<SelectListItem> sortitems = new List<SelectListItem>();

            SelectListItem sortitem1 = new SelectListItem();
            sortitem1.Text = "Play List Name";
            sortitem1.Value = "PlayListName";

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

        private PlayListPageState GetPageState()
        {
            try
            {
                PlayListPageState pagestate = new PlayListPageState();


                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["PlayListPageState"] == null)
                {
                    int accountid = AuthUtils.GetAccountId();

                    pagestate.AccountID = accountid;
                    pagestate.PlayListName = String.Empty;
                    pagestate.Tag = String.Empty;
                    pagestate.IncludeInactive = false;
                    pagestate.SortBy = "PlayListName";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    Session["PlayListPageState"] = pagestate;
                }
                else
                {
                    pagestate = (PlayListPageState)Session["PlayListPageState"];
                }
                return pagestate;
            }
            catch { return new PlayListPageState(); }
        }

        private void SavePageState(PlayListPageState pagestate)
        {
            Session["PlayListPageState"] = pagestate;
        }

        private PlayList CreateNewPlayList()
        {
            PlayList playlist = new PlayList();
            playlist.PlayListID = 0;
            playlist.AccountID = 0;
            playlist.PlayListName = String.Empty;
            playlist.Tags = String.Empty;
            playlist.IsActive = true;

            return playlist;
        }

        private PlayList FillNulls(PlayList playlist)
        {
            if (playlist.Tags == null) playlist.Tags = String.Empty;

            return playlist;
        }

        private string ValidateInput(PlayList playlist, string sVideos)
        {
            if (playlist.AccountID == 0)
                return "Account ID is not valid.";

            if (String.IsNullOrEmpty(playlist.PlayListName))
                return "Play List Name is required.";

            if (String.IsNullOrEmpty(sVideos.Replace("|", "")))
                return "You must select at least one video for this play list.";

            return String.Empty;
        }

        private List<SelectListItem> BuildVideoList()
        {
            // Get the account id
            int accountid = AuthUtils.GetAccountId();

            // Get the active videos
            IVideoRepository vidrep = new EntityVideoRepository();
            IEnumerable<Video> vids = vidrep.GetAllVideos(accountid);

            string videofolder = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/";

            List<SelectListItem> items = new List<SelectListItem>();
            bool first = true;
            foreach (Video vid in vids)
            {
                if (first)
                {
                    first = false;
                    firstfile = videofolder + vid.StoredFilename;
                }

                SelectListItem item = new SelectListItem();
                item.Text = vid.VideoName;
                item.Value = vid.StoredFilename;

                items.Add(item);
            }

            return items;
        }

        private List<SelectListItem> BuildPlayListVideoList(string videoguids)
        {
            IVideoRepository vidrep = new EntityVideoRepository();
            List<SelectListItem> items = new List<SelectListItem>();

            // Get the video from the database
            string[] guids = videoguids.Split('|');
            foreach (string guid in guids)
            {
                if (!String.IsNullOrEmpty(guid.Trim()))
                {
                    Video vid = vidrep.GetVideoByGuid(AuthUtils.GetAccountId(), guid.Trim());
                    if (vid != null)
                    {
                        SelectListItem item = new SelectListItem();
                        item.Text = vid.VideoName;
                        item.Value = vid.StoredFilename;

                        items.Add(item);
                    }
                }
            }

            return items;
        }
    }
}
