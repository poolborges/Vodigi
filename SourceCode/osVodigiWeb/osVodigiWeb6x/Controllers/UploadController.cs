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
using System.Web.Mvc;
using System.IO;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class UploadController : Controller
    {
        //
        // GET: /Upload/

        public ActionResult Index()
        {
            try
            {
                User user = AuthUtils.CheckAuthUser();

                ViewData["UploadMessage"] = String.Empty;
                ViewData["ImageFolder"] = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Images/";
                ViewData["VideoFolder"] = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Videos/";
                ViewData["MusicFolder"] = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Music/";

                if (Request.Files.Count > 0)
                {
                    // Only one file is allowed per upload
                    HttpPostedFileBase file = Request.Files[0];
                    if (file != null)
                    {
                        if (file.ContentLength > 208000000)
                        {
                            ViewData["UploadMessage"] = "Uploaded files must be 200MB or less.";
                        }
                        else
                        {
                            string fname = file.FileName.ToLower();
                            if (!fname.EndsWith(".png") && !fname.EndsWith(".jpg") && !fname.EndsWith(".jpeg") &&
                                !fname.EndsWith(".wmv") && !fname.EndsWith(".mp4") && !fname.EndsWith(".mp3") &&
                                !fname.EndsWith(".wma"))
                            {
                                ViewData["UploadMessage"] = "Only media files of the types listed above can be uploaded.";
                            }
                            else
                            {
                                string filetype = "Images";
                                string filename = Path.GetFileName(file.FileName);
                                if (filename.ToLower().EndsWith(".wmv") || filename.ToLower().EndsWith(".mp4"))
                                    filetype = "Videos";
                                else if (filename.ToLower().EndsWith(".wma") || filename.ToLower().EndsWith(".mp3"))
                                    filetype = "Music";
                                string serverpath = "~/UploadedFiles/" + user.AccountID.ToString() + @"/" + filetype + @"/" + filename;
                                string path = Server.MapPath(serverpath);
                                if (!System.IO.File.Exists(path))
                                    file.SaveAs(path);
                                else
                                    ViewData["UploadMessage"] = "A file already exists with this name.";
                            }
                        }
                    }
                }

                List<Upload> uploads = GetUploads();

                return View(uploads);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Upload", "Index", ex);
                
            }
        }


        //
        // GET: /Upload/Delete/Filename

        public ActionResult Delete(string filename, string filetype)
        {
            User user = AuthUtils.CheckAuthUser();
            IUploadRepository uploadrep = new IOUploadRepository();

            if (filetype == "Image")
                uploadrep.DeleteImageUpload(AuthUtils.GetAccountId(), filename, Server.MapPath(@"~/UploadedFiles"));
            else if (filetype == "Video")
                uploadrep.DeleteVideoUpload(AuthUtils.GetAccountId(), filename, Server.MapPath(@"~/UploadedFiles"));
            else
                uploadrep.DeleteMusicUpload(AuthUtils.GetAccountId(), filename, Server.MapPath(@"~/UploadedFiles"));

            return RedirectToAction("Index");
        }

        private List<Upload> GetUploads()
        {
            IUploadRepository uploadrep = new IOUploadRepository();

            User user = AuthUtils.CheckAuthUser();

            List<Upload> images = uploadrep.GetImageUploads(user.AccountID, Server.MapPath(@"~/UploadedFiles")).ToList();
            List<Upload> videos = uploadrep.GetVideoUploads(user.AccountID, Server.MapPath(@"~/UploadedFiles")).ToList();
            List<Upload> musics = uploadrep.GetMusicUploads(user.AccountID, Server.MapPath(@"~/UploadedFiles")).ToList();

            List<Upload> uploads = new List<Upload>();
            foreach (Upload image in images)
            {
                uploads.Add(image);
            }
            foreach (Upload video in videos)
            {
                uploads.Add(video);
            }
            foreach (Upload music in musics)
            {
                uploads.Add(music);
            }

            return uploads;
        }
    }
}
