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
using System.IO;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class MusicController : Controller
    {
        IMusicRepository repository;
        string firstfile = String.Empty;
        string selectedfile = String.Empty;

        public MusicController()
            : this(new EntityMusicRepository())
        { }

        public MusicController(IMusicRepository paramrepository)
        {
            repository = paramrepository;
        }

        //
        // GET: /Music/

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
                MusicPageState pagestate = GetPageState();

                // Get the account id
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);

                // Set and save the page state to the submitted form values if any values are passed
                if (Request.Form["lstAscDesc"] != null)
                {
                    pagestate.AccountID = accountid;
                    pagestate.MusicName = Request.Form["txtMusicName"].ToString().Trim();
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
                ViewData["MusicName"] = pagestate.MusicName;
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
                int recordcount = repository.GetMusicRecordCount(pagestate.AccountID, pagestate.MusicName, pagestate.Tag, pagestate.IncludeInactive);

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

                // Set the music folder 
                ViewData["MusicFolder"] = @"~/Media/" + Convert.ToString(Session["UserAccountID"]) + @"/Music/";

                ViewResult result = View(repository.GetMusicPage(pagestate.AccountID, pagestate.MusicName, pagestate.Tag, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Music", "Index", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Music/Create

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
                ViewData["FileList"] = new SelectList(BuildFileList(""), "Value", "Text", "");
                ViewData["MusicURL"] = firstfile;

                return View(CreateNewMusic());
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Music", "Create", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Music/Create

        [HttpPost]
        public ActionResult Create(Music music)
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
                    music = FillNulls(music);
                    music.AccountID = Convert.ToInt32(Session["UserAccountID"]);
                    Guid fileguid = Guid.NewGuid();

                    if (Request.Form["lstFile"] != null && !String.IsNullOrEmpty(Request.Form["lstFile"].ToString().Trim()))
                    {
                        music.OriginalFilename = Request.Form["lstFile"].ToString().Trim();
                        if (music.OriginalFilename != "0")
                        {
                            string extension = music.OriginalFilename.Substring(music.OriginalFilename.LastIndexOf('.'));
                            music.StoredFilename = fileguid + extension;
                        }
                        else
                        {
                            music.OriginalFilename = String.Empty;
                        }
                    }
                    else
                    {
                        music.OriginalFilename = String.Empty;
                        music.StoredFilename = String.Empty;
                    }

                    string validation = ValidateInput(music);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["FileList"] = new SelectList(BuildFileList(Request.Form["lstFile"].ToString().Trim()), "Value", "Text", music.OriginalFilename);
                        ViewData["MusicURL"] = selectedfile;
                        return View(music);
                    }
                    else
                    {
                        try
                        {
                            // Move the music
                            string oldmusic = Server.MapPath(@"~/UploadedFiles");
                            if (!oldmusic.EndsWith(@"\"))
                                oldmusic += @"\";
                            oldmusic += Convert.ToString(Session["UserAccountID"]) + @"\Music\" + music.OriginalFilename;

                            string newmusic = Server.MapPath(@"~/Media");
                            if (!newmusic.EndsWith(@"\"))
                                newmusic += @"\";
                            newmusic += Convert.ToString(Session["UserAccountID"]) + @"\Music\" + music.StoredFilename;

                            System.IO.File.Copy(oldmusic, newmusic);
                            System.IO.File.Delete(oldmusic);
                        }
                        catch
                        {
                            ViewData["ValidationMessage"] = "Failed to copy file.";
                            ViewData["FileList"] = new SelectList(BuildFileList(Request.Form["lstFile"].ToString().Trim()), "Value", "Text", music.OriginalFilename);
                            ViewData["MusicURL"] = selectedfile;
                            return View(music);
                        }

                        repository.CreateMusic(music);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Music", "Add",
                                "Added music '" + music.MusicName + "' - ID: " + music.MusicID.ToString());

                        return RedirectToAction("Index");
                    }
                }

                return View(music);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Music", "Create POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Music/Edit/5

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

                Music music = repository.GetMusic(id);
                ViewData["MusicURL"] = @"~/Media/" + Convert.ToString(Session["UserAccountID"]) + @"/Music/" + music.StoredFilename;
                ViewData["ValidationMessage"] = String.Empty;

                return View(music);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Music", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Music/Edit/5

        [HttpPost]
        public ActionResult Edit(Music music)
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
                    music = FillNulls(music);

                    string validation = ValidateInput(music);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["MusicURL"] = @"~/Media/" + Convert.ToString(Session["UserAccountID"]) + @"/Music/" + music.StoredFilename;
                        ViewData["ValidationMessage"] = validation;
                        return View(music);
                    }

                    repository.UpdateMusic(music);

                    CommonMethods.CreateActivityLog((User)Session["User"], "Music", "Edit",
                            "Edited music '" + music.MusicName + "' - ID: " + music.MusicID.ToString());

                    return RedirectToAction("Index");
                }

                return View(music);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Music", "Edit POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /Music/Upload

        public ActionResult Upload()
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

                return View(CreateNewMusic());
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Music", "Upload", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Music/Upload

        [HttpPost]
        public ActionResult Upload(Music music)
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

                string validation = String.Empty;
                if (ModelState.IsValid)
                {
                    // Only one file is allowed per upload
                    HttpPostedFileBase file = Request.Files[0];
                    if (file != null && file.ContentLength > 0 && !String.IsNullOrEmpty(file.FileName))
                    {
                        if (file.ContentLength > 208000000)
                        {
                            ViewData["ValidationMessage"] = "Uploaded files must be 200MB or less.";
                        }
                        else
                        {
                            string fname = file.FileName.ToLower();
                            if (!fname.EndsWith(".mp3") && !fname.EndsWith(".wma"))
                            {
                                ViewData["ValidationMessage"] = "Only .mp3 and .wma music files can be uploaded.";
                            }
                            else
                            {
                                string filetype = "Music";
                                string filename = Path.GetFileName(file.FileName);
                                string serverpath = Server.MapPath("~/UploadedFiles");
                                if (!serverpath.EndsWith(@"\"))
                                    serverpath += @"\";
                                string path = serverpath + user.AccountID.ToString() + @"\" + filetype + @"\" + filename;
                                if (!System.IO.File.Exists(path))
                                    System.IO.File.Delete(path);
                                file.SaveAs(path);
                            }
                        }
                    }
                    else
                    {
                        ViewData["ValidationMessage"] = "You must select a file.";
                        return View(music);
                    }

                    // Set NULLs to Empty Strings
                    music = FillNulls(music);
                    music.AccountID = Convert.ToInt32(Session["UserAccountID"]);
                    Guid fileguid = Guid.NewGuid();

                    music.OriginalFilename = Path.GetFileName(file.FileName);
                    string extension = music.OriginalFilename.Substring(music.OriginalFilename.LastIndexOf('.'));
                    music.StoredFilename = fileguid + extension;

                    validation = ValidateInput(music);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(music);
                    }
                    else
                    {
                        try
                        {
                            // Move the music
                            string oldmusic = Server.MapPath(@"~/UploadedFiles");
                            if (!oldmusic.EndsWith(@"\"))
                                oldmusic += @"\";
                            oldmusic += Convert.ToString(Session["UserAccountID"]) + @"\Music\" + music.OriginalFilename;

                            string newmusic = Server.MapPath(@"~/Media");
                            if (!newmusic.EndsWith(@"\"))
                                newmusic += @"\";
                            newmusic += Convert.ToString(Session["UserAccountID"]) + @"\Music\" + music.StoredFilename;

                            System.IO.File.Copy(oldmusic, newmusic);
                            System.IO.File.Delete(oldmusic);
                        }
                        catch
                        {
                            ViewData["ValidationMessage"] = "Failed to copy file.";
                            return View(music);
                        }

                        repository.CreateMusic(music);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Music", "Upload",
                                "Added music '" + music.MusicName + "' - ID: " + music.MusicID.ToString());

                        return RedirectToAction("Index");
                    }
                }

                return View(music);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Music", "Upload POST", ex.Message);
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
            sortitem1.Text = "Music Name";
            sortitem1.Value = "MusicName";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "File Name";
            sortitem2.Value = "OriginalFilename";

            SelectListItem sortitem3 = new SelectListItem();
            sortitem3.Text = "Tags";
            sortitem3.Value = "Tags";

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

        private List<SelectListItem> BuildFileList(string currentfile)
        {
            // Build the file list
            List<SelectListItem> files = new List<SelectListItem>();

            string path = Server.MapPath(@"~/UploadedFiles");
            if (!path.EndsWith(@"\"))
                path += @"\";
            path += Convert.ToString(Session["UserAccountID"]) + @"\Music\";

            string[] imgs = Directory.GetFiles(path);
            bool first = true;
            foreach (string img in imgs)
            {
                FileInfo fi = new FileInfo(img);

                if (first)
                {
                    first = false;
                    string previewfolder = @"~/UploadedFiles/" + Convert.ToString(Session["UserAccountID"]) + @"/Music/";
                    firstfile = previewfolder + fi.Name;
                }

                SelectListItem item = new SelectListItem();
                item.Text = fi.Name;
                item.Value = fi.Name;
                if (item.Text == currentfile)
                    selectedfile = @"~/UploadedFiles/" + Convert.ToString(Session["UserAccountID"]) + @"/Music/" + fi.Name;

                files.Add(item);
            }

            if (files.Count == 0)
            {
                SelectListItem none = new SelectListItem();
                none.Text = "No music available.";
                none.Value = "0";
                files.Add(none);
                firstfile = @"~/Images/no-image-available.jpg";
                selectedfile = firstfile;
            }

            return files;
        }

        private MusicPageState GetPageState()
        {
            try
            {
                MusicPageState pagestate = new MusicPageState();


                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (Session["MusicPageState"] == null)
                {
                    int accountid = 0;
                    if (Session["UserAccountID"] != null)
                        accountid = Convert.ToInt32(Session["UserAccountID"]);

                    pagestate.AccountID = accountid;
                    pagestate.MusicName = String.Empty;
                    pagestate.Tag = String.Empty;
                    pagestate.IncludeInactive = false;
                    pagestate.SortBy = "MusicName";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    Session["MusicPageState"] = pagestate;
                }
                else
                {
                    pagestate = (MusicPageState)Session["MusicPageState"];
                }
                return pagestate;
            }
            catch { return new MusicPageState(); }
        }

        private void SavePageState(MusicPageState pagestate)
        {
            Session["MusicPageState"] = pagestate;
        }

        private Music FillNulls(Music music)
        {
            if (music.Tags == null) music.Tags = String.Empty;

            return music;
        }

        private Music CreateNewMusic()
        {
            Music music = new Music();
            music.MusicID = 0;
            music.AccountID = 0;
            music.OriginalFilename = String.Empty;
            music.StoredFilename = String.Empty;
            music.MusicName = String.Empty;
            music.Tags = String.Empty;
            music.IsActive = true;

            return music;
        }

        private string ValidateInput(Music music)
        {
            if (music.AccountID == 0)
                return "Account ID is not valid.";

            if (music.OriginalFilename == "0" || String.IsNullOrEmpty(music.OriginalFilename))
                return "You must select a valid music file.";

            if (String.IsNullOrEmpty(music.MusicName))
                return "Music Name is required.";

            if (String.IsNullOrEmpty(music.OriginalFilename))
                return "Original Filename is required.";

            if (String.IsNullOrEmpty(music.StoredFilename))
                return "Stored Filename is required.";

            return String.Empty;
        }
    }
}
