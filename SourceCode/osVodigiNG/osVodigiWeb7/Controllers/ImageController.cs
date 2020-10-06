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
    public class ImageController : AbstractVodigiController
    {
        IImageRepository repository;
        string firstfile = String.Empty;
        string selectedfile = String.Empty;

        public ImageController(IImageRepository paramrepository, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
            repository = paramrepository;
        }

        //
        // GET: /Image/

        public ActionResult Index()
        {
            try
            {
                AuthUtils.CheckAuthUser();

                // Initialize or get the page state using session
                ImagePageState pagestate = GetPageState();

                // Get the account id
                int accountid = AuthUtils.GetAccountId();

                // Set and save the page state to the submitted form values if any values are passed
                if (!String.IsNullOrEmpty(Request.Form["lstAscDesc"]))
                {
                    pagestate.AccountID = accountid;
                    pagestate.ImageName = Request.Form["txtImageName"].ToString().Trim();
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
                ViewData["ImageName"] = pagestate.ImageName;
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
                int recordcount = repository.GetImageRecordCount(pagestate.AccountID, pagestate.ImageName, pagestate.Tag, pagestate.IncludeInactive);

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
                ViewData["ImageFolder"] = @"~/Media/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Images/";

                ViewResult result = View(repository.GetImagePage(pagestate.AccountID, pagestate.ImageName, pagestate.Tag, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Image", "Index", ex);
                
            }
        }

        //
        // GET: /Image/Create

        public ActionResult Create()
        {
            try
            {
                AuthUtils.CheckAuthUser();

                ViewData["ValidationMessage"] = String.Empty;
                ViewData["FileList"] = new SelectList(BuildFileList(""), "Value", "Text", "");
                ViewData["ImageURL"] = firstfile;

                return View(CreateNewImage());
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Image", "Create", ex);
                
            }
        }

        //
        // POST: /Image/Create

        [HttpPost]
        public ActionResult Create(Image image)
        {
            try
            {
                AuthUtils.CheckAuthUser();

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    image = FillNulls(image);
                    image.AccountID = AuthUtils.GetAccountId();
                    Guid fileguid = Guid.NewGuid();

                    if (!String.IsNullOrEmpty(Request.Form["lstFile"]) && !String.IsNullOrEmpty(Request.Form["lstFile"].ToString().Trim()))
                    {
                        image.OriginalFilename = Request.Form["lstFile"].ToString().Trim();
                        if (image.OriginalFilename != "0")
                        {
                            string extension = image.OriginalFilename.Substring(image.OriginalFilename.LastIndexOf('.'));
                            image.StoredFilename = fileguid + extension;
                        }
                        else
                        {
                            image.OriginalFilename = String.Empty;
                        }
                    }
                    else
                    {
                        image.OriginalFilename = String.Empty;
                        image.StoredFilename = String.Empty;
                    }

                    string validation = ValidateInput(image);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["FileList"] = new SelectList(BuildFileList(Request.Form["lstFile"].ToString().Trim()), "Value", "Text", image.OriginalFilename);
                        ViewData["ImageURL"] = selectedfile;
                        return View(image);
                    }
                    else
                    {
                        try
                        {
                            // Move the image
                            string oldimage = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Images/" + image.OriginalFilename;
                            oldimage = GetHostFolder(oldimage);  

                            string newimage = @"~/Media/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Images/" + image.StoredFilename;
                            newimage = GetHostFolder(newimage); 

                            System.IO.File.Copy(oldimage, newimage);
                            System.IO.File.Delete(oldimage);
                        }
                        catch
                        {
                            ViewData["ValidationMessage"] = "Failed to copy file.";
                            ViewData["FileList"] = new SelectList(BuildFileList(Request.Form["lstFile"].ToString().Trim()), "Value", "Text", image.OriginalFilename);
                            ViewData["ImageURL"] = selectedfile;
                            return View(image);
                        }

                        repository.CreateImage(image);

                        CommonMethods.CreateActivityLog(HttpContext.Session.Get<User>("User"), "Image", "Add",
                                "Added image '" + image.ImageName + "' - ID: " + image.ImageID.ToString());

                        return RedirectToAction("Index");
                    }
                }

                return View(image);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Image", "Create POST", ex);
                
            }
        }

        //
        // GET: /Image/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                AuthUtils.CheckAuthUser();

                Image image = repository.GetImage(id);
                ViewData["ImageURL"] = @"~/Media/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Images/" + image.StoredFilename;
                ViewData["ValidationMessage"] = String.Empty;

                return View(image);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Image", "Edit", ex);
                
            }
        }

        //
        // POST: /Image/Edit/5

        [HttpPost]
        public ActionResult Edit(Image image)
        {
            try
            {
                AuthUtils.CheckAuthUser();

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    image = FillNulls(image);

                    string validation = ValidateInput(image);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ImageURL"] = @"~/Media/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Images/" + image.StoredFilename;
                        ViewData["ValidationMessage"] = validation;
                        return View(image);
                    }

                    repository.UpdateImage(image);

                    CommonMethods.CreateActivityLog(HttpContext.Session.Get<User>("User"), "Image", "Edit",
                            "Edited image '" + image.ImageName + "' - ID: " + image.ImageID.ToString());

                    return RedirectToAction("Index");
                }

                return View(image);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Image", "Edit POST", ex);
                
            }
        }

        //
        // GET: /Image/Upload

        public ActionResult Upload()
        {
            try
            {
                AuthUtils.CheckAuthUser();

                ViewData["ValidationMessage"] = String.Empty;

                return View(CreateNewImage());
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Image", "Upload", ex);
                
            }
        }

        //
        // POST: /Image/Upload

        [HttpPost]
        public ActionResult Upload(Image image)
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
                            if (!fname.EndsWith(".png") && !fname.EndsWith(".jpg") && !fname.EndsWith(".jpeg"))
                            {
                                ViewData["ValidationMessage"] = "Only .jpg, .jpeg, and .png image files can be uploaded.";
                            }
                            else
                            {
                                string filename = Path.GetFileName(file.FileName);
                                string serverpath = "~/UploadedFiles/" + user.AccountID.ToString() + @"/Images/" + filename;
                                
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
                        return View(image);
                    }

                    // Set NULLs to Empty Strings
                    image = FillNulls(image);
                    image.AccountID = AuthUtils.GetAccountId();
                    Guid fileguid = Guid.NewGuid();

                    image.OriginalFilename = Path.GetFileName(file.FileName);
                    string extension = image.OriginalFilename.Substring(image.OriginalFilename.LastIndexOf('.'));
                    image.StoredFilename = fileguid + extension;

                    validation = ValidateInput(image);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(image);
                    }
                    else
                    {
                        try
                        {
                            // Move the image
                            string oldimage = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Images/" + image.OriginalFilename;
                            oldimage = GetHostFolder(oldimage);

                            string newimage = @"~/Media/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Images/" + image.StoredFilename;
                            newimage = GetHostFolder(newimage);

                            System.IO.File.Copy(oldimage, newimage);
                            System.IO.File.Delete(oldimage);
                        }
                        catch
                        {
                            ViewData["ValidationMessage"] = "Failed to copy file.";
                            return View(image);
                        }

                        repository.CreateImage(image);

                        CommonMethods.CreateActivityLog(HttpContext.Session.Get<User>("User"), "Image", "Upload",
                                "Added image '" + image.ImageName + "' - ID: " + image.ImageID.ToString());

                        return RedirectToAction("Index");
                    }
                }

                return View(image);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Image", "Upload POST", ex);
                
            }
        }

        //
        // Support Methods

        private List<SelectListItem> BuildSortByList()
        {
            // Build the sort by list
            List<SelectListItem> sortitems = new List<SelectListItem>();

            SelectListItem sortitem1 = new SelectListItem();
            sortitem1.Text = "Image Name";
            sortitem1.Value = "ImageName";

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

            string path = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Images/";
            path = GetHostFolder(path);  

            string[] imgs = Directory.GetFiles(path);
            bool first = true;
            foreach (string img in imgs)
            {
                FileInfo fi = new FileInfo(img);

                if (first)
                {
                    first = false;
                    firstfile = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Images/" + fi.Name;
                }

                SelectListItem item = new SelectListItem();
                item.Text = fi.Name;
                item.Value = fi.Name;
                if (item.Text == currentfile)
                    selectedfile = @"~/UploadedFiles/" + Convert.ToString(AuthUtils.GetAccountId()) + @"/Images/" + fi.Name;

                files.Add(item);
            }

            if (files.Count == 0)
            {
                SelectListItem none = new SelectListItem();
                none.Text = "No image available.";
                none.Value = "0";
                files.Add(none);
                firstfile = @"~/Images/no-image-available.jpg";
                selectedfile = firstfile;
            }

            return files;
        }

        private ImagePageState GetPageState()
        {
            try
            {
                ImagePageState pagestate = HttpContext.Session.Get<ImagePageState>("ImagePageState");

                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (pagestate == null)
                {
                    int accountid = AuthUtils.GetAccountId();

                    pagestate = new ImagePageState();

                    pagestate.AccountID = accountid;
                    pagestate.ImageName = String.Empty;
                    pagestate.Tag = String.Empty;
                    pagestate.IncludeInactive = false;
                    pagestate.SortBy = "ImageName";
                    pagestate.AscDesc = "Ascending";
                    pagestate.PageNumber = 1;
                    SavePageState(pagestate);
                }
                return pagestate;
            }
            catch { return new ImagePageState(); }
        }

        private void SavePageState(ImagePageState pagestate)
        {
            HttpContext.Session.Set<ImagePageState>("ImagePageState", pagestate);
        }

        private Image FillNulls(Image image)
        {
            if (image.Tags == null) image.Tags = String.Empty;

            return image;
        }

        private Image CreateNewImage()
        {
            Image image = new Image
            {
                ImageID = 0,
                AccountID = 0,
                OriginalFilename = String.Empty,
                StoredFilename = String.Empty,
                ImageName = String.Empty,
                Tags = String.Empty,
                IsActive = true
            };

            return image;
        }

        private string ValidateInput(Image image)
        {
            if (image.AccountID == 0)
                return "Account ID is not valid.";

            if (image.OriginalFilename == "0" || String.IsNullOrEmpty(image.OriginalFilename))
                return "You must select a valid image.";

            if (String.IsNullOrEmpty(image.ImageName))
                return "Image Name is required.";

            if (String.IsNullOrEmpty(image.OriginalFilename))
                return "Original Filename is required.";

            if (String.IsNullOrEmpty(image.StoredFilename))
                return "Stored Filename is required.";

            return String.Empty;
        }
    }
}
