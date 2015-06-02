using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Linq;
using System.Drawing;
using System.IO;
using System.Windows.Media.Animation;

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

namespace osVodigiPlayer
{
    public partial class MainWindow : Window
    {
        osVodigiWS.osVodigiServiceSoapClient ws = new osVodigiWS.osVodigiServiceSoapClient();

        string lastcontentcontroltype = String.Empty;

        int playerscreencontentlogid = 0;
        int playerscreenlogid = 0;

        int schedulecheckinterval = 15;
        DateTime lastschedulecheck = DateTime.Now;

        Thread dlthread;
        DispatcherTimer heartbeat;
        DispatcherTimer screencheck;
        DispatcherTimer contenttimer;
        DispatcherTimer startuptimer;

        UserControls.ucSelectionBar selectionbar = new UserControls.ucSelectionBar();

        Storyboard sbContentFadeIn;
        Storyboard sbContentFadeOut;

        string imagefillmode = "Fill";

        public MainWindow()
        {
            try
            {
                InitializeComponent();

                this.Loaded += new RoutedEventHandler(MainWindow_Loaded);
                this.Unloaded += new RoutedEventHandler(MainWindow_Unloaded);
                this.SizeChanged += new SizeChangedEventHandler(MainWindow_SizeChanged);
                this.KeyUp += new KeyEventHandler(MainWindow_KeyUp);
                this.MouseMove += new MouseEventHandler(MainWindow_MouseMove);

                ucSettings.SettingsComplete += new RoutedEventHandler(ucSettings_SettingsComplete);
                ucSurvey.SurveyClosed += new RoutedEventHandler(ucSurvey_SurveyClosed);

                ucSplash.SplashClosed += ucSplash_SplashClosed;
                ucSplash.SplashFadeInComplete += ucSplash_SplashFadeInComplete;
                ucRegister.RegisterClosed += ucRegister_RegisterClosed;
                ucSchedule.ScheduleClosed += ucSchedule_ScheduleClosed;
                ucDownload.DownloadClosed += ucDownload_DownloadClosed;
                ucWebservice.WebserviceClosed += ucWebservice_WebserviceClosed;
                ucResetPlayer.ResetPlayer += ucResetPlayer_ResetPlayer;
                ucResetPlayer.ResetPlayerCancel += ucResetPlayer_ResetPlayerCancel;

                sbContentFadeIn = (Storyboard)FindResource("sbContentFadeIn");
                sbContentFadeOut = (Storyboard)FindResource("sbContentFadeOut");
                sbContentFadeOut.Completed += new EventHandler(sbContentFadeOut_Completed);

                // Timer for heartbeat log
                heartbeat = new DispatcherTimer();
                heartbeat.Interval = new TimeSpan(0, 1, 0); // Every 60 seconds
                heartbeat.Tick += new EventHandler(heartbeat_Tick);

                // Timer for screen check
                screencheck = new DispatcherTimer();
                screencheck.Interval = new TimeSpan(0, 1, 0); // Every 60 seconds
                screencheck.Tick += new EventHandler(screencheck_Tick);

                // Timer for automatic content close
                contenttimer = new DispatcherTimer();
                int contenttimerinsecs = 60;
                contenttimer.Interval = TimeSpan.FromSeconds(contenttimerinsecs);
                contenttimer.Tick += new EventHandler(contenttimer_Tick);

                // Timer for the Startup
                startuptimer = new DispatcherTimer();
                startuptimer.Interval = TimeSpan.FromSeconds(3);
                startuptimer.Tick += startuptimer_Tick;

            }
            catch { }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Title = "Vodigi Player - Desktop Edition";
                DownloadManager.CreateDownloadFolders();
                startuptimer.Start();
                CreateHeartbeatLog();
            }
            catch { }
        }

        private void InitializeApplication()
        {
            try
            {
                ResetAppToStartupState();
                ResizeChildrenControls();

                PlayerConfiguration.LoadPlayerConfiguration();
                if (!PlayerConfiguration.configIsPlayerInitialized)
                    PlayerConfiguration.SavePlayerConfiguration();

                ucSplash.FadeIn();
            }
            catch { }
        }

        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        void ucSplash_SplashClosed(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!PlayerConfiguration.configIsPlayerInitialized)
                {
                    ucWebservice.FadeIn();
                }
                else
                {
                    ucSchedule.FadeIn();
                }
            }
            catch { }
        }

        void ucSplash_SplashFadeInComplete(object sender, RoutedEventArgs e)
        {
            try
            {
                // Load the Player Configuration
                PlayerConfiguration.LoadPlayerConfiguration();
                Thread.Sleep(2000);
                ucSplash.FadeOut();
            }
            catch { }
        }

        void ucWebservice_WebserviceClosed(object sender, RoutedEventArgs e)
        {
            try
            {
                if (PlayerConfiguration.configIsPlayerInitialized)
                    ucSchedule.FadeIn();
                else
                    ucRegister.FadeIn();
            }
            catch { }
        }


        void ucRegister_RegisterClosed(object sender, RoutedEventArgs e)
        {
            try
            {
                ucSchedule.FadeIn();
            }
            catch { }
        }


        void ucResetPlayer_ResetPlayerCancel(object sender, RoutedEventArgs e)
        {
            try
            {
                ResumeMainScreen();
            }
            catch { }
        }


        void ucResetPlayer_ResetPlayer(object sender, RoutedEventArgs e)
        {
            try
            {
                PlayerConfiguration.configIsPlayerInitialized = false;
                PlayerConfiguration.SavePlayerConfiguration();
                InitializeApplication();
            }
            catch { }
        }


        void ucDownload_DownloadClosed(object sender, RoutedEventArgs e)
        {
            try
            {
                // Start the download thread if not already running
                if (dlthread == null)
                {
                    DownloadThread downloadthread = new DownloadThread();
                    dlthread = new Thread(downloadthread.DownloadThreadWorker);
                    dlthread.Start();
                }

                // Hide the cursor if applicable
                if (Helpers.PlayerSettings.GetPlayerSetting("ShowCursor").PlayerSettingValue.ToLower() == "true")
                    this.Cursor = Cursors.Arrow;
                else
                    this.Cursor = Cursors.None;

                // Load the Current Screen and Display It
                LoadCurrentScreen(DateTime.Now, true);

                // Start the timers
                heartbeat.Start();
                screencheck.Start();
            }
            catch { }
        }

        void ucSchedule_ScheduleClosed(object sender, RoutedEventArgs e)
        {
            try
            {
                ucDownload.FadeIn();

                ws.Endpoint.Address = new System.ServiceModel.EndpointAddress(new Uri(PlayerConfiguration.configVodigiWebserviceURL));

                DownloadManager.DownloadFolder = Helpers.PlayerSettings.GetPlayerSetting("DownloadFolder").PlayerSettingValue;
                DownloadManager.MediaSourceUrl = Helpers.PlayerSettings.GetPlayerSetting("MediaSourceUrl").PlayerSettingValue;
                btnClose.Text = Helpers.PlayerSettings.GetPlayerSetting("ButtonTextClose").PlayerSettingValue;

            }
            catch { }
        }

        private void btnCloseContent_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CloseScreenContent();
            }
            catch { }
        }

        void contenttimer_Tick(object sender, EventArgs e)
        {
            try
            {
                CloseScreenContent();
            }
            catch { }
        }

        private void CloseScreenContent()
        {
            try
            {
                sbContentFadeOut.Begin();

                try
                {
                    if (lastcontentcontroltype == "ucSlideShowFader")
                    {
                        var control = (UserControls.ucSlideShowFader)gridContent.Children[0];
                        control.StopTimer();
                    }
                    else if (lastcontentcontroltype == "ucSlideShowDropFromTop")
                    {
                        var control = (UserControls.ucSlideShowDropFromTop)gridContent.Children[0];
                        control.StopTimer();
                    }
                    else if (lastcontentcontroltype == "ucSlideShowPanZoom")
                    {
                        var control = (UserControls.ucSlideShowPanZoom)gridContent.Children[0];
                        control.StopTimer();
                    }
                    else if (lastcontentcontroltype == "ucSlideShowSlideFromRight")
                    {
                        var control = (UserControls.ucSlideShowSlideFromRight)gridContent.Children[0];
                        control.StopTimer();
                    }
                    else if (lastcontentcontroltype == "ucSlideShowZoomIn")
                    {
                        var control = (UserControls.ucSlideShowZoomIn)gridContent.Children[0];
                        control.StopTimer();
                    }
                    else if (lastcontentcontroltype == "ucPlayList")
                    {
                        var control = (UserControls.ucPlayList)gridContent.Children[0];
                        control.StopAll();
                    }
                    else if (lastcontentcontroltype == "ucTimeline")
                    {
                        var control = (UserControls.ucTimeline)gridContent.Children[0];
                        control.StopAll();
                    }
                }
                catch { }

                gridClose.Visibility = Visibility.Collapsed;

                // Stop the image timer
                contenttimer.Stop();

                // Resume the main screen
                ResumeMainScreen();

                // Update the close time
                try
                {
                    ws.PlayerScreenContentLog_UpdateCloseDateTimeAsync(playerscreencontentlogid, DateTime.UtcNow);
                }
                catch { }
            }
            catch { }
        }

        void ResizeChildrenControls()
        {
            try
            {
                // Size the children controls
                gridFullScreen.Width = this.Width - 20;
                gridFullScreen.Height = this.Height - 20;

                gridMain.Width = this.Width - 20;
                gridMain.Height = this.Height - 20;
            }
            catch { }
        }

        private void StopMainScreen()
        {
            try
            {
                try
                {
                    UserControls.ucPlayList playlist = (UserControls.ucPlayList)gridScreenControls.Children[0];
                    playlist.Pause();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucSlideShowDropFromTop slideshow = (UserControls.ucSlideShowDropFromTop)gridScreenControls.Children[0];
                    slideshow.Pause();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucSlideShowFader slideshow = (UserControls.ucSlideShowFader)gridScreenControls.Children[0];
                    slideshow.Pause();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucSlideShowPanZoom slideshow = (UserControls.ucSlideShowPanZoom)gridScreenControls.Children[0];
                    slideshow.Pause();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucSlideShowSlideFromRight slideshow = (UserControls.ucSlideShowSlideFromRight)gridScreenControls.Children[0];
                    slideshow.Pause();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucSlideShowZoomIn slideshow = (UserControls.ucSlideShowZoomIn)gridScreenControls.Children[0];
                    slideshow.Pause();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucTimeline timeline = (UserControls.ucTimeline)gridScreenControls.Children[0];
                    timeline.Pause();
                    return;
                }
                catch { }
            }
            catch { }
        }

        private void ResumeMainScreen()
        {
            try
            {
                try
                {
                    UserControls.ucPlayList playlist = (UserControls.ucPlayList)gridScreenControls.Children[0];
                    playlist.Resume();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucSlideShowDropFromTop slideshow = (UserControls.ucSlideShowDropFromTop)gridScreenControls.Children[0];
                    slideshow.Resume();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucSlideShowFader slideshow = (UserControls.ucSlideShowFader)gridScreenControls.Children[0];
                    slideshow.Resume();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucSlideShowPanZoom slideshow = (UserControls.ucSlideShowPanZoom)gridScreenControls.Children[0];
                    slideshow.Resume();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucSlideShowSlideFromRight slideshow = (UserControls.ucSlideShowSlideFromRight)gridScreenControls.Children[0];
                    slideshow.Resume();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucSlideShowZoomIn slideshow = (UserControls.ucSlideShowZoomIn)gridScreenControls.Children[0];
                    slideshow.Resume();
                    return;
                }
                catch { }
                try
                {
                    UserControls.ucTimeline timeline = (UserControls.ucTimeline)gridScreenControls.Children[0];
                    timeline.Resume();
                    return;
                }
                catch { }
            }
            catch { }
        }

        private void StopControl(UIElement control)
        {
            try
            {
                try
                {
                    var playlist = (UserControls.ucPlayList)control;
                    playlist.StopAll();
                    return;
                }
                catch { }
                try
                {
                    var slideshow = (UserControls.ucSlideShowDropFromTop)control;
                    slideshow.StopTimer();
                    return;
                }
                catch { }
                try
                {
                    var slideshow = (UserControls.ucSlideShowFader)control;
                    slideshow.StopTimer();
                    return;
                }
                catch { }
                try
                {
                    var slideshow = (UserControls.ucSlideShowPanZoom)control;
                    slideshow.StopTimer();
                    return;
                }
                catch { }
                try
                {
                    var slideshow = (UserControls.ucSlideShowSlideFromRight)control;
                    slideshow.StopTimer();
                    return;
                }
                catch { }
                try
                {
                    var slideshow = (UserControls.ucSlideShowZoomIn)control;
                    slideshow.StopTimer();
                    return;
                }
                catch { }
                try
                {
                    var timeline = (UserControls.ucTimeline)control;
                    timeline.StopAll();
                    return;
                }
                catch { }
            }
            catch { }
        }
 
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                StopMainScreen();

                // Attempt to kill the download thread
                dlthread.Abort();
            }
            catch { }
        }

        private void InteractiveButtonClick()
        {
            try
            {
                // Stop the slideshow/playlist
                StopMainScreen();

                // Reset and display the selection bar
                selectionbar.dsButtonBackText = Helpers.PlayerSettings.GetPlayerSetting("ButtonTextBack").PlayerSettingValue;
                selectionbar.dsButtonNextText = Helpers.PlayerSettings.GetPlayerSetting("ButtonTextNext").PlayerSettingValue;
                selectionbar.dsButtonCloseText = Helpers.PlayerSettings.GetPlayerSetting("ButtonTextClose").PlayerSettingValue;

                selectionbar.ResetControl();
                selectionbar.FadeIn();
            }
            catch { }
        }

        private void CreateHeartbeatLog()
        {
            try
            {
                ws.ActivityLog_CreateAsync(PlayerConfiguration.configAccountID,
                    0,
                    "Pla.yer",
                    "Heartbeat",
                    DateTime.UtcNow,
                    "Reported heartbeat player '" + PlayerConfiguration.configPlayerName + "' - ID: " + PlayerConfiguration.configPlayerID.ToString());
            }
            catch { }
        }

        // Return true to indicate InitializeApplication should be called on return; false otherwise
        private bool GetPlayerSchedule()
        {
            try
            {
                int playerID = PlayerConfiguration.configPlayerID;

                // Get the player's schedule - there is only one
                string xml = String.Empty;

                try
                {
                    xml = ws.Player_GetCurrentSchedule(playerID);
                }
                catch
                {
                    // If unable to get the current schedule, and there is no schedule in memory, then attempt to
                    // load the last schedule from disk
                    if (CurrentSchedule.PlayerGroupSchedules == null || CurrentSchedule.PlayerGroupSchedules.Count == 0)
                    {
                        xml = ScheduleFile.ReadScheduleFile();
                        if (String.IsNullOrEmpty(xml))
                            return false;
                    }
                    else
                    {
                        return false;
                    }
                }

                if (xml == CurrentSchedule.LastScheduleXML)
                    return false;

                ScheduleFile.SaveScheduleFile(xml);

                CurrentSchedule.ClearSchedule();
                CurrentSchedule.LastScheduleXML = xml;
                CurrentSchedule.ParseScheduleXml(xml);

                return true;
            }
            catch { return false; }
        }

        private void ResetAppToStartupState()
        {
            try
            {
                gridScreenControls.Children.Clear();
                gridInteractiveControls.Visibility = Visibility.Collapsed;
                ucSettings.Visibility = Visibility.Collapsed;
                ucSplash.Visibility = Visibility.Collapsed;
                ucRegister.Visibility = Visibility.Collapsed;
                ucSchedule.Visibility = Visibility.Collapsed;
                ucDownload.Visibility = Visibility.Collapsed;
                ucSurvey.Visibility = Visibility.Collapsed;
                gridContent.Children.Clear();
                gridContent.Visibility = Visibility.Collapsed;
                gridClose.Visibility = Visibility.Collapsed;
                gridMouseCounter.Visibility = Visibility.Collapsed;
            }
            catch { }
        }

        void startuptimer_Tick(object sender, EventArgs e)
        {
            try
            {
                startuptimer.Stop();
                InitializeApplication();
            }
            catch { }
        }

        private void LoadCurrentScreen(DateTime now, bool forceScreenRefresh)
        {
            try
            {
                // Update the current screen and populate the CurrentScreen object

                int day = 0;
                if (now.DayOfWeek == DayOfWeek.Sunday)
                    day = 0;
                else if (now.DayOfWeek == DayOfWeek.Monday)
                    day = 1;
                else if (now.DayOfWeek == DayOfWeek.Tuesday)
                    day = 2;
                else if (now.DayOfWeek == DayOfWeek.Wednesday)
                    day = 3;
                else if (now.DayOfWeek == DayOfWeek.Thursday)
                    day = 4;
                else if (now.DayOfWeek == DayOfWeek.Friday)
                    day = 5;
                else if (now.DayOfWeek == DayOfWeek.Saturday)
                    day = 6;

                for (int i = 0; i < CurrentSchedule.PlayerGroupSchedules.Count; i += 1)
                {
                    PlayerGroupSchedule pgs = CurrentSchedule.PlayerGroupSchedules[i];
                    bool displayScreen = false;
                    int index = 0;

                    // Create a date for comparison with  forced screen refresh
                    DateTime compare = now.AddDays(-day); // Set to first of week
                    compare = new DateTime(compare.Year, compare.Month, compare.Day, 0, 0, 0); // Clear the times
                    compare = compare.AddDays(pgs.Day).AddHours(pgs.Hour).AddMinutes(pgs.Minute);

                    if (!forceScreenRefresh) // Normal operation
                    {
                        if (pgs.Day == day && pgs.Hour == now.Hour && pgs.Minute == now.Minute)
                        {
                            displayScreen = true;
                            index = i;
                        }
                    }
                    else // force a screen refresh from the previous screen
                    {
                        if (now < compare)
                        {
                            displayScreen = true;
                            index = i - 1;
                            if (index < 0)
                                index = 0;
                        }
                        else if ((i + 1) == CurrentSchedule.PlayerGroupSchedules.Count)
                        {
                            displayScreen = true;
                            index = CurrentSchedule.PlayerGroupSchedules.Count - 1;
                            if (index < 0)
                                index = 0;
                        }
                    }

                    if (displayScreen)
                    {
                        // Update the close time on the previous screen
                        try
                        {
                            ws.PlayerScreenLog_UpdateCloseDateTimeAsync(playerscreenlogid, DateTime.UtcNow);
                        }
                        catch { }

                        pgs = CurrentSchedule.PlayerGroupSchedules[index];
                        
                        // Blank the screen
                        DisplayCurrentScreen(true);

                        // Initialize the Current Screen object
                        CurrentScreen.ScreenInfo = new Screen();
                        CurrentScreen.ScreenScreenContentXrefs = new List<ScreenScreenContentXref>();
                        CurrentScreen.ScreenContents = new List<ScreenContent>();
                        CurrentScreen.SlideShows = new List<SlideShow>();
                        CurrentScreen.SlideShowImageXrefs = new List<SlideShowImageXref>();
                        CurrentScreen.SlideShowMusicXrefs = new List<SlideShowMusicXref>();
                        CurrentScreen.Timelines = new List<Timeline>();
                        CurrentScreen.TimelineImageXrefs = new List<TimelineImageXref>();
                        CurrentScreen.TimelineMusicXrefs = new List<TimelineMusicXref>();
                        CurrentScreen.TimelineVideoXrefs = new List<TimelineVideoXref>();
                        CurrentScreen.Images = new List<Image>();
                        CurrentScreen.Musics = new List<Music>();
                        CurrentScreen.PlayLists = new List<PlayList>();
                        CurrentScreen.PlayListVideoXrefs = new List<PlayListVideoXref>();
                        CurrentScreen.Videos = new List<Video>();
                        CurrentScreen.Surveys = new List<Survey>();
                        CurrentScreen.SurveyQuestions = new List<SurveyQuestion>();
                        CurrentScreen.SurveyQuestionOptions = new List<SurveyQuestionOption>();

                        int screenID = pgs.ScreenID;

                        // Get the Screen Info
                        foreach (Screen screen in CurrentSchedule.Screens)
                        {
                            if (screenID == screen.ScreenID)
                            {
                                CurrentScreen.ScreenInfo = screen;
                                break;
                            }
                        }

                        // Get the ScreenScreenContentXrefs
                        foreach (ScreenScreenContentXref xref in CurrentSchedule.ScreenScreenContentXrefs)
                        {
                            if (screenID == xref.ScreenID)
                                CurrentScreen.ScreenScreenContentXrefs.Add(xref);
                        }
                        CurrentScreen.ScreenScreenContentXrefs = CurrentScreen.ScreenScreenContentXrefs.Distinct().ToList();

                        // Get the ScreenContents
                        foreach (ScreenScreenContentXref xref in CurrentScreen.ScreenScreenContentXrefs)
                        {
                            foreach (ScreenContent sc in CurrentSchedule.ScreenContents)
                            {
                                if (xref.ScreenContentID == sc.ScreenContentID)
                                {
                                    CurrentScreen.ScreenContents.Add(sc);
                                    // Add the thumbnail image
                                    foreach (Image image in CurrentSchedule.Images)
                                    {
                                        if (sc.ThumbnailImageID == image.ImageID)
                                        {
                                            CurrentScreen.Images.Add(image);
                                            break;
                                        }
                                    }

                                    // Add the image if image screen content
                                    if (sc.ScreenContentTypeID == 1000000)
                                    {
                                        foreach (Image image in CurrentSchedule.Images)
                                        {
                                            if (Convert.ToInt32(sc.CustomField1) == image.ImageID)
                                            {
                                                CurrentScreen.Images.Add(image);
                                                break;
                                            }
                                        }
                                    }

                                    // Add the video if video screen content
                                    if (sc.ScreenContentTypeID == 1000002)
                                    {
                                        foreach (Video video in CurrentSchedule.Videos)
                                        {
                                            if (Convert.ToInt32(sc.CustomField1) == video.VideoID)
                                            {
                                                CurrentScreen.Videos.Add(video);
                                                break;
                                            }
                                        }
                                    }

                                    break;
                                }
                            }
                        }
                        CurrentScreen.ScreenContents = CurrentScreen.ScreenContents.Distinct().ToList();

                        // Get the content Surveys
                        foreach (ScreenContent sc in CurrentScreen.ScreenContents)
                        {
                            if (sc.ScreenContentTypeID == 1000007)
                            {
                                foreach (Survey svy in CurrentSchedule.Surveys)
                                {
                                    if (Convert.ToInt32(sc.CustomField1) == svy.SurveyID)
                                    {
                                        CurrentScreen.Surveys.Add(svy);

                                        // Add the survey image
                                        foreach (Image image in CurrentSchedule.Images)
                                        {
                                            if (Convert.ToInt32(svy.SurveyImageID) == image.ImageID)
                                            {
                                                CurrentScreen.Images.Add(image);
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        CurrentScreen.Surveys = CurrentScreen.Surveys.Distinct().ToList();

                        // Add the Survey Questions
                        foreach (Survey svy in CurrentScreen.Surveys)
                        {
                            foreach (SurveyQuestion qst in CurrentSchedule.SurveyQuestions)
                            {
                                if (qst.SurveyID == svy.SurveyID)
                                    CurrentScreen.SurveyQuestions.Add(qst);
                            }
                        }
                        CurrentScreen.SurveyQuestions = CurrentScreen.SurveyQuestions.Distinct().ToList();

                        // Add the Survey Question Options
                        foreach (SurveyQuestion qst in CurrentScreen.SurveyQuestions)
                        {
                            foreach (SurveyQuestionOption opt in CurrentSchedule.SurveyQuestionOptions)
                            {
                                if (opt.SurveyQuestionID == qst.SurveyQuestionID)
                                    CurrentScreen.SurveyQuestionOptions.Add(opt);
                            }
                        }
                        CurrentScreen.SurveyQuestionOptions = CurrentScreen.SurveyQuestionOptions.Distinct().ToList();

                        // Get the Screen SlideShow - if appropriate
                        if (CurrentScreen.ScreenInfo.SlideShowID != 0)
                        {
                            foreach (SlideShow ss in CurrentSchedule.SlideShows)
                            {
                                if (CurrentScreen.ScreenInfo.SlideShowID == ss.SlideShowID)
                                {
                                    CurrentScreen.SlideShows.Add(ss);
                                    break;
                                }
                            }
                        }

                        // Get the content SlideShows
                        foreach (ScreenContent sc in CurrentScreen.ScreenContents)
                        {
                            if (sc.ScreenContentTypeID == 1000001)
                            {
                                foreach (SlideShow ss in CurrentSchedule.SlideShows)
                                {
                                    if (Convert.ToInt32(sc.CustomField1) == ss.SlideShowID)
                                    {
                                        CurrentScreen.SlideShows.Add(ss);
                                        break;
                                    }
                                }
                            }
                        }
                        CurrentScreen.SlideShows = CurrentScreen.SlideShows.Distinct().ToList();

                        // Get the SlideShowImageXrefs
                        foreach (SlideShow ss in CurrentScreen.SlideShows)
                        {
                            foreach (SlideShowImageXref xref in CurrentSchedule.SlideShowImageXrefs)
                            {
                                if (ss.SlideShowID == xref.SlideShowID)
                                    CurrentScreen.SlideShowImageXrefs.Add(xref);
                            }
                        }
                        CurrentScreen.SlideShowImageXrefs = CurrentScreen.SlideShowImageXrefs.Distinct().ToList();

                        // Get the SlideShowMusicXrefs
                        foreach (SlideShow ss in CurrentScreen.SlideShows)
                        {
                            foreach (SlideShowMusicXref xref in CurrentSchedule.SlideShowMusicXrefs)
                            {
                                if (ss.SlideShowID == xref.SlideShowID)
                                    CurrentScreen.SlideShowMusicXrefs.Add(xref);
                            }
                        }
                        CurrentScreen.SlideShowMusicXrefs = CurrentScreen.SlideShowMusicXrefs.Distinct().ToList();

                        // Get the Screen Button Thumbnail - if appropriate
                        if (CurrentScreen.ScreenInfo.ButtonImageID != 0)
                        {
                            foreach (Image image in CurrentSchedule.Images)
                            {
                                if (CurrentScreen.ScreenInfo.ButtonImageID == image.ImageID)
                                {
                                    CurrentScreen.Images.Add(image);
                                    break;
                                }
                            }
                        }

                        // Get the Slideshow Images
                        foreach (SlideShowImageXref xref in CurrentScreen.SlideShowImageXrefs)
                        {
                            foreach (Image image in CurrentSchedule.Images)
                            {
                                if (xref.ImageID == image.ImageID)
                                {
                                    CurrentScreen.Images.Add(image);
                                    break;
                                }
                            }
                        }
                        CurrentScreen.Images = CurrentScreen.Images.Distinct().ToList();

                        // Get the Slideshow Musics
                        foreach (SlideShowMusicXref xref in CurrentScreen.SlideShowMusicXrefs)
                        {
                            foreach (Music music in CurrentSchedule.Musics)
                            {
                                if (xref.MusicID == music.MusicID)
                                {
                                    CurrentScreen.Musics.Add(music);
                                    break;
                                }
                            }
                        }
                        CurrentScreen.Musics = CurrentScreen.Musics.Distinct().ToList();

                        // Get the Screen Timeline - if appropriate
                        if (CurrentScreen.ScreenInfo.TimelineID != 0)
                        {
                            foreach (Timeline tl in CurrentSchedule.Timelines)
                            {
                                if (CurrentScreen.ScreenInfo.TimelineID == tl.TimelineID)
                                {
                                    CurrentScreen.Timelines.Add(tl);
                                    break;
                                }
                            }
                        }

                        // Get the content Timelines
                        foreach (ScreenContent sc in CurrentScreen.ScreenContents)
                        {
                            if (sc.ScreenContentTypeID == 1000008)
                            {
                                foreach (Timeline tl in CurrentSchedule.Timelines)
                                {
                                    if (Convert.ToInt32(sc.CustomField1) == tl.TimelineID)
                                    {
                                        CurrentScreen.Timelines.Add(tl);
                                        break;
                                    }
                                }
                            }
                        }
                        CurrentScreen.Timelines = CurrentScreen.Timelines.Distinct().ToList();

                        // Get the TimelineImageXrefs
                        foreach (Timeline tl in CurrentScreen.Timelines)
                        {
                            foreach (TimelineImageXref xref in CurrentSchedule.TimelineImageXrefs)
                            {
                                if (tl.TimelineID == xref.TimelineID)
                                    CurrentScreen.TimelineImageXrefs.Add(xref);
                            }
                        }
                        CurrentScreen.TimelineImageXrefs = CurrentScreen.TimelineImageXrefs.Distinct().ToList();

                        // Get the TimelineMusicXrefs
                        foreach (Timeline tl in CurrentScreen.Timelines)
                        {
                            foreach (TimelineMusicXref xref in CurrentSchedule.TimelineMusicXrefs)
                            {
                                if (tl.TimelineID == xref.TimelineID)
                                    CurrentScreen.TimelineMusicXrefs.Add(xref);
                            }
                        }
                        CurrentScreen.TimelineMusicXrefs = CurrentScreen.TimelineMusicXrefs.Distinct().ToList();

                        // Get the TimelineVideoXrefs
                        foreach (Timeline tl in CurrentScreen.Timelines)
                        {
                            foreach (TimelineVideoXref xref in CurrentSchedule.TimelineVideoXrefs)
                            {
                                if (tl.TimelineID == xref.TimelineID)
                                    CurrentScreen.TimelineVideoXrefs.Add(xref);
                            }
                        }
                        CurrentScreen.TimelineVideoXrefs = CurrentScreen.TimelineVideoXrefs.Distinct().ToList();

                        // Get the Timeline Images
                        foreach (TimelineImageXref xref in CurrentScreen.TimelineImageXrefs)
                        {
                            foreach (Image image in CurrentSchedule.Images)
                            {
                                if (xref.ImageID == image.ImageID)
                                {
                                    CurrentScreen.Images.Add(image);
                                    break;
                                }
                            }
                        }
                        CurrentScreen.Images = CurrentScreen.Images.Distinct().ToList();

                        // Get the Timeline Musics
                        foreach (TimelineMusicXref xref in CurrentScreen.TimelineMusicXrefs)
                        {
                            foreach (Music music in CurrentSchedule.Musics)
                            {
                                if (xref.MusicID == music.MusicID)
                                {
                                    CurrentScreen.Musics.Add(music);
                                    break;
                                }
                            }
                        }
                        CurrentScreen.Musics = CurrentScreen.Musics.Distinct().ToList();

                        // Get the Timeline Videos
                        foreach (TimelineVideoXref xref in CurrentScreen.TimelineVideoXrefs)
                        {
                            foreach (Video video in CurrentSchedule.Videos)
                            {
                                if (xref.VideoID == video.VideoID)
                                {
                                    CurrentScreen.Videos.Add(video);
                                    break;
                                }
                            }
                        }
                        CurrentScreen.Videos = CurrentScreen.Videos.Distinct().ToList();

                        // Get the Screen PlayList - if appropriate
                        if (CurrentScreen.ScreenInfo.PlayListID != 0)
                        {
                            foreach (PlayList pl in CurrentSchedule.PlayLists)
                            {
                                if (CurrentScreen.ScreenInfo.PlayListID == pl.PlayListID)
                                {
                                    CurrentScreen.PlayLists.Add(pl);
                                    break;
                                }
                            }
                        }

                        // Get the content PlayLists
                        foreach (ScreenContent sc in CurrentScreen.ScreenContents)
                        {
                            if (sc.ScreenContentTypeID == 1000003)
                            {
                                foreach (PlayList pl in CurrentSchedule.PlayLists)
                                {
                                    if (Convert.ToInt32(sc.CustomField1) == pl.PlayListID)
                                    {
                                        CurrentScreen.PlayLists.Add(pl);
                                        break;
                                    }
                                }
                            }
                        }
                        CurrentScreen.PlayLists = CurrentScreen.PlayLists.Distinct().ToList();

                        // Get the PlayListVideoXrefs
                        foreach (PlayList pl in CurrentScreen.PlayLists)
                        {
                            foreach (PlayListVideoXref xref in CurrentSchedule.PlayListVideoXrefs)
                            {
                                if (pl.PlayListID == xref.PlayListID)
                                    CurrentScreen.PlayListVideoXrefs.Add(xref);
                            }
                        }
                        CurrentScreen.PlayListVideoXrefs = CurrentScreen.PlayListVideoXrefs.Distinct().ToList();

                        // Get the PlayList Videos
                        foreach (PlayListVideoXref xref in CurrentScreen.PlayListVideoXrefs)
                        {
                            foreach (Video video in CurrentSchedule.Videos)
                            {
                                if (xref.VideoID == video.VideoID)
                                {
                                    CurrentScreen.Videos.Add(video);
                                    break;
                                }
                            }
                        }
                        CurrentScreen.Videos = CurrentScreen.Videos.Distinct().ToList();

                        DisplayCurrentScreen(false);

                        if (CurrentScreen.ScreenInfo != null)
                        {
                            string details = String.Empty;
                            if (CurrentScreen.ScreenInfo.SlideShowID != 0)
                                details += "SlideShow";
                            else if (CurrentScreen.ScreenInfo.TimelineID != 0)
                                details += "Timeline";
                            else if (CurrentScreen.ScreenInfo.PlayListID != 0)
                                details += "PlayList";
                            if (CurrentScreen.ScreenInfo.IsInteractive)
                                details += " - Interactive";

                            try
                            {
                                ws.PlayerScreenLog_CreateAsync(PlayerConfiguration.configAccountID,
                                            PlayerConfiguration.configPlayerID,
                                            PlayerConfiguration.configPlayerName,
                                            CurrentScreen.ScreenInfo.ScreenID,
                                            CurrentScreen.ScreenInfo.ScreenName,
                                            DateTime.UtcNow,
                                            DateTime.UtcNow,
                                            details);
                            }
                            catch { }
                        }

                        return;
                    }
                }
            }
            catch { }
        }

        public void DisplayCurrentScreen(bool showBlank)
        {
            try
            {
                gridScreenControls.Visibility = Visibility.Collapsed;
                gridInteractiveControls.Visibility = Visibility.Collapsed;

                foreach (UIElement control in gridScreenControls.Children)
                {
                    StopControl(control);
                }

                foreach (UIElement control in gridInteractiveControls.Children)
                {
                    StopControl(control);
                }
 
                gridScreenControls.Children.Clear();
                gridInteractiveControls.Children.Clear();

                // Blank screen and exit if appropriate
                if (showBlank || CurrentScreen.ScreenInfo.ScreenID == 0)
                {
                    return;
                }

                // Display the screen

                // Add the playlist if appropriate
                if (CurrentScreen.ScreenInfo.PlayListID != 0)
                {
                    // Get the videos to display
                    List<string> videos = new List<string>();
                    foreach (PlayListVideoXref xref in CurrentScreen.PlayListVideoXrefs)
                    {
                        if (CurrentScreen.ScreenInfo.PlayListID == xref.PlayListID)
                        {
                            foreach (Video video in CurrentScreen.Videos)
                            {
                                if (xref.VideoID == video.VideoID)
                                {
                                    videos.Add(DownloadManager.DownloadFolder + @"Videos\" + video.StoredFilename);
                                    break;
                                }
                            }
                        }
                    }

                    UserControls.ucPlayList ucplaylist = new UserControls.ucPlayList();
                    ucplaylist.Width = this.Width;
                    ucplaylist.Height = this.Height;
                    ucplaylist.dsVideoURLs = videos;
                    ucplaylist.Visibility = Visibility.Visible;
                    gridScreenControls.Children.Add(ucplaylist);
                    ucplaylist.ResetControl();
                }

                // Add the slideshow if appropriate
                if (CurrentScreen.ScreenInfo.SlideShowID != 0)
                {
                    // Get the images to display
                    List<string> images = new List<string>();
                    foreach (SlideShowImageXref xref in CurrentScreen.SlideShowImageXrefs)
                    {
                        if (CurrentScreen.ScreenInfo.SlideShowID == xref.SlideShowID)
                        {
                            foreach (Image image in CurrentScreen.Images)
                            {
                                if (xref.ImageID == image.ImageID)
                                {
                                    images.Add(DownloadManager.DownloadFolder + @"Images\" + image.StoredFilename);
                                    break;
                                }
                            }
                        }
                    }

                    // Get the music to play
                    List<string> musics = new List<string>();
                    foreach (SlideShowMusicXref xref in CurrentScreen.SlideShowMusicXrefs)
                    {
                        if (CurrentScreen.ScreenInfo.SlideShowID == xref.SlideShowID)
                        {
                            foreach (Music music in CurrentScreen.Musics)
                            {
                                if (xref.MusicID == music.MusicID)
                                {
                                    musics.Add(DownloadManager.DownloadFolder + @"Music\" + music.StoredFilename);
                                    break;
                                }
                            }
                        }
                    }

                    // Determine the type of slideshow to create - "Fade", "Drop From Top", "Slide From Right", "Pan Zoom", "Zoom In"
                    SlideShow slideshow = new SlideShow();
                    foreach (SlideShow ss in CurrentScreen.SlideShows)
                    {
                        if (CurrentScreen.ScreenInfo.SlideShowID == ss.SlideShowID)
                            slideshow = ss;
                    }

                    if (slideshow.TransitionType == "Drop From Top")
                    {
                        UserControls.ucSlideShowDropFromTop drop = new UserControls.ucSlideShowDropFromTop();
                        drop.Width = this.Width;
                        drop.Height = this.Height;
                        drop.VerticalAlignment = VerticalAlignment.Top;
                        drop.HorizontalAlignment = HorizontalAlignment.Left;
                        drop.dsSlideDurationInSeconds = slideshow.IntervalInSecs;
                        drop.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                        drop.dsImageURLs = images;
                        drop.dsMusicURLs = musics;
                        drop.dsImageFillMode = imagefillmode;
                        drop.Visibility = Visibility.Visible;
                        gridScreenControls.Children.Add(drop);
                        drop.ResetControl();
                    }
                    else if (slideshow.TransitionType == "Slide From Right")
                    {
                        UserControls.ucSlideShowSlideFromRight slide = new UserControls.ucSlideShowSlideFromRight();
                        slide.Width = this.Width;
                        slide.Height = this.Height;
                        slide.VerticalAlignment = VerticalAlignment.Top;
                        slide.HorizontalAlignment = HorizontalAlignment.Left;
                        slide.dsSlideDurationInSeconds = slideshow.IntervalInSecs;
                        slide.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                        slide.dsImageURLs = images;
                        slide.dsMusicURLs = musics;
                        slide.dsImageFillMode = imagefillmode;
                        slide.Visibility = Visibility.Visible;
                        gridScreenControls.Children.Add(slide);
                        slide.ResetControl();
                    }
                    else if (slideshow.TransitionType == "Pan Zoom")
                    {
                        UserControls.ucSlideShowPanZoom panzoom = new UserControls.ucSlideShowPanZoom();
                        panzoom.Width = this.Width;
                        panzoom.Height = this.Height;
                        panzoom.VerticalAlignment = VerticalAlignment.Top;
                        panzoom.HorizontalAlignment = HorizontalAlignment.Left;
                        panzoom.dsSlideDurationInSeconds = slideshow.IntervalInSecs;
                        panzoom.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                        panzoom.dsImageURLs = images;
                        panzoom.dsMusicURLs = musics;
                        panzoom.dsImageFillMode = imagefillmode;
                        panzoom.Visibility = Visibility.Visible;
                        gridScreenControls.Children.Add(panzoom);
                        panzoom.ResetControl();
                    }
                    else if (slideshow.TransitionType == "Zoom In")
                    {
                        UserControls.ucSlideShowZoomIn zoomin = new UserControls.ucSlideShowZoomIn();
                        zoomin.Width = this.Width;
                        zoomin.Height = this.Height;
                        zoomin.VerticalAlignment = VerticalAlignment.Top;
                        zoomin.HorizontalAlignment = HorizontalAlignment.Left;
                        zoomin.dsSlideDurationInSeconds = slideshow.IntervalInSecs;
                        zoomin.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                        zoomin.dsImageURLs = images;
                        zoomin.dsMusicURLs = musics;
                        zoomin.dsImageFillMode = imagefillmode;
                        zoomin.Visibility = Visibility.Visible;
                        gridScreenControls.Children.Add(zoomin);
                        zoomin.ResetControl();
                    }
                    else // Fade
                    {
                        UserControls.ucSlideShowFader fader = new UserControls.ucSlideShowFader();
                        fader.Width = this.Width;
                        fader.Height = this.Height;
                        fader.VerticalAlignment = VerticalAlignment.Top;
                        fader.HorizontalAlignment = HorizontalAlignment.Left;
                        fader.dsSlideDurationInSeconds = slideshow.IntervalInSecs;
                        fader.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                        fader.dsImageURLs = images;
                        fader.dsMusicURLs = musics;
                        fader.dsImageFillMode = imagefillmode;
                        fader.Visibility = Visibility.Visible;
                        gridScreenControls.Children.Add(fader);
                        fader.ResetControl();
                    }
                }

                // Add the timeline if appropriate
                if (CurrentScreen.ScreenInfo.TimelineID != 0)
                {
                    // Get the timeline to display
                    Timeline timeline = new Timeline();
                    foreach (Timeline tl in CurrentScreen.Timelines)
                    {
                        if (CurrentScreen.ScreenInfo.TimelineID == tl.TimelineID)
                            timeline = tl;
                    }

                    List<TimelineMedia> timelinemedia = new List<TimelineMedia>();

                    // Get the videos to display
                    foreach (TimelineVideoXref xref in CurrentScreen.TimelineVideoXrefs)
                    {
                        if (CurrentScreen.ScreenInfo.TimelineID == xref.TimelineID)
                        {
                            foreach (Video video in CurrentScreen.Videos)
                            {
                                if (xref.VideoID == video.VideoID)
                                {
                                    TimelineMedia tlm = new TimelineMedia();
                                    tlm.StoredFilename = DownloadManager.DownloadFolder + @"Videos\" + video.StoredFilename;
                                    tlm.DisplayOrder = xref.DisplayOrder;
                                    timelinemedia.Add(tlm);
                                    break;
                                }
                            }
                        }
                    }

                    // Get the images to display
                    List<string> images = new List<string>();
                    foreach (TimelineImageXref xref in CurrentScreen.TimelineImageXrefs)
                    {
                        if (CurrentScreen.ScreenInfo.TimelineID == xref.TimelineID)
                        {
                            foreach (Image image in CurrentScreen.Images)
                            {
                                if (xref.ImageID == image.ImageID)
                                {
                                    TimelineMedia tlm = new TimelineMedia();
                                    tlm.StoredFilename = DownloadManager.DownloadFolder + @"Images\" + image.StoredFilename;
                                    tlm.DisplayOrder = xref.DisplayOrder;
                                    timelinemedia.Add(tlm);
                                    break;
                                }
                            }
                        }
                    }

                    timelinemedia.Sort(delegate(TimelineMedia tlm1, TimelineMedia tlm2)
                                { return tlm1.DisplayOrder.CompareTo(tlm2.DisplayOrder); });

                    List<string> tlmedia = new List<string>();
                    foreach (TimelineMedia tlm in timelinemedia)
                    {
                        tlmedia.Add(tlm.StoredFilename);
                    }

                    // Get the music to play
                    List<string> musics = new List<string>();
                    foreach (TimelineMusicXref xref in CurrentScreen.TimelineMusicXrefs)
                    {
                        if (CurrentScreen.ScreenInfo.TimelineID == xref.TimelineID)
                        {
                            foreach (Music music in CurrentScreen.Musics)
                            {
                                if (xref.MusicID == music.MusicID)
                                {
                                    musics.Add(DownloadManager.DownloadFolder + @"Music\" + music.StoredFilename);
                                    break;
                                }
                            }
                        }
                    }


                    UserControls.ucTimeline uctimeline = new UserControls.ucTimeline();
                    uctimeline.Width = this.Width;
                    uctimeline.Height = this.Height;
                    uctimeline.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                    uctimeline.dsMediaURLs = tlmedia;
                    uctimeline.dsMusicURLs = musics;
                    uctimeline.dsFireCompleteEvent = false;
                    uctimeline.dsMuteMusicOnPlayback = timeline.MuteMusicOnPlayback;
                    uctimeline.dsSlideDurationInSeconds = timeline.DurationInSecs;
                    uctimeline.Visibility = Visibility.Visible;
                    gridScreenControls.Children.Add(uctimeline);
                    uctimeline.ResetControl();
                }

                // Add the interactive controls
                if (CurrentScreen.ScreenInfo.IsInteractive)
                {
                    string imagepath = String.Empty;

                    // Add the Interactive button
                    foreach (Image image in CurrentSchedule.Images)
                    {
                        if (CurrentScreen.ScreenInfo.ButtonImageID == image.ImageID)
                            imagepath = DownloadManager.DownloadFolder + @"Images\" + image.StoredFilename;
                    }
                    System.Windows.Controls.Image interactivebutton = new System.Windows.Controls.Image();
                    interactivebutton.Source = new BitmapImage(new Uri(imagepath));
                    interactivebutton.Stretch = Stretch.Fill;
                    interactivebutton.Width = 225;
                    interactivebutton.Height = 80;
                    interactivebutton.VerticalAlignment = VerticalAlignment.Bottom;
                    interactivebutton.HorizontalAlignment = HorizontalAlignment.Right;
                    interactivebutton.Margin = new Thickness(0, 0, 30, 30);
                    interactivebutton.MouseLeftButtonDown += new MouseButtonEventHandler(interactivebutton_MouseLeftButtonDown);
                    gridScreenControls.Children.Add(interactivebutton);

                    // Add the Selection Bar
                    selectionbar = new UserControls.ucSelectionBar();
                    if (this.Width < 1280)
                        selectionbar.Width = 1280;
                    else
                        selectionbar.Width = this.Width;
                    selectionbar.Height = 380;
                    selectionbar.VerticalAlignment = VerticalAlignment.Center;
                    selectionbar.HorizontalAlignment = HorizontalAlignment.Center;
                    selectionbar.dsBackgroundColor = System.Windows.Media.Color.FromArgb(196, 0, 0, 0);
                    selectionbar.dsMainWindowWidth = this.Width;
                    List<string> captions = new List<string>();
                    List<string> thumbnails = new List<string>();
                    foreach (ScreenContent sc in CurrentScreen.ScreenContents)
                    {
                        captions.Add(sc.ScreenContentTitle);
                        foreach (Image image in CurrentScreen.Images)
                        {
                            if (sc.ThumbnailImageID == image.ImageID)
                            {
                                thumbnails.Add(DownloadManager.DownloadFolder + @"Images\" + image.StoredFilename);
                                break;
                            }
                        }
                    }
                    selectionbar.dsImageCaptions = captions;
                    selectionbar.dsImageURLs = thumbnails;
                    selectionbar.Visibility = Visibility.Collapsed;
                    gridScreenControls.Children.Add(selectionbar);
                    selectionbar.SelectionBarClosed += new RoutedEventHandler(selectionbar_SelectionBarClosed);
                    selectionbar.SelectionBarComplete += new RoutedEventHandler(selectionbar_SelectionBarComplete);
                    selectionbar.ResetControl();
                }

                gridScreenControls.Visibility = Visibility.Visible;
            }
            catch { }
        }

        void interactivebutton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                InteractiveButtonClick();
            }
            catch { }
        }

        void selectionbar_SelectionBarComplete(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenSelection();
            }
            catch { }
        }

        private void OpenSelection()
        {
            try
            {
                // Display the appropriate screen content
                int selectedindex = selectionbar.dsSelectionIndex;
                if (selectedindex < CurrentScreen.ScreenContents.Count)
                {
                    DisplayScreenContent(selectedindex);
                    selectionbar.FadeOut();
                    selectionbar.StopTimer();
                    StopMainScreen();
                }
            }
            catch { }
        }

        void selectionbar_SelectionBarClosed(object sender, RoutedEventArgs e)
        {
            try
            {
                selectionbar.FadeOut();

                // Restart the main playlist or slideshow and hide the selection bar
                ResumeMainScreen();
            }
            catch { }
        }

        private void DisplayScreenContent(int selectedindex)
        {
            try
            {
                contenttimer.Stop();

                // Screen Contents should be displayed 'On Demand' and not pre-loaded
                if (CurrentScreen.ScreenContents[selectedindex].ScreenContentTypeID == 1000000) // Image
                {
                    lastcontentcontroltype = "Image";

                    string imagepath = String.Empty;
                    foreach (Image image in CurrentSchedule.Images)
                    {
                        if (Convert.ToInt32(CurrentScreen.ScreenContents[selectedindex].CustomField1) == image.ImageID)
                        {
                            imagepath = DownloadManager.DownloadFolder + @"Images\" + image.StoredFilename;
                            break;
                        }
                    }

                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    img.Width = this.Width;
                    img.Height = this.Height;
                    img.Source = new BitmapImage(new Uri(imagepath));
                    img.Stretch = Stretch.Fill;
                    img.VerticalAlignment = VerticalAlignment.Center;
                    img.HorizontalAlignment = HorizontalAlignment.Center;

                    gridContent.Children.Add(img);

                    ContentFadeIn();
                    gridClose.Visibility = Visibility.Visible;

                    contenttimer.Start();
                }
                else if (CurrentScreen.ScreenContents[selectedindex].ScreenContentTypeID == 1000001) // SlideShow
                {
                    int slideshowid = Convert.ToInt32(CurrentScreen.ScreenContents[selectedindex].CustomField1);

                    // Get the images to display
                    List<string> images = new List<string>();
                    foreach (SlideShowImageXref xref in CurrentScreen.SlideShowImageXrefs)
                    {
                        if (slideshowid == xref.SlideShowID)
                        {
                            foreach (Image image in CurrentScreen.Images)
                            {
                                if (xref.ImageID == image.ImageID)
                                {
                                    images.Add(DownloadManager.DownloadFolder + @"Images\" + image.StoredFilename);
                                    break;
                                }
                            }
                        }
                    }

                    // Get the music to play
                    List<string> musics = new List<string>();
                    foreach (SlideShowMusicXref xref in CurrentScreen.SlideShowMusicXrefs)
                    {
                        if (slideshowid == xref.SlideShowID)
                        {
                            foreach (Music music in CurrentScreen.Musics)
                            {
                                if (xref.MusicID == music.MusicID)
                                {
                                    musics.Add(DownloadManager.DownloadFolder + @"Music\" + music.StoredFilename);
                                    break;
                                }
                            }
                        }
                    }

                    // Determine the type of slideshow to create - "Fade", "Drop From Top", "Slide From Right", "Pan Zoom", "Zoom In"
                    SlideShow slideshow = new SlideShow();
                    foreach (SlideShow ss in CurrentScreen.SlideShows)
                    {
                        if (slideshowid == ss.SlideShowID)
                            slideshow = ss;
                    }

                    if (slideshow.TransitionType == "Drop From Top")
                    {
                        lastcontentcontroltype = "ucSlideShowDropFromTop";

                        UserControls.ucSlideShowDropFromTop drop = new UserControls.ucSlideShowDropFromTop();
                        drop.Width = this.Width;
                        drop.Height = this.Height;
                        drop.VerticalAlignment = VerticalAlignment.Top;
                        drop.HorizontalAlignment = HorizontalAlignment.Left;
                        drop.dsSlideDurationInSeconds = slideshow.IntervalInSecs;
                        drop.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                        drop.dsImageURLs = images;
                        drop.dsMusicURLs = musics;
                        drop.dsImageFillMode = imagefillmode;
                        drop.dsFireCompleteEvent = true;
                        drop.SlideShowComplete += new RoutedEventHandler(slideshow_SlideShowComplete);
                        drop.Visibility = Visibility.Visible;
                        gridContent.Children.Add(drop);
                        drop.ResetControl();
                    }
                    else if (slideshow.TransitionType == "Slide From Right")
                    {
                        lastcontentcontroltype = "ucSlideShowSlideFromRight";

                        UserControls.ucSlideShowSlideFromRight slide = new UserControls.ucSlideShowSlideFromRight();
                        slide.Width = this.Width;
                        slide.Height = this.Height;
                        slide.VerticalAlignment = VerticalAlignment.Top;
                        slide.HorizontalAlignment = HorizontalAlignment.Left;
                        slide.dsSlideDurationInSeconds = slideshow.IntervalInSecs;
                        slide.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                        slide.dsImageURLs = images;
                        slide.dsMusicURLs = musics;
                        slide.dsImageFillMode = imagefillmode;
                        slide.dsFireCompleteEvent = true;
                        slide.SlideShowComplete += new RoutedEventHandler(slideshow_SlideShowComplete);
                        slide.Visibility = Visibility.Visible;
                        gridContent.Children.Add(slide);
                        slide.ResetControl();
                    }
                    else if (slideshow.TransitionType == "Pan Zoom")
                    {
                        lastcontentcontroltype = "ucSlideShowPanZoom";

                        UserControls.ucSlideShowPanZoom panzoom = new UserControls.ucSlideShowPanZoom();
                        panzoom.Width = this.Width;
                        panzoom.Height = this.Height;
                        panzoom.VerticalAlignment = VerticalAlignment.Top;
                        panzoom.HorizontalAlignment = HorizontalAlignment.Left;
                        panzoom.dsSlideDurationInSeconds = slideshow.IntervalInSecs;
                        panzoom.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                        panzoom.dsImageURLs = images;
                        panzoom.dsMusicURLs = musics;
                        panzoom.dsImageFillMode = imagefillmode;
                        panzoom.dsFireCompleteEvent = true;
                        panzoom.SlideShowComplete += new RoutedEventHandler(slideshow_SlideShowComplete);
                        panzoom.Visibility = Visibility.Visible;
                        gridContent.Children.Add(panzoom);
                        panzoom.ResetControl();
                    }
                    else if (slideshow.TransitionType == "Zoom In")
                    {
                        lastcontentcontroltype = "ucSlideShowZoomIn";

                        UserControls.ucSlideShowZoomIn zoomin = new UserControls.ucSlideShowZoomIn();
                        zoomin.Width = this.Width;
                        zoomin.Height = this.Height;
                        zoomin.VerticalAlignment = VerticalAlignment.Top;
                        zoomin.HorizontalAlignment = HorizontalAlignment.Left;
                        zoomin.dsSlideDurationInSeconds = slideshow.IntervalInSecs;
                        zoomin.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                        zoomin.dsImageURLs = images;
                        zoomin.dsMusicURLs = musics;
                        zoomin.dsImageFillMode = imagefillmode;
                        zoomin.dsFireCompleteEvent = true;
                        zoomin.SlideShowComplete += new RoutedEventHandler(slideshow_SlideShowComplete);
                        zoomin.Visibility = Visibility.Visible;
                        gridContent.Children.Add(zoomin);
                        zoomin.ResetControl();
                    }
                    else // Fade
                    {
                        lastcontentcontroltype = "ucSlideShowFader";

                        UserControls.ucSlideShowFader fader = new UserControls.ucSlideShowFader();
                        fader.Width = this.Width;
                        fader.Height = this.Height;
                        fader.VerticalAlignment = VerticalAlignment.Top;
                        fader.HorizontalAlignment = HorizontalAlignment.Left;
                        fader.dsSlideDurationInSeconds = slideshow.IntervalInSecs;
                        fader.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                        fader.dsImageURLs = images;
                        fader.dsMusicURLs = musics;
                        fader.dsImageFillMode = imagefillmode;
                        fader.dsFireCompleteEvent = true;
                        fader.SlideShowComplete += new RoutedEventHandler(slideshow_SlideShowComplete);
                        fader.Visibility = Visibility.Visible;
                        gridContent.Children.Add(fader);
                        fader.ResetControl();
                    }
                    ContentFadeIn();
                    gridClose.Visibility = Visibility.Visible;
                }
                else if (CurrentScreen.ScreenContents[selectedindex].ScreenContentTypeID == 1000002) // Video
                {
                    lastcontentcontroltype = "Video";

                    int videoid = Convert.ToInt32(CurrentScreen.ScreenContents[selectedindex].CustomField1);

                    // Get the videos to display
                    List<string> videos = new List<string>();
                    foreach (Video video in CurrentScreen.Videos)
                    {
                        if (videoid == video.VideoID)
                        {
                            videos.Add(DownloadManager.DownloadFolder + @"Videos\" + video.StoredFilename);
                            break;
                        }
                    }

                    UserControls.ucPlayList ucplaylist = new UserControls.ucPlayList();
                    ucplaylist.Width = this.Width;
                    ucplaylist.Height = this.Height;
                    ucplaylist.dsVideoURLs = videos;
                    ucplaylist.dsFireCompleteEvent = true;
                    ucplaylist.PlayListComplete += new RoutedEventHandler(ucplaylist_PlayListComplete);
                    ucplaylist.Visibility = Visibility.Visible;
                    gridContent.Children.Add(ucplaylist);
                    ucplaylist.ResetControl();
                    ContentFadeIn();
                    gridClose.Visibility = Visibility.Visible;
                }
                else if (CurrentScreen.ScreenContents[selectedindex].ScreenContentTypeID == 1000003) // PlayList
                {
                    lastcontentcontroltype = "ucPlayList";

                    int playlistid = Convert.ToInt32(CurrentScreen.ScreenContents[selectedindex].CustomField1);

                    // Get the videos to display
                    List<string> videos = new List<string>();
                    foreach (PlayListVideoXref xref in CurrentScreen.PlayListVideoXrefs)
                    {
                        if (playlistid == xref.PlayListID)
                        {
                            foreach (Video video in CurrentScreen.Videos)
                            {
                                if (xref.VideoID == video.VideoID)
                                {
                                    videos.Add(DownloadManager.DownloadFolder + @"Videos\" + video.StoredFilename);
                                    break;
                                }
                            }
                        }
                    }

                    UserControls.ucPlayList ucplaylist = new UserControls.ucPlayList();
                    ucplaylist.Width = this.Width;
                    ucplaylist.Height = this.Height;
                    ucplaylist.dsVideoURLs = videos;
                    ucplaylist.dsFireCompleteEvent = true;
                    ucplaylist.PlayListComplete += new RoutedEventHandler(ucplaylist_PlayListComplete);
                    ucplaylist.Visibility = Visibility.Visible;
                    gridContent.Children.Add(ucplaylist);
                    ucplaylist.ResetControl();
                    ContentFadeIn();
                    gridClose.Visibility = Visibility.Visible;
                }
                else if (CurrentScreen.ScreenContents[selectedindex].ScreenContentTypeID == 1000004) // Web Site
                {
                    lastcontentcontroltype = "ucWeb";

                    string website = CurrentScreen.ScreenContents[selectedindex].CustomField1;

                    UserControls.ucWeb ucweb = new UserControls.ucWeb();
                    ucweb.Width = this.Width;
                    ucweb.Height = this.Height;
                    ucweb.VerticalAlignment = VerticalAlignment.Top;
                    ucweb.HorizontalAlignment = HorizontalAlignment.Left;
                    ucweb.Margin = new Thickness(0, 0, 0, 0);
                    ucweb.dsWebsiteUrl = website;
                    ucweb.ResetControl();
                    ucweb.Visibility = Visibility.Visible;
                    gridContent.Children.Add(ucweb);
                    ContentFadeIn();
                    gridClose.Visibility = Visibility.Visible;

                    contenttimer.Start();
                }
                else if (CurrentScreen.ScreenContents[selectedindex].ScreenContentTypeID == 1000007) // Survey
                {
                    // Unlike the other controls, the survey control is not in the controls library
                    int surveyid = Convert.ToInt32(CurrentScreen.ScreenContents[selectedindex].CustomField1);

                    // Assign the appropriate survey, questions, and options to the control
                    foreach (Survey survey in CurrentScreen.Surveys)
                    {
                        if (survey.SurveyID == surveyid)
                            ucSurvey.dsSurvey = survey;
                    }

                    ucSurvey.dsSurveyQuestions = new List<SurveyQuestion>();
                    ucSurvey.dsSurveyQuestionOptions = new List<SurveyQuestionOption>();

                    foreach (SurveyQuestion question in CurrentScreen.SurveyQuestions)
                    {
                        if (question.SurveyID == ucSurvey.dsSurvey.SurveyID)
                            ucSurvey.dsSurveyQuestions.Add(question);
                    }

                    foreach (SurveyQuestion question in ucSurvey.dsSurveyQuestions)
                    {
                        foreach (SurveyQuestionOption option in CurrentScreen.SurveyQuestionOptions)
                        {
                            if (option.SurveyQuestionID == question.SurveyQuestionID)
                                ucSurvey.dsSurveyQuestionOptions.Add(option);
                        }
                    }

                    ucSurvey.ResetControl();
                    ucSurvey.FadeIn();
                }
                else if (CurrentScreen.ScreenContents[selectedindex].ScreenContentTypeID == 1000008) // Timeline
                {
                    lastcontentcontroltype = "ucTimeline";

                    int timelineid = Convert.ToInt32(CurrentScreen.ScreenContents[selectedindex].CustomField1);

                    // Get the timeline to display
                    Timeline timeline = new Timeline();
                    foreach (Timeline tl in CurrentScreen.Timelines)
                    {
                        if (CurrentScreen.ScreenInfo.TimelineID == tl.TimelineID)
                            timeline = tl;
                    }

                    List<TimelineMedia> timelinemedia = new List<TimelineMedia>();

                    // Get the videos to display
                    foreach (TimelineVideoXref xref in CurrentScreen.TimelineVideoXrefs)
                    {
                        if (CurrentScreen.ScreenInfo.TimelineID == xref.TimelineID)
                        {
                            foreach (Video video in CurrentScreen.Videos)
                            {
                                if (xref.VideoID == video.VideoID)
                                {
                                    TimelineMedia tlm = new TimelineMedia();
                                    tlm.StoredFilename = DownloadManager.DownloadFolder + @"Videos\" + video.StoredFilename;
                                    tlm.DisplayOrder = xref.DisplayOrder;
                                    timelinemedia.Add(tlm);
                                    break;
                                }
                            }
                        }
                    }

                    // Get the images to display
                    List<string> images = new List<string>();
                    foreach (TimelineImageXref xref in CurrentScreen.TimelineImageXrefs)
                    {
                        if (CurrentScreen.ScreenInfo.TimelineID == xref.TimelineID)
                        {
                            foreach (Image image in CurrentScreen.Images)
                            {
                                if (xref.ImageID == image.ImageID)
                                {
                                    TimelineMedia tlm = new TimelineMedia();
                                    tlm.StoredFilename = DownloadManager.DownloadFolder + @"Images\" + image.StoredFilename;
                                    tlm.DisplayOrder = xref.DisplayOrder;
                                    timelinemedia.Add(tlm);
                                    break;
                                }
                            }
                        }
                    }

                    timelinemedia.Sort(delegate(TimelineMedia tlm1, TimelineMedia tlm2)
                    { return tlm1.DisplayOrder.CompareTo(tlm2.DisplayOrder); });

                    List<string> tlmedia = new List<string>();
                    foreach (TimelineMedia tlm in timelinemedia)
                    {
                        tlmedia.Add(tlm.StoredFilename);
                    }

                    // Get the music to play
                    List<string> musics = new List<string>();
                    foreach (TimelineMusicXref xref in CurrentScreen.TimelineMusicXrefs)
                    {
                        if (CurrentScreen.ScreenInfo.TimelineID == xref.TimelineID)
                        {
                            foreach (Music music in CurrentScreen.Musics)
                            {
                                if (xref.MusicID == music.MusicID)
                                {
                                    musics.Add(DownloadManager.DownloadFolder + @"Music\" + music.StoredFilename);
                                    break;
                                }
                            }
                        }
                    }

                    UserControls.ucTimeline uctimeline = new UserControls.ucTimeline();
                    uctimeline.Width = this.Width;
                    uctimeline.Height = this.Height;
                    uctimeline.dsBackgroundColor = System.Windows.Media.Color.FromArgb(255, 0, 0, 0);
                    uctimeline.dsMediaURLs = tlmedia;
                    uctimeline.dsMusicURLs = musics;
                    uctimeline.dsFireCompleteEvent = true;
                    uctimeline.dsMuteMusicOnPlayback = timeline.MuteMusicOnPlayback;
                    uctimeline.dsSlideDurationInSeconds = timeline.DurationInSecs;
                    uctimeline.Visibility = Visibility.Visible;
                    uctimeline.TimelineComplete += uctimeline_TimelineComplete;
                    gridContent.Children.Add(uctimeline);
                    uctimeline.ResetControl();
                    ContentFadeIn();
                    gridClose.Visibility = Visibility.Visible;

                }

                // Create the screen content log
                CreateScreenContentLog(selectedindex);
            }
            catch { }
        }

        public async void CreateScreenContentLog(int selectedindex)
        {
            try
            {
                osVodigiWS.PlayerScreenContentLog_CreateResponse response = await ws.PlayerScreenContentLog_CreateAsync(PlayerConfiguration.configAccountID,
                                            PlayerConfiguration.configPlayerID,
                                            PlayerConfiguration.configPlayerName,
                                            CurrentScreen.ScreenInfo.ScreenID,
                                            CurrentScreen.ScreenInfo.ScreenName,
                                            CurrentScreen.ScreenContents[selectedindex].ScreenContentID,
                                            CurrentScreen.ScreenContents[selectedindex].ScreenContentName,
                                            CurrentScreen.ScreenContents[selectedindex].ScreenContentTypeID,
                                            CurrentScreen.ScreenContents[selectedindex].ScreenContentTypeName,
                                            DateTime.UtcNow,
                                            DateTime.UtcNow,
                                            String.Empty);

                playerscreencontentlogid = Convert.ToInt32(response.Body.PlayerScreenContentLog_CreateResult);
            }
            catch { }
        }

        public void ContentFadeIn()
        {
            try
            {
                gridContent.Opacity = 0;
                gridContent.Visibility = Visibility.Visible;
                sbContentFadeIn.Begin();
            }
            catch { }
        }

        void ucplaylist_PlayListComplete(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseScreenContent();
            }
            catch { }
        }

        void slideshow_SlideShowComplete(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseScreenContent();
            }
            catch { }
        }

        void uctimeline_TimelineComplete(object sender, RoutedEventArgs e)
        {
            try
            {
                CloseScreenContent();
            }
            catch { }
        }


        void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                // Resize the children controls
                ResizeChildrenControls();
            }
            catch { }
        }

        void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                // F12 = Exit
                if (e.Key == Key.F12 || e.SystemKey == Key.F12)
                {
                    MessageBoxResult mbr = MessageBox.Show("Do you want to exit the application?", "Exit Application", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (mbr == MessageBoxResult.Yes)
                        Application.Current.Shutdown();
                }
                // F11 = Reset Player
                else if (e.Key == Key.F11 || e.SystemKey == Key.F11)
                {
                    // Only show reset player when content is not showing
                    if (gridContent.Visibility != Visibility.Visible)
                    {
                        StopMainScreen();
                        ucResetPlayer.Visibility = Visibility.Visible;
                    }
                }
                // F10 - Force Schedule and Screen Refresh
                else if (e.Key == Key.F10 || e.SystemKey == Key.F10)
                {
                    MessageBoxResult start = MessageBox.Show("Do you want to force a schedule refresh?", "Schedule Refresh", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (start == MessageBoxResult.Yes)
                        InitializeApplication();
                }
            }
            catch { }
        }

        void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                // Position the mouse countdown grid based on the mouse position
                System.Windows.Point pt = e.GetPosition(this);
                gridMouseCounter.Margin = new Thickness(pt.X - 36, pt.Y - 36, 0, 0);
            }
            catch { }
        }

        void ucSettings_SettingsComplete(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeApplication();
            }
            catch { }
        }

        void ucSurvey_SurveyClosed(object sender, RoutedEventArgs e)
        {
            try
            {
                ResumeMainScreen();
            }
            catch { }
        }

        void sbContentFadeOut_Completed(object sender, EventArgs e)
        {
            try
            {
                gridContent.Visibility = Visibility.Collapsed;
                gridContent.Children.Clear();
            }
            catch { }
        }

        void heartbeat_Tick(object sender, EventArgs e)
        {
            try
            {
                // Send the heartbeat log message at the top of every hour
                if (DateTime.Now.Minute == 0)
                {
                    CreateHeartbeatLog();
                }
            }
            catch { }
        }

        void screencheck_Tick(object sender, EventArgs e)
        {
            try
            {
                DateTime now = DateTime.Now;

                // Get the player schedule at the specified interval
                if (now > lastschedulecheck.AddMinutes(schedulecheckinterval))
                {
                    try
                    {
                        bool initialize = GetPlayerSchedule();
                        if (initialize)
                            InitializeApplication();
                    }
                    catch { }
                    lastschedulecheck = now;
                }

                LoadCurrentScreen(now, false);
            }
            catch { }
        }

    }
}
