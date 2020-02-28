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
    public class ScreenWizardController : Controller
    {
        IScreenRepository repository;
        string firstfile = String.Empty;
        string selectedfile = String.Empty;

        public ScreenWizardController()
            : this(new EntityScreenRepository())
        { }

        public ScreenWizardController(IScreenRepository paramrepository)
        {
            repository = paramrepository;
        }


        //
        // GET: /ScreenWizard/Step1/5

        public ActionResult Step1(int id)
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

                Screen screen = CreateNewScreen();
                if (id > 0)
                    screen = repository.GetScreen(id);

                string mainfeaturetype = String.Empty;
                if (screen.SlideShowID != 0)
                    mainfeaturetype = "Slide Show";
                else if (screen.PlayListID != 0)
                    mainfeaturetype = "Play List";
                else if (screen.TimelineID != 0)
                    mainfeaturetype = "Media Timeline";

                ViewData["ValidationMessage"] = String.Empty;
                ViewData["MainFeatureTypeList"] = new SelectList(BuildMainFeatureTypeList(), "Value", "Text", mainfeaturetype);

                return View(screen);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenWizard", "Step 1", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /ScreenWizard/Step1/5

        [HttpPost]
        public ActionResult Step1(Screen screen)
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

                    string validation = ValidateInput(screen);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        ViewData["MainFeatureTypeList"] = new SelectList(BuildMainFeatureTypeList(), "Value", "Text", Request.Form["lstMainFeatureType"]);

                        return View(screen);
                    }
                    else
                    {
                        string mainfeaturetype = Request.Form["lstMainFeatureType"];

                        if (mainfeaturetype == "Slide Show")
                        {
                            if (screen.SlideShowID <= 0)
                                screen.SlideShowID = -1;
                            screen.PlayListID = 0;
                            screen.TimelineID = 0;
                        }
                        else if (mainfeaturetype == "Play List")
                        {
                            screen.SlideShowID = 0;
                            if (screen.PlayListID <= 0)
                                screen.PlayListID = -1;
                            screen.TimelineID = 0;
                        }
                        else if (mainfeaturetype == "Media Timeline")
                        {
                            screen.SlideShowID = 0;
                            screen.PlayListID = 0;
                            if (screen.TimelineID <= 0)
                                screen.TimelineID = -1;
                        }

                        if (screen.ScreenID == 0)
                            repository.CreateScreen(screen);
                        else
                            repository.UpdateScreen(screen);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Screen", "Wizard Step 1",
                            "Step 1 '" + screen.ScreenName + "' - ID: " + screen.ScreenID.ToString());

                        return RedirectToAction("Step2", new { id = screen.ScreenID });
                    }
                }

                return View(screen);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenWizard", "Step 1 POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /ScreenWizard/Step2/5

        public ActionResult Step2(int id)
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
                string mainfeaturetype = String.Empty;
                if (screen.SlideShowID != 0)
                    mainfeaturetype = "Slide Show";
                else if (screen.PlayListID != 0)
                    mainfeaturetype = "Play List";
                else if (screen.TimelineID != 0)
                    mainfeaturetype = "Media Timeline";

                if (screen.IsInteractive)
                    ViewData["ButtonName"] = "Next >";
                else
                    ViewData["ButtonName"] = "Finish";

                ViewData["ValidationMessage"] = String.Empty;
                ViewData["ScreenID"] = id;
                ViewData["MainFeatureType"] = mainfeaturetype;
                ViewData["SlideShowList"] = new SelectList(BuildSlideShowList(), "Value", "Text", screen.SlideShowID);
                ViewData["PlayListList"] = new SelectList(BuildPlayListList(), "Value", "Text", screen.PlayListID);
                ViewData["TimelineList"] = new SelectList(BuildTimelineList(), "Value", "Text", screen.TimelineID);

                return View(screen);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenWizard", "Step 2", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        [HttpPost]
        public ActionResult Step2(Screen screen)
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

                    string mainfeaturetype = Request.Form["txtMainFeatureType"];
                    screen.SlideShowID = 0;
                    screen.PlayListID = 0;
                    screen.TimelineID = 0;
                    if (mainfeaturetype == "Slide Show")
                        screen.SlideShowID = Convert.ToInt32(Request.Form["lstSlideShow"]);
                    else if (mainfeaturetype == "Play List")
                        screen.PlayListID = Convert.ToInt32(Request.Form["lstPlayList"]);
                    else if (mainfeaturetype == "Media Timeline")
                        screen.TimelineID = Convert.ToInt32(Request.Form["lstTimeline"]);

                    string validation = ValidateInput(screen);
                    if (screen.SlideShowID < 1 && screen.PlayListID < 1 && screen.TimelineID < 1)
                        validation = "Please select a valid item from the list";
                    if (!String.IsNullOrEmpty(validation))
                    {
                        if (screen.IsInteractive)
                            ViewData["ButtonName"] = "Next >";
                        else
                            ViewData["ButtonName"] = "Finish";

                        ViewData["ValidationMessage"] = validation;
                        ViewData["ScreenID"] = screen.ScreenID;
                        ViewData["MainFeatureTypeList"] = Request.Form["MainFeatureType"];
                        ViewData["SlideShowList"] = new SelectList(BuildSlideShowList(), "Value", "Text", screen.SlideShowID);
                        ViewData["PlayListList"] = new SelectList(BuildPlayListList(), "Value", "Text", screen.PlayListID);
                        ViewData["TimelineList"] = new SelectList(BuildTimelineList(), "Value", "Text", screen.TimelineID);

                        return View(screen);
                    }
                    else
                    {
                        repository.UpdateScreen(screen);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Screen", "Wizard Step 2",
                            "Step 2 '" + screen.ScreenName + "' - ID: " + screen.ScreenID.ToString());

                        if (screen.IsInteractive)
                            return RedirectToAction("Step3", new { id = screen.ScreenID });
                        else
                            return RedirectToAction("Index", "Screen");
                    }
                }

                return View(screen);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenWizard", "Step 2 POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /ScreenWizard/Step3/5

        public ActionResult Step3(int id)
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
                IImageRepository imgrep = new EntityImageRepository();
                Image image = imgrep.GetImage(screen.ButtonImageID);
                if (image == null)
                    ViewData["ImageList"] = new SelectList(BuildImageList(""), "Value", "Text", "");
                else
                    ViewData["ImageList"] = new SelectList(BuildImageList(image.StoredFilename), "Value", "Text", image.StoredFilename);
                if (!String.IsNullOrEmpty(selectedfile))
                    ViewData["ImageUrl"] = selectedfile;
                else
                    ViewData["ImageUrl"] = firstfile;
                ViewData["ValidationMessage"] = String.Empty;
                ViewData["ScreenID"] = id;
                
                int accountid = 0;
                if (Session["UserAccountID"] != null)
                    accountid = Convert.ToInt32(Session["UserAccountID"]);
                ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                return View(screen);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenWizard", "Step 3", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        [HttpPost]
        public ActionResult Step3(Screen screen)
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
                        ViewData["ScreenID"] = screen.ScreenID;
                        if (img == null)
                            ViewData["ImageList"] = new SelectList(BuildImageList(""), "Value", "Text", "");
                        else
                            ViewData["ImageList"] = new SelectList(BuildImageList(img.StoredFilename), "Value", "Text", img.StoredFilename);
                        if (!String.IsNullOrEmpty(selectedfile))
                            ViewData["ImageUrl"] = selectedfile;
                        else
                            ViewData["ImageUrl"] = firstfile;

                        int accountid = 0;
                        if (Session["UserAccountID"] != null)
                            accountid = Convert.ToInt32(Session["UserAccountID"]);
                        ViewData["ImageFolder"] = ConfigurationManager.AppSettings["MediaRootFolder"] + Convert.ToString(Session["UserAccountID"]) + @"/Images/";

                        return View(screen);
                    }
                    else
                    {
                        repository.UpdateScreen(screen);

                        CommonMethods.CreateActivityLog((User)Session["User"], "Screen", "Wizard Step 3",
                            "Step 3 '" + screen.ScreenName + "' - ID: " + screen.ScreenID.ToString());

                        if (screen.IsInteractive)
                            return RedirectToAction("Step4", new { id = screen.ScreenID });
                        else
                            return RedirectToAction("Index", "Screen");
                    }
                }

                return View(screen);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenWizard", "Step 3 POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // GET: /ScreenWizard/Step4/5

        public ActionResult Step4(int id)
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
                ViewData["ScreenContentList"] = new SelectList(BuildScreenContentList(), "Value", "Text", "");
                ViewData["ScreenScreenContentList"] = new SelectList(BuildScreenScreenContentList(screen.ScreenID), "Value", "Text", "");
                ViewData["ScreenID"] = id;

                // Get the content ids for the screen
                string ids = String.Empty;
                IScreenScreenContentXrefRepository sscrep = new EntityScreenScreenContentXrefRepository();
                IEnumerable<ScreenScreenContentXref> sscs = sscrep.GetScreenScreenContentXrefs(screen.ScreenID);
                foreach (ScreenScreenContentXref ssc in sscs)
                {
                    ids += "|" + ssc.ScreenContentID.ToString();
                }
                ViewData["ScreenScreenContent"] = ids;

                return View(screen);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenWizard", "Step 3", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        [HttpPost]
        public ActionResult Step4(Screen screen)
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

                    if (String.IsNullOrEmpty(Request.Form["txtScreenScreenContent"]))
                    {
                        ViewData["ScreenScreenContent"] = String.Empty;
                        ViewData["ScreenContentList"] = new SelectList(BuildScreenContentList(), "Value", "Text", "");
                        ViewData["ScreenScreenContentList"] = new SelectList(BuildScreenScreenContentList(screen.ScreenID), "Value", "Text", "");
                        ViewData["ScreenID"] = screen.ScreenID;
                        ViewData["ValidationMessage"] = "Please select one or more screen content items";
                        return View(screen);
                    }
                    else
                    {
                        // Delete and recreate the screen/screen content xrefs
                        IScreenScreenContentXrefRepository xrefrep = new EntityScreenScreenContentXrefRepository();
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

                        CommonMethods.CreateActivityLog((User)Session["User"], "Screen", "Wizard Step 4",
                            "Step 4 '" + screen.ScreenName + "' - ID: " + screen.ScreenID.ToString());

                        return RedirectToAction("Index", "Screen");
                    }
                }

                return View(screen);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("ScreenWizard", "Step 3 POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        private List<SelectListItem> BuildMainFeatureTypeList()
        {
            // Build the list
            List<SelectListItem> items = new List<SelectListItem>();

            SelectListItem item1 = new SelectListItem();
            item1.Text = "Slide Show";
            item1.Value = "Slide Show";

            SelectListItem item2 = new SelectListItem();
            item2.Text = "Play List";
            item2.Value = "Play List";

            SelectListItem item3 = new SelectListItem();
            item3.Text = "Media Timeline";
            item3.Value = "Media Timeline";

            items.Add(item1);
            items.Add(item2);
            items.Add(item3);

            return items;
        }

        private Screen FillNulls(Screen screen)
        {
            if (screen.ScreenDescription == null) screen.ScreenDescription = String.Empty;

            return screen;
        }

        private string ValidateInput(Screen screen)
        {
            if (screen.AccountID == 0)
                return "Account ID is not valid.";

            if (String.IsNullOrEmpty(screen.ScreenName))
                return "Screen Name is required.";

            return String.Empty;
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

        private List<SelectListItem> BuildTimelineList()
        {
            // Get the account id
            int accountid = 0;
            if (Session["UserAccountID"] != null)
                accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Build the timeline list
            List<SelectListItem> items = new List<SelectListItem>();

            ITimelineRepository tlrep = new EntityTimelineRepository();
            IEnumerable<Timeline> tls = tlrep.GetAllTimelines(accountid);
            foreach (Timeline tl in tls)
            {
                SelectListItem item = new SelectListItem();
                item.Text = tl.TimelineName;
                item.Value = tl.TimelineID.ToString();
                items.Add(item);
            }

            if (items.Count == 0)
            {
                SelectListItem item = new SelectListItem();
                item.Text = "No media timelines available";
                item.Value = "0";
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

            ISlideShowRepository ssrep = new EntitySlideShowRepository();
            IEnumerable<SlideShow> sss = ssrep.GetAllSlideShows(accountid);
            foreach (SlideShow ss in sss)
            {
                SelectListItem item = new SelectListItem();
                item.Text = ss.SlideShowName;
                item.Value = ss.SlideShowID.ToString();
                items.Add(item);
            }

            if (items.Count == 0)
            {
                SelectListItem item = new SelectListItem();
                item.Text = "No slide shows available";
                item.Value = "0";
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

            IPlayListRepository plrep = new EntityPlayListRepository();
            IEnumerable<PlayList> pls = plrep.GetAllPlayLists(accountid);
            foreach (PlayList pl in pls)
            {
                SelectListItem item = new SelectListItem();
                item.Text = pl.PlayListName;
                item.Value = pl.PlayListID.ToString();
                items.Add(item);
            }

            if (items.Count == 0)
            {
                SelectListItem item = new SelectListItem();
                item.Text = "No play lists available";
                item.Value = "0";
                items.Add(item);
            }

            return items;
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

    }
}
