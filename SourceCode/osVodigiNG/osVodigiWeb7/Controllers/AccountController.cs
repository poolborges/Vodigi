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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using osVodigiWeb7.Extensions;
using osVodigiWeb6x.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace osVodigiWeb6x.Controllers
{
    public class AccountController : AbstractVodigiController
    {

        IAccountRepository repository;

        public AccountController(IAccountRepository paramrepository,
            IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
            : base(webHostEnvironment, configuration)
        {
            repository = paramrepository;
        }

        //
        // GET: /Account/

        public ActionResult Index()
        {
            try
            {
                AuthUtils.CheckIfAdmin();


                // Initialize or get the page state using session
                AccountPageState pagestate = GetPageState();

                // Set and save the page state to the submitted form values if any values are passed
                if (!String.IsNullOrEmpty(Request.Form["lstAscDesc"]))
                {
                    pagestate.AccountName = Request.Form["txtAccountName"].ToString().Trim();
                    pagestate.Description = Request.Form["txtDescription"].ToString().Trim();
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
                ViewData["AccountName"] = pagestate.AccountName;
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
                int recordcount = repository.GetAccountRecordCount(pagestate.AccountName, pagestate.Description, pagestate.IncludeInactive);

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

                ViewResult result = View(repository.GetAccountPage(pagestate.AccountName, pagestate.Description, pagestate.IncludeInactive, pagestate.SortBy, isdescending, pagestate.PageNumber, pagecount));
                result.ViewName = "Index";
                return result;
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Account", "Index", ex);
                
            }
        }

        //
        // GET: /Account/Create

        public ActionResult Create()
        {
            try
            {
                AuthUtils.CheckIfAdmin();

                ViewData["ValidationMessage"] = String.Empty;

                return View(CreateNewAccount());
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Account", "Create", ex);
                
            }
        }

        //
        // POST: /Account/Create

        [HttpPost]
        public ActionResult Create(Account account)
        {
            try
            {
                AuthUtils.CheckIfAdmin();

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    account = FillNulls(account);

                    string validation = ValidateInput(account);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(account);
                    }

                    repository.CreateAccount(account);
                    CreateExampleData(account.AccountID);

                    CommonMethods.CreateActivityLog(AuthUtils.CheckAuthUser(), "Account", "Add",
                                                    "Added account '" + account.AccountName + "' - ID: " + account.AccountID.ToString());

                    return RedirectToAction("Index");
                }

                return View(account);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Account", "Create POST", ex);
                
            }
        }

        //
        // GET: /Account/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                AuthUtils.CheckIfAdmin();

                Account account = repository.GetAccount(id);
                ViewData["ValidationMessage"] = String.Empty;

                return View(account);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Account", "Edit", ex);
                
            }
        }

        //
        // POST: /Account/Edit/5

        [HttpPost]
        public ActionResult Edit(Account account)
        {
            try
            {
                AuthUtils.CheckIfAdmin();

                if (ModelState.IsValid)
                {
                    // Set NULLs to Empty Strings
                    account = FillNulls(account);

                    string validation = ValidateInput(account);
                    if (!String.IsNullOrEmpty(validation))
                    {
                        ViewData["ValidationMessage"] = validation;
                        return View(account);
                    }

                    repository.UpdateAccount(account);

                    CommonMethods.CreateActivityLog(HttpContext.Session.Get<User>("User"), "Account", "Edit",
                                                    "Edited account '" + account.AccountName + "' - ID: " + account.AccountID.ToString());

                    return RedirectToAction("Index");
                }

                return View(account);
            }
            catch (Exception ex)
            {
                throw new Exceptions.AppControllerException("Account", "Edit POST", ex);
                
            }
        }

        //
        // Support Methods

        private List<SelectListItem> BuildSortByList()
        {
            // Build the sort by list
            List<SelectListItem> sortitems = new List<SelectListItem>();

            SelectListItem sortitem1 = new SelectListItem();
            sortitem1.Text = "Account Name";
            sortitem1.Value = "AccountName";

            SelectListItem sortitem2 = new SelectListItem();
            sortitem2.Text = "Description";
            sortitem2.Value = "AccountDescription";

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

        private AccountPageState GetPageState()
        {
            try
            {
                AccountPageState pagestate = HttpContext.Session.Get<AccountPageState>("AccountPageState");


                // Initialize the session values if they don't exist - need to do this the first time controller is hit
                if (pagestate == null)
                {
                    int accountid = AuthUtils.GetAccountId();
                    pagestate = new AccountPageState
                    {
                        AccountName = String.Empty,
                        Description = String.Empty,
                        IncludeInactive = false,
                        SortBy = "AccountName",
                        AscDesc = "Ascending",
                        PageNumber = 1
                    };
                    SavePageState(pagestate);
                }
                return pagestate;
            }
            catch { return new AccountPageState(); }
        }

        private void SavePageState(AccountPageState pagestate)
        {
            HttpContext.Session.Set<AccountPageState>("AccountPageState", pagestate);
        }

        private Account FillNulls(Account account)
        {
            if (account.AccountDescription == null) account.AccountDescription = String.Empty;
            if (account.FTPServer == null) account.FTPServer = String.Empty;
            if (account.FTPUsername == null) account.FTPUsername = String.Empty;
            if (account.FTPPassword == null) account.FTPPassword = String.Empty;

            return account;
        }

        private Account CreateNewAccount()
        {
            Account account = new Account();
            account.AccountID = 0;
            account.AccountName = String.Empty;
            account.AccountDescription = String.Empty;
            account.FTPServer = String.Empty;
            account.FTPUsername = String.Empty;
            account.FTPPassword = String.Empty;
            account.IsActive = true;

            return account;
        }

        private string ValidateInput(Account account)
        {
            if (String.IsNullOrEmpty(account.AccountName))
                return "Account Name is required.";

            if (!String.IsNullOrEmpty(account.FTPServer) && !account.FTPServer.ToLower().StartsWith(@"ftp://"))
                return "FTP Server must start with ftp://";

            return String.Empty;
        }

        ///////////////////////////////////////////////////////////////////////
        ///


        private void CreateExampleData(int accountid)
        {
            try
            {
                List<Image> newimages = new List<Image>();
                bool createimages = true;
                bool createvideos = true;
                bool createmusics = true;

                IImageRepository imagerep = new EntityImageRepository();
                IEnumerable<Image> images = imagerep.GetAllImages(accountid);
                foreach (Image image in images)
                {
                    if (
                            image.StoredFilename.ToLower() == "60255096-6a72-409e-b905-4d98ee717bb0.jpg".ToLower() ||
                            image.StoredFilename.ToLower() == "612bb76c-e16e-4fe8-87f2-bddc7eb59300.jpg".ToLower() ||
                            image.StoredFilename.ToLower() == "626c6a35-4523-46aa-9d0a-c2687b581e27.jpg".ToLower() ||
                            image.StoredFilename.ToLower() == "69f99c47-d1b0-4123-b62b-8f18bdc5702f.jpg".ToLower()
                        )
                    {
                        newimages.Add(image);
                        createimages = false;
                    }
                }

                IVideoRepository videorep = new EntityVideoRepository();
                IEnumerable<Video> videos = videorep.GetAllVideos(accountid);
                foreach (Video video in videos)
                {
                    if (video.StoredFilename.ToLower() == "0EBC6160-CA2C-4497-960C-0A2C2DE7B380.mp4".ToLower())
                        createvideos = false;
                }

                IMusicRepository musicrep = new EntityMusicRepository();
                IEnumerable<Music> musics = musicrep.GetAllMusics(accountid);
                foreach (Music music in musics)
                {
                    if (music.StoredFilename.ToLower() == "1B36982F-4101-4D38-AF20-FAD88A0FA9B5.mp3".ToLower())
                        createmusics = false;
                }

                // Initialize the example data for a new account so there is data available
                if (createimages)
                    newimages = CreateExampleImageAndSlideShowData(accountid); // Also creates screen, default playergroup, and schedule
                if (createvideos)
                    CreateExampleVideoAndPlayListData(accountid);
                if (createmusics)
                    CreateExampleMusicAndTimelineData(accountid, newimages);
            }
            catch { }
        }

        private List<Image> CreateExampleImageAndSlideShowData(int accountid)
        {
            try
            {
                // Copy the five images for the sample
                Image img1 = CreateExampleImage(accountid, "Visit Las Vegas Button", "vegasbutton.png", "6f5e187f-52a2-4799-bdac-2e9199580b98.png", "Las Vegas");
                Image img2 = CreateExampleImage(accountid, "Vegas 01", "vegas01.jpg", "60255096-6a72-409e-b905-4d98ee717bb0.jpg", "Las Vegas");
                Image img3 = CreateExampleImage(accountid, "Vegas 02", "vegas02.jpg", "612bb76c-e16e-4fe8-87f2-bddc7eb59300.jpg", "Las Vegas");
                Image img4 = CreateExampleImage(accountid, "Vegas 03", "vegas03.jpg", "69f99c47-d1b0-4123-b62b-8f18bdc5702f.jpg", "Las Vegas");
                Image img5 = CreateExampleImage(accountid, "Vegas 04", "vegas04.jpg", "626c6a35-4523-46aa-9d0a-c2687b581e27.jpg", "Las Vegas");

                // Create a slideshow with the images
                List<Image> images = new List<Image> { img2, img3, img4, img5 };
                SlideShow slideshow = CreateExampleSlideShow(accountid, "Visit Vegas Slideshow", "Las Vegas", images);

                // Create one screencontent item for each image
                ScreenContent sc1 = CreateExampleScreenContent(accountid, "Vegas 01 Image", "Las Vegas Is Fun!", 1000000, img2.ImageID, img2.ImageID.ToString());
                ScreenContent sc2 = CreateExampleScreenContent(accountid, "Vegas 02 Image", "Visit Las Vegas!", 1000000, img3.ImageID, img3.ImageID.ToString());
                ScreenContent sc3 = CreateExampleScreenContent(accountid, "Vegas 03 Image", "There's so much to do in Vegas!", 1000000, img4.ImageID, img4.ImageID.ToString());
                ScreenContent sc4 = CreateExampleScreenContent(accountid, "Vegas 04 Image", "Good times, day or night!", 1000000, img5.ImageID, img5.ImageID.ToString());

                // Create the screen with slideshow and four screen content items
                List<ScreenContent> screencontents = new List<ScreenContent> { sc1, sc2, sc3, sc4 };
                Screen screen = CreateExampleScreen(accountid, img1.ImageID, 0, slideshow.SlideShowID, 0, "Visit Las Vegas", screencontents);

                // Create a PlayerGroup - My Players - and Player - My Player
                PlayerGroup playergroup = CreateExamplePlayerGroup(accountid, "My Players");
                Player player = CreateExamplePlayer(accountid, playergroup.PlayerGroupID, "My Player");

                // Create the schedule for My Players player group
                CreateExamplePlayerGroupSchedule(accountid, playergroup.PlayerGroupID, screen.ScreenID);

                return images;
            }
            catch { return new List<Image>(); }
        }

        private bool CopyExampleImage(int accountid, string filename)
        {
            try
            {

                string sourceimage = GetHostFolder(@"~/ExampleImages/" + filename);
                string newimage = GetHostFolder(@"~/Media");
                if (!newimage.EndsWith(@"\"))
                    newimage += @"\";
                System.IO.Directory.CreateDirectory(newimage + Convert.ToString(accountid) + @"\Images\");
                newimage += Convert.ToString(accountid) + @"\Images\" + filename;
                if (!System.IO.File.Exists(newimage))
                    System.IO.File.Copy(sourceimage, newimage);

                return true;
                
            }
            catch { return false; }
        }

        private Image CreateExampleImage(int accountid, string imagename, string originalfilename, string storedfilename, string tags)
        {
            try
            {
                string sourceimage = GetHostFolder(@"~/ExampleImages/" + storedfilename);
                string newimage = GetHostFolder(@"~/Media");
                if (!newimage.EndsWith(@"\"))
                    newimage += @"\";
                System.IO.Directory.CreateDirectory(newimage + Convert.ToString(accountid) + @"\Images\");
                newimage += Convert.ToString(accountid) + @"\Images\" + storedfilename;

                if (!System.IO.File.Exists(newimage))
                    System.IO.File.Copy(sourceimage, newimage);

                IImageRepository imagerep = new EntityImageRepository();

                Image image = imagerep.GetImageByGuid(accountid, storedfilename);

                if (image == null || image.ImageID == 0)
                {
                    image = new Image();
                    image.AccountID = accountid;
                    image.ImageName = imagename;
                    image.OriginalFilename = originalfilename;
                    image.StoredFilename = storedfilename;
                    image.Tags = tags;
                    image.IsActive = true;
                    imagerep.CreateImage(image);
                }

                return image;
            }
            catch { return null; }
        }

        private SlideShow CreateExampleSlideShow(int accountid, string slideshowname, string tags, List<Image> images)
        {
            try
            {
                ISlideShowRepository slideshowrep = new EntitySlideShowRepository();
                SlideShow slideshow = new SlideShow();
                slideshow.AccountID = accountid;
                slideshow.SlideShowName = slideshowname;
                slideshow.Tags = tags;
                slideshow.TransitionType = "Fade";
                slideshow.IntervalInSecs = 10;
                slideshow.IsActive = true;
                slideshowrep.CreateSlideShow(slideshow);

                ISlideShowImageXrefRepository ssixrefrep = new EntitySlideShowImageXrefRepository();
                int i = 1;
                foreach (Image image in images)
                {
                    SlideShowImageXref xref = new SlideShowImageXref();
                    xref.PlayOrder = i;
                    xref.SlideShowID = slideshow.SlideShowID;
                    xref.ImageID = image.ImageID;
                    ssixrefrep.CreateSlideShowImageXref(xref);
                    i += 1;
                }

                return slideshow;
            }
            catch { return null; }
        }

        private ScreenContent CreateExampleScreenContent(int accountid, string screencontentname, string screencontenttitle, int screencontenttypeid, int thumbnailimageid, string customfield1)
        {
            try
            {
                IScreenContentRepository screencontentrep = new EntityScreenContentRepository();
                ScreenContent content = new ScreenContent();
                content.AccountID = accountid;
                content.ScreenContentName = screencontentname;
                content.ScreenContentTitle = screencontenttitle;
                content.ScreenContentTypeID = screencontenttypeid;
                content.ThumbnailImageID = thumbnailimageid;
                content.CustomField1 = customfield1;
                content.CustomField2 = String.Empty;
                content.CustomField3 = String.Empty;
                content.CustomField4 = String.Empty;
                content.IsActive = true;
                screencontentrep.CreateScreenContent(content);

                return content;
            }
            catch { return null; }
        }

        private Screen CreateExampleScreen(int accountid, int buttonimageid, int playlistid, int slideshowid, int timelineid, string screenname, List<ScreenContent> screencontents)
        {
            try
            {
                IScreenRepository screenrep = new EntityScreenRepository();
                IScreenScreenContentXrefRepository sscxrefrep = new EntityScreenScreenContentXrefRepository();

                Screen screen = new Screen();
                screen.ButtonImageID = buttonimageid;
                screen.AccountID = accountid;
                screen.IsInteractive = true;
                screen.PlayListID = playlistid;
                screen.SlideShowID = slideshowid;
                screen.TimelineID = timelineid;
                screen.ScreenName = screenname;
                screen.ScreenDescription = String.Empty;
                screen.IsActive = true;
                screenrep.CreateScreen(screen);

                int i = 1;
                foreach (ScreenContent screencontent in screencontents)
                {
                    ScreenScreenContentXref sscxref = new ScreenScreenContentXref();
                    sscxref.ScreenID = screen.ScreenID;
                    sscxref.ScreenContentID = screencontent.ScreenContentID;
                    sscxref.DisplayOrder = i;
                    sscxrefrep.CreateScreenScreenContentXref(sscxref);
                    i += 1;
                }

                return screen;
            }
            catch { return null; }
        }

        private PlayerGroup CreateExamplePlayerGroup(int accountid, string playergroupname)
        {
            try
            {
                IPlayerGroupRepository playergrouprep = new EntityPlayerGroupRepository();

                PlayerGroup playergroup = new PlayerGroup();
                playergroup.AccountID = accountid;
                playergroup.PlayerGroupName = playergroupname;
                playergroup.PlayerGroupDescription = String.Empty;
                playergroup.IsActive = true;
                playergrouprep.CreatePlayerGroup(playergroup);

                return playergroup;
            }
            catch { return null; }
        }

        private Player CreateExamplePlayer(int accountid, int playergroupid, string playername)
        {
            try
            {
                IPlayerRepository playerrep = new EntityPlayerRepository();

                Player player = new Player();
                player.AccountID = accountid;
                player.PlayerGroupID = playergroupid;
                player.PlayerName = playername;
                player.PlayerLocation = String.Empty;
                player.PlayerDescription = String.Empty;
                player.IsActive = true;
                playerrep.CreatePlayer(player);

                return player;
            }
            catch { return null; }
        }

        private void CreateExamplePlayerGroupSchedule(int account, int playergroupid, int screenid)
        {
            try
            {
                IPlayerGroupScheduleRepository schedulerep = new EntityPlayerGroupScheduleRepository();
                for (int i = 0; i < 7; i += 1)
                {
                    PlayerGroupSchedule schedule = new PlayerGroupSchedule();
                    schedule.PlayerGroupID = playergroupid;
                    schedule.ScreenID = screenid;
                    schedule.Day = i;
                    schedule.Hour = 0;
                    schedule.Minute = 0;
                    schedulerep.CreatePlayerGroupSchedule(schedule);
                }
            }
            catch { }
        }

        public void CreateExampleVideoAndPlayListData(int accountid)
        {
            try
            {
                // Copy the video for the example
                Video video = CreateExampleVideo(accountid, "Countdown Video", "countdown.mp4", "0EBC6160-CA2C-4497-960C-0A2C2DE7B380.mp4", "Example Video");

                // Create a playlist with the video
                List<Video> videos = new List<Video> { video };
                PlayList playlist = CreateExamplePlayList(accountid, "Example Video PlayList", "Examples", videos);
            }
            catch { }
        }

        private Video CreateExampleVideo(int accountid, string videoname, string originalfilename, string storedfilename, string tags)
        {
            try
            {
                string sourcevideo = GetHostFolder(@"~/ExampleVideos/" + storedfilename);
                string newvideo = GetHostFolder(@"~/Media");
                if (!newvideo.EndsWith(@"\"))
                    newvideo += @"\";
                System.IO.Directory.CreateDirectory(newvideo + Convert.ToString(accountid) + @"\Videos\");
                newvideo += Convert.ToString(accountid) + @"\Videos\" + storedfilename;

                if (!System.IO.File.Exists(newvideo))
                    System.IO.File.Copy(sourcevideo, newvideo);

                IVideoRepository videorep = new EntityVideoRepository();

                Video video = videorep.GetVideoByGuid(accountid, storedfilename);

                if (video == null || video.VideoID == 0)
                {
                    video = new Video();
                    video.AccountID = accountid;
                    video.VideoName = videoname;
                    video.OriginalFilename = originalfilename;
                    video.StoredFilename = storedfilename;
                    video.Tags = tags;
                    video.IsActive = true;
                    videorep.CreateVideo(video);
                }

                return video;
            }
            catch { return null; }
        }

        private PlayList CreateExamplePlayList(int accountid, string playlistname, string tags, List<Video> videos)
        {
            try
            {
                IPlayListRepository playlistrep = new EntityPlayListRepository();

                PlayList playlist = new PlayList();
                playlist.AccountID = accountid;
                playlist.PlayListName = playlistname;
                playlist.Tags = tags;
                playlist.IsActive = true;
                playlistrep.CreatePlayList(playlist);

                IPlayListVideoXrefRepository plvxrefrep = new EntityPlayListVideoXrefRepository();
                int i = 1;
                foreach (Video video in videos)
                {
                    PlayListVideoXref xref = new PlayListVideoXref();
                    xref.PlayOrder = i;
                    xref.PlayListID = playlist.PlayListID;
                    xref.VideoID = video.VideoID;
                    plvxrefrep.CreatePlayListVideoXref(xref);
                    i += 1;
                }

                return playlist;
            }
            catch { return null; }
        }

        public void CreateExampleMusicAndTimelineData(int accountid, List<Image> images)
        {
            try
            {
                // Copy the music for the example
                Music music1 = CreateExampleMusic(accountid, "Music Example 1", "musicexample1.mp3", "1B36982F-4101-4D38-AF20-FAD88A0FA9B5.mp3", "Example Music");
                Music music2 = CreateExampleMusic(accountid, "Music Example 2", "musicexample2.mp3", "ADA2DBFA-D8D9-49A8-8370-8329A830667E.mp3", "Example Music");
                Music music3 = CreateExampleMusic(accountid, "Music Example 3", "musicexample3.mp3", "E4B660F0-ACD3-44F1-92EE-FA23110BE5C6.mp3", "Example Music");
                List<Music> musics = new List<Music> { music1, music2, music3 };

                // Create the example timeline
                ITimelineRepository tlrep = new EntityTimelineRepository();
                Timeline timeline = new Timeline();
                timeline.AccountID = accountid;
                timeline.TimelineName = "Timeline Example";
                timeline.Tags = "Examples";
                timeline.IsActive = true;
                timeline.MuteMusicOnPlayback = true;
                timeline.DurationInSecs = 10;
                tlrep.CreateTimeline(timeline);

                // Create the timeline music xrefs
                ITimelineMusicXrefRepository mxrefrep = new EntityTimelineMusicXrefRepository();
                int i = 1;
                foreach (Music music in musics)
                {
                    TimelineMusicXref mxref = new TimelineMusicXref();
                    mxref.TimelineID = timeline.TimelineID;
                    mxref.MusicID = music.MusicID;
                    mxref.PlayOrder = i;
                    mxrefrep.CreateTimelineMusicXref(mxref);
                    i = +1;
                }

                // Create the timeline image xrefs
                ITimelineImageXrefRepository ixrefrep = new EntityTimelineImageXrefRepository();
                i = 1;
                foreach (Image image in images)
                {
                    TimelineImageXref ixref = new TimelineImageXref();
                    ixref.TimelineID = timeline.TimelineID;
                    ixref.ImageID = image.ImageID;
                    ixref.DisplayOrder = i;
                    ixrefrep.CreateTimelineImageXref(ixref);
                    i = +1;
                }

            }
            catch { }
        }

        private Music CreateExampleMusic(int accountid, string musicname, string originalfilename, string storedfilename, string tags)
        {
            try
            {
                string sourcemusic = GetHostFolder(@"~/ExampleMusic/" + storedfilename);
                string newmusic = GetHostFolder(@"~/Media");
                if (!newmusic.EndsWith(@"\"))
                    newmusic += @"\";
                System.IO.Directory.CreateDirectory(newmusic + Convert.ToString(accountid) + @"\Music\");
                newmusic += Convert.ToString(accountid) + @"\Music\" + storedfilename;

                if (!System.IO.File.Exists(newmusic))
                    System.IO.File.Copy(sourcemusic, newmusic);

                IMusicRepository musicrep = new EntityMusicRepository();

                Music music = musicrep.GetMusicByGuid(accountid, storedfilename);

                if (music == null || music.MusicID == 0)
                {
                    music = new Music();
                    music.AccountID = accountid;
                    music.MusicName = musicname;
                    music.OriginalFilename = originalfilename;
                    music.StoredFilename = storedfilename;
                    music.Tags = tags;
                    music.IsActive = true;
                    musicrep.CreateMusic(music);
                }

                return music;
            }
            catch { return null; }
        }



    }
}