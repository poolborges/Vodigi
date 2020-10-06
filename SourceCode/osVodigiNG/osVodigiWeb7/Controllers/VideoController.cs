﻿/* ----------------------------------------------------------------------------------------
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
using System.IO;
using osVodigiWeb6x.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace osVodigiWeb6x.Controllers
{
    public class VideoController : AbstractVodigiController
    {
        IVideoRepository repository;
        string firstfile = String.Empty;
        string selectedfile = String.Empty;


        public VideoController(IVideoRepository paramrepository, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
            repository = paramrepository;
        }

        //
        // GET: /Video/

        public ActionResult Index()
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                // Initialize or get the page state using session
                VideoPageState pagestate = GetPageState();

                // Get the account id
                int accountid = AuthUtils.GetAccountId();

                // Set and save the page state to the submitted form values if any values are passed
                if (!String.IsNullOrEmpty(Request.Form["lstAscDesc"]))
                {
                    pagestate.AccountID = accountid;
                    pagestate.VideoName = Request.Form["txtVideoName"].ToString().Trim();
                    pagestate.Tag = Request.Form["txtTag"].ToString().Trim();
                    if (Request.Form["chkIncludeInactive"].ToString().ToLower().StartsWith("true"))
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
                ViewData["VideoName"] = pagestate.VideoName;
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
                int recordcount = repository.GetVideoRecordCount(pagestate.AccountID, pagestate.VideoName, pagestate.Tag, pagestate.IncludeInactive);

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

                // Set the Video folder 
                ViewData["VideoFolder"] = @"~/Media/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/";

                ViewResult result = View(repository.GetVideoPage(pagestate.AccountID, pagestate.VideoName, pagestate.Tag, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Video", "Index", ex);
                
            }
        }

        //
        // GET: /Video/Create

        public ActionResult Create()
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                ViewData["ValidationMessage"] = String.Empty;
                ViewData["FileList"] = new SelectList(BuildFileList(""), "Value", "Text", "");
                ViewData["VideoURL"] = firstfile;

                return View(CreateNewVideo());
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Video", "Create", ex);
                
            }
        }

        //
        // POST: /Video/Create

        [HttpPost]
        public ActionResult Create(Video video)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    video = FillNulls(video);
                    video.AccountID = AuthUtils.GetAccountId();
                    Guid fileguid = Guid.NewGuid();

                    if (!String.IsNullOrEmpty(Request.Form["lstFile"]) && !String.IsNullOrEmpty(Request.Form["lstFile"].ToString().Trim()))
                    {
                        video.OriginalFilename = Request.Form["lstFile"].ToString().Trim();
                        if (video.OriginalFilename != "0")
                        {
                            string extension = video.OriginalFilename.Substring(video.OriginalFilename.LastIndexOf('.'));
                            video.StoredFilename = fileguid + extension;
                        }
                        else
                        {
                            video.OriginalFilename = String.Empty;
                        }
                    }
                    else
                    {
                        video.OriginalFilename = String.Empty;
                        video.StoredFilename = String.Empty;
                    }

                    string validation = ValidateInput(video);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["FileList"] = new SelectList(BuildFileList(Request.Form["lstFile"].ToString().Trim()), "Value", "Text", video.OriginalFilename);
                        ViewData["VideoURL"] = selectedfile;
                        return View(video);
                    }
                    else
                    {
                        try
                        {
                            // Move the video
                            string oldvideo = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/" + video.OriginalFilename; ;
                            oldvideo = GetHostFolder(oldvideo);

                            string newvideo =  @"~/Media/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/" + video.StoredFilename;
                            newvideo = GetHostFolder(newvideo);

                            System.IO.File.Copy(oldvideo, newvideo);
                            System.IO.File.Delete(oldvideo);
                        }
                        catch
                        {
                            ViewData["ValidationMessage"] = "Failed to copy file.";
                            ViewData["FileList"] = new SelectList(BuildFileList(Request.Form["lstFile"].ToString().Trim()), "Value", "Text", video.OriginalFilename);
                            ViewData["VideoURL"] = selectedfile;
                            return View(video);
                        }

                        repository.CreateVideo(video);

                        CommonMethods.CreateActivityLog(HttpContext.Session.Get<User>("User"), "Video", "Add",
                            "Added video '" + video.VideoName + "' - ID: " + video.VideoID.ToString());

                        return RedirectToAction("Index");
                    }
                }

                return View(video);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Video", "Create POST", ex);
                
            }
        }

        //
        // GET: /Video/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                Video video = repository.GetVideo(id);
                ViewData["VideoURL"] = @"~/Media/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/" + video.StoredFilename;
                ViewData["ValidationMessage"] = String.Empty;

                return View(video);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Video", "Edit", ex);
                
            }
        }

        //
        // POST: /Video/Edit/5

        [HttpPost]
        public ActionResult Edit(Video video)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    video = FillNulls(video);

                    string validation = ValidateInput(video);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["VideoURL"] = @"~/Media/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/" + video.StoredFilename;
                        ViewData["ValidationMessage"] = validation;
                        return View(video);
                    }

                    repository.UpdateVideo(video);

                    CommonMethods.CreateActivityLog(HttpContext.Session.Get<User>("User"), "Video", "Edit",
                        "Edited video '" + video.VideoName + "' - ID: " + video.VideoID.ToString());

                    return RedirectToAction("Index");
                }

                return View(video);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Video", "Edit POST", ex);
                
            }
        }


        //
        // GET: /Video/Upload

        public ActionResult Upload()
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                ViewData["ValidationMessage"] = String.Empty;

                return View(CreateNewVideo());
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Video", "Upload", ex);
                
            }
        }

        //
        // POST: /Video/Upload

        [HttpPost]
        public ActionResult Upload(Video video)
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                string validation = String.Empty;
                if (ModelState.IsValid)
                {
                    // Only one file is allowed per upload
                    var file = Request.Form.Files[0];
                    if (file != null && file.Length > 0 && !String.IsNullOrEmpty(file.FileName))
                    {
                        if (file.Length > 208000000)
                        {
                            ViewData["ValidationMessage"] = "Uploaded files must be 200MB or less.";
                        }
                        else
                        {
                            string fname = file.FileName.ToLower();
                            if (!fname.EndsWith(".mp4") && !fname.EndsWith(".wmv"))
                            {
                                ViewData["ValidationMessage"] = "Only .wmv, and .mp4 video files can be uploaded.";
                            }
                            else
                            {
                                string filetype = "Videos";
                                string filename = Path.GetFileName(file.FileName);
                                string serverpath =  "~/UploadedFiles/" + user.AccountID.ToString() + @"/" + filetype + @"/" + filename;
                                string path = GetHostFolder(serverpath);
                                if (!System.IO.File.Exists(path))
                                    System.IO.File.Create(path);

                                using (var stream = System.IO.File.Open(path, FileMode.Create))
                                {
                                    file.CopyToAsync(stream);
                                }
                            }
                        }
                    }
                    else
                    {
                        ViewData["ValidationMessage"] = "You must select a file.";
                        return View(video);
                    }

                    // Set NULLs to Empty Strings
                    video = FillNulls(video);
                    video.AccountID = AuthUtils.GetAccountId();
                    Guid fileguid = Guid.NewGuid();

                    video.OriginalFilename = Path.GetFileName(file.FileName);
                    string extension = video.OriginalFilename.Substring(video.OriginalFilename.LastIndexOf('.'));
                    video.StoredFilename = fileguid + extension;

                    validation = ValidateInput(video);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(video);
                    }
                    else
                    {
                        try
                        {
                            // Move the video
                            string oldvideo = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/" + video.OriginalFilename;
                            oldvideo = GetHostFolder(oldvideo);

                            string newvideo = @"~/Media/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/" + video.StoredFilename;
                            newvideo = GetHostFolder(newvideo); 

                            System.IO.File.Copy(oldvideo, newvideo);
                            System.IO.File.Delete(oldvideo);
                        }
                        catch
                        {
                            ViewData["ValidationMessage"] = "Failed to copy file.";
                            return View(video);
                        }

                        repository.CreateVideo(video);

                        CommonMethods.CreateActivityLog(HttpContext.Session.Get<User>("User"), "Video", "Upload",
                                "Added video '" + video.VideoName + "' - ID: " + video.VideoID.ToString());

                        return RedirectToAction("Index");
                    }
                }

                return View(video);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Video", "Upload POST", ex);
                
            }
        }

        //
        // Support Methods

        private List<SelectListItem> BuildSortByList()
        {
            // Build the sort by list
            List<SelectListItem> sortitems = new List<SelectListItem>();

            SelectListItem sortitem1 = new SelectListItem();
            sortitem1.Text = "Video Name";
            sortitem1.Value = "VideoName";

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

            string path = @"~/UploadedFiles/"+ Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/";
            path = GetHostFolder(path);

            string[] vids = Directory.GetFiles(path);
            bool first = true;
            foreach (string vid in vids)
            {
                FileInfo fi = new FileInfo(vid);

                if (first)
                {
                    first = false;
                    string previewfolder = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/";
                    firstfile = previewfolder + fi.Name;
                }

                SelectListItem item = new SelectListItem();
                item.Text = fi.Name;
                item.Value = fi.Name;
                if (item.Text == currentfile)
                    selectedfile = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/" + fi.Name;

                files.Add(item);
            }

            if (files.Count == 0)
            {
                SelectListItem none = new SelectListItem();
                none.Text = "No video available.";
                none.Value = "0";
                files.Add(none);
                firstfile = @"~/Videos/no-image-available.jpg";
                selectedfile = firstfile;
            }

            return files;
        }

        private VideoPageState GetPageState()
        {
            try
            {
                VideoPageState pagestate = HttpContext.Session.Get<VideoPageState>("VideoPageState");


                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (pagestate == null)
                {
                    int accountid = AuthUtils.GetAccountId();
                    pagestate = new VideoPageState
                    {
                        AccountID = accountid,
                        VideoName = String.Empty,
                        Tag = String.Empty,
                        IncludeInactive = false,
                        SortBy = "VideoName",
                        AscDesc = "Ascending",
                        PageNumber = 1
                    };
                    SavePageState(pagestate);
                }
                return pagestate;
            }
            catch { return new VideoPageState(); }
        }

        private void SavePageState(VideoPageState pagestate)
        {
            HttpContext.Session.Set<VideoPageState>("VideoPageState", pagestate);
        }

        private Video FillNulls(Video video)
        {
            if (video.Tags == null) video.Tags = String.Empty;

            return video;
        }

        private Video CreateNewVideo()
        {
            Video video = new Video();
            video.VideoID = 0;
            video.AccountID = 0;
            video.OriginalFilename = String.Empty;
            video.StoredFilename = String.Empty;
            video.VideoName = String.Empty;
            video.Tags = String.Empty;
            video.IsActive = true;

            return video;
        }

        private string ValidateInput(Video video)
        {
            if (video.AccountID == 0)
                return "Account ID is not valid.";

            if (video.OriginalFilename == "0" || String.IsNullOrEmpty(video.OriginalFilename))
                return "You must select a valid video.";

            if (String.IsNullOrEmpty(video.VideoName))
                return "Video Name is required.";

            if (String.IsNullOrEmpty(video.OriginalFilename))
                return "Original Filename is required.";

            if (String.IsNullOrEmpty(video.StoredFilename))
                return "Stored Filename is required.";

            return String.Empty;
        }
    }
}
