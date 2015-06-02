using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;
using osVodigiWeb6x.Models;

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

namespace osVodigiWeb6x
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]

    public class osVodigiService : System.Web.Services.WebService
    {

        [WebMethod]
        public Account Account_GetByName(string accountname)
        {
            try
            {
                IAccountRepository accountrep = new EntityAccountRepository();
                List<Account> accounts = accountrep.GetAccountByName(accountname).ToList();
                if (accounts == null || accounts.Count == 0)
                    return null;
                else
                    return accounts[0];
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public void ActivityLog_Create(int accountid, int userid, string entitytype, string entityaction, DateTime activitydatetime, string activitydetails)
        {
            try
            {
                IActivityLogRepository activityrep = new EntityActivityLogRepository();

                ActivityLog activitylog = new ActivityLog();
                activitylog.AccountID = accountid;
                activitylog.UserID = userid;
                activitylog.Username = "Vodigi Player";
                activitylog.EntityType = entitytype;
                activitylog.EntityAction = entityaction;
                activitylog.ActivityDateTime = activitydatetime;
                activitylog.ActivityDetails = activitydetails;

                activityrep.CreateActivityLog(activitylog);
            }
            catch { }
        }

        [WebMethod]
        public int AnsweredSurvey_Create(int accountid, int surveyid, int playerid)
        {
            try
            {
                IAnsweredSurveyRepository answeredsurveyrep = new EntityAnsweredSurveyRepository();

                AnsweredSurvey answeredsurvey = new AnsweredSurvey();
                answeredsurvey.AccountID = accountid;
                answeredsurvey.SurveyID = surveyid;
                answeredsurvey.PlayerID = playerid;
                answeredsurvey.CreatedDateTime = DateTime.UtcNow;

                answeredsurveyrep.CreateAnsweredSurvey(answeredsurvey);

                return answeredsurvey.AnsweredSurveyID;
            }
            catch { return 0; }
        }

        [WebMethod]
        public int AnsweredSurveyQuestionOption_Create(int answeredsurveyid, int surveyquestionoptionid, bool isselected)
        {
            try
            {
                IAnsweredSurveyQuestionOptionRepository optionrep = new EntityAnsweredSurveyQuestionOptionRepository();

                AnsweredSurveyQuestionOption option = new AnsweredSurveyQuestionOption();
                option.AnsweredSurveyID = answeredsurveyid;
                option.SurveyQuestionOptionID = surveyquestionoptionid;
                option.IsSelected = isselected;

                optionrep.CreateAnsweredSurveyQuestionOption(option);

                return option.AnsweredSurveyQuestionOptionID;
            }
            catch { return 0; }
        }

        [WebMethod]
        public DatabaseVersion DatabaseVersion_Get()
        {
            try
            {
                IDatabaseVersionRepository dbrep = new EntityDatabaseVersionRepository();
                return dbrep.GetDatabaseVersion();
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public Player Player_GetByName(int accountid, string playername)
        {
            try
            {
                IPlayerRepository playerrep = new EntityPlayerRepository();
                List<Player> players = playerrep.GetPlayerByName(accountid, playername).ToList();
                if (players == null || players.Count == 0)
                    return null;
                else
                    return players[0];
            }
            catch
            {
                return null;
            }
        }

        [WebMethod]
        public string Player_GetCurrentSchedule(int playerid)
        {
            try
            {
                IImageRepository imagerep = new EntityImageRepository();
                IPlayerRepository playerrep = new EntityPlayerRepository();
                IPlayerGroupScheduleRepository playergroupschedulerep = new EntityPlayerGroupScheduleRepository();
                IPlayListRepository playlistrep = new EntityPlayListRepository();
                IPlayListVideoXrefRepository playlistvideoxrefrep = new EntityPlayListVideoXrefRepository();
                IScreenScreenContentXrefRepository screenscreencontentxrefrep = new EntityScreenScreenContentXrefRepository();
                IScreenContentRepository screencontentrep = new EntityScreenContentRepository();
                IScreenContentTypeRepository screencontenttyperep = new EntityScreenContentTypeRepository();
                IScreenRepository screenrep = new EntityScreenRepository();
                ISlideShowRepository slideshowrep = new EntitySlideShowRepository();
                ISlideShowImageXrefRepository slideshowimagexrefrep = new EntitySlideShowImageXrefRepository();
                ISlideShowMusicXrefRepository slideshowmusicxrefrep = new EntitySlideShowMusicXrefRepository();
                ITimelineRepository timelinerep = new EntityTimelineRepository();
                ITimelineImageXrefRepository timelineimagexrefrep = new EntityTimelineImageXrefRepository();
                ITimelineVideoXrefRepository timelinevideoxrefrep = new EntityTimelineVideoXrefRepository();
                ITimelineMusicXrefRepository timelinemusicxrefrep = new EntityTimelineMusicXrefRepository();
                ISurveyRepository surveyrep = new EntitySurveyRepository();
                ISurveyQuestionRepository surveyquestionrep = new EntitySurveyQuestionRepository();
                ISurveyQuestionOptionRepository surveyquestionoptionrep = new EntitySurveyQuestionOptionRepository();
                IVideoRepository videorep = new EntityVideoRepository();
                IMusicRepository musicrep = new EntityMusicRepository();
                IPlayerSettingRepository playersettingrep = new EntityPlayerSettingRepository();
                IPlayerSettingSystemDefaultRepository playersettingsystemrep = new EntityPlayerSettingSystemDefaultRepository();
                IPlayerSettingAccountDefaultRepository playersettingaccountrep = new EntityPlayerSettingAccountDefaultRepository();

                // returns the summarized schedule information for the player
                List<Image> images = new List<Image>();
                List<PlayerGroupSchedule> playergroupschedules = new List<PlayerGroupSchedule>();
                List<PlayList> playlists = new List<PlayList>();
                List<PlayListVideoXref> playlistvideoxrefs = new List<PlayListVideoXref>();
                List<ScreenContent> screencontents = new List<ScreenContent>();
                List<ScreenScreenContentXref> screenscreencontentxrefs = new List<ScreenScreenContentXref>();
                List<Screen> screens = new List<Screen>();
                List<SlideShow> slideshows = new List<SlideShow>();
                List<SlideShowImageXref> slideshowimagexrefs = new List<SlideShowImageXref>();
                List<SlideShowMusicXref> slideshowmusicxrefs = new List<SlideShowMusicXref>();
                List<Timeline> timelines = new List<Timeline>();
                List<TimelineImageXref> timelineimagexrefs = new List<TimelineImageXref>();
                List<TimelineVideoXref> timelinevideoxrefs = new List<TimelineVideoXref>();
                List<TimelineMusicXref> timelinemusicxrefs = new List<TimelineMusicXref>();
                List<Survey> surveys = new List<Survey>();
                List<SurveyQuestion> surveyquestions = new List<SurveyQuestion>();
                List<SurveyQuestionOption> surveyquestionoptions = new List<SurveyQuestionOption>();
                List<Video> videos = new List<Video>();
                List<Music> musics = new List<Music>();

                StringBuilder sb = new StringBuilder();
                sb.Append("<xml>");

                // Player Schedule Info --------------------------------------------------------------------------------------

                // Get the PlayerGroupID - Player should only exist in one Player Group
                Player player = playerrep.GetPlayer(playerid);
                if (player == null)
                    throw new Exception("No player found.");

                // Get the PlayerGroupSchedule for this player
                playergroupschedules = playergroupschedulerep.GetPlayerGroupSchedulesByPlayerGroup(player.PlayerGroupID).ToList();
                if (playergroupschedules == null || playergroupschedules.Count == 0)
                    throw new Exception("No schedule found for this player.");

                sb.Append("<PlayerGroupSchedules>");
                foreach (PlayerGroupSchedule playergroupschedule in playergroupschedules)
                {
                    sb.Append("<PlayerGroupSchedule ");
                    sb.Append("PlayerGroupScheduleID=\"" + playergroupschedule.PlayerGroupScheduleID.ToString() + "\" ");
                    sb.Append("PlayerGroupID=\"" + playergroupschedule.PlayerGroupID.ToString() + "\" ");
                    sb.Append("ScreenID=\"" + playergroupschedule.ScreenID.ToString() + "\" ");
                    sb.Append("Day=\"" + playergroupschedule.Day.ToString() + "\" ");
                    sb.Append("Hour=\"" + playergroupschedule.Hour.ToString() + "\" ");
                    sb.Append("Minute=\"" + playergroupschedule.Minute.ToString() + "\" ");
                    sb.Append(" />");

                    // Add the screen to the screens list
                    if (playergroupschedule.ScreenID > 0)
                        screens.Add(screenrep.GetScreen(playergroupschedule.ScreenID));
                }
                sb.Append("</PlayerGroupSchedules>");

                // Screens --------------------------------------------------------------------------------------
                screens = screens.Distinct().ToList();
                sb.Append("<Screens>");
                foreach (Screen screen in screens)
                {
                    sb.Append("<Screen ");
                    sb.Append("ScreenID=\"" + screen.ScreenID.ToString() + "\" ");
                    sb.Append("AccountID=\"" + screen.AccountID.ToString() + "\" ");
                    sb.Append("ScreenName=\"" + Utility.EncodeXMLString(screen.ScreenName) + "\" ");
                    sb.Append("SlideShowID=\"" + screen.SlideShowID.ToString() + "\" ");
                    sb.Append("PlayListID=\"" + screen.PlayListID.ToString() + "\" ");
                    sb.Append("TimelineID=\"" + screen.TimelineID.ToString() + "\" ");
                    string interactive = "true";
                    if (!screen.IsInteractive) interactive = "false";
                    sb.Append("IsInteractive=\"" + interactive + "\" ");
                    sb.Append("ButtonImageID=\"" + screen.ButtonImageID.ToString() + "\" ");
                    sb.Append(" />");

                    // Save the SlideShow
                    if (screen.SlideShowID != 0)
                        slideshows.Add(slideshowrep.GetSlideShow(screen.SlideShowID));

                    // Save the PlayList
                    if (screen.PlayListID != 0)
                        playlists.Add(playlistrep.GetPlayList(screen.PlayListID));

                    // Save the Timeline
                    if (screen.TimelineID != 0)
                        timelines.Add(timelinerep.GetTimeline(screen.TimelineID));

                    // Save the screen button image
                    if (screen.ButtonImageID != 0)
                        images.Add(imagerep.GetImage(screen.ButtonImageID));

                    // Save the ScreenContentXrefs
                    List<ScreenScreenContentXref> sscxrefs = screenscreencontentxrefrep.GetScreenScreenContentXrefs(screen.ScreenID).ToList();
                    foreach (ScreenScreenContentXref sscxref in sscxrefs)
                    {
                        // Save to the xref
                        screenscreencontentxrefs.Add(sscxref);
                    }
                }
                sb.Append("</Screens>");


                // ScreenScreenContentXrefs -----------------------------------------------------------------------------
                sb.Append("<ScreenScreenContentXrefs>");
                foreach (ScreenScreenContentXref sscxref in screenscreencontentxrefs)
                {
                    sb.Append("<ScreenScreenContentXref ");
                    sb.Append("ScreenScreenContentXrefID=\"" + sscxref.ScreenScreenContentXrefID.ToString() + "\" ");
                    sb.Append("ScreenID=\"" + sscxref.ScreenID.ToString() + "\" ");
                    sb.Append("ScreenContentID=\"" + sscxref.ScreenContentID.ToString() + "\" ");
                    sb.Append("DisplayOrder=\"" + sscxref.DisplayOrder.ToString() + "\" ");
                    sb.Append(" />");

                    // Save the screen content
                    screencontents.Add(screencontentrep.GetScreenContent(sscxref.ScreenContentID));
                }
                sb.Append("</ScreenScreenContentXrefs>");


                // ScreenContents -------------------------------------------------------------------------------------
                screencontents = screencontents.Distinct().ToList();
                sb.Append("<ScreenContents>");
                foreach (ScreenContent sc in screencontents)
                {
                    ScreenContentType sctype = screencontenttyperep.GetScreenContentType(sc.ScreenContentTypeID);

                    sb.Append("<ScreenContent ");
                    sb.Append("ScreenContentID=\"" + sc.ScreenContentID.ToString() + "\" ");
                    sb.Append("ScreenContentTypeID=\"" + sc.ScreenContentTypeID.ToString() + "\" ");
                    sb.Append("ScreenContentTypeName=\"" + Utility.EncodeXMLString(sctype.ScreenContentTypeName) + "\" ");
                    sb.Append("ScreenContentName=\"" + Utility.EncodeXMLString(sc.ScreenContentName) + "\" ");
                    sb.Append("ScreenContentTitle=\"" + Utility.EncodeXMLString(sc.ScreenContentTitle) + "\" ");
                    sb.Append("ThumbnailImageID=\"" + sc.ThumbnailImageID.ToString() + "\" ");
                    sb.Append("CustomField1=\"" + Utility.EncodeXMLString(sc.CustomField1) + "\" ");
                    sb.Append("CustomField2=\"" + Utility.EncodeXMLString(sc.CustomField2) + "\" ");
                    sb.Append("CustomField3=\"" + Utility.EncodeXMLString(sc.CustomField3) + "\" ");
                    sb.Append("CustomField4=\"" + Utility.EncodeXMLString(sc.CustomField4) + "\" ");
                    sb.Append(" />");

                    // Add the Thumbnail Image
                    if (sc.ThumbnailImageID != 0)
                        images.Add(imagerep.GetImage(sc.ThumbnailImageID));

                    // If Image, add the image
                    if (sc.ScreenContentTypeID == 1000000 && !String.IsNullOrEmpty(sc.CustomField1))
                        images.Add(imagerep.GetImage(Convert.ToInt32(sc.CustomField1)));

                    // If Slideshow, add the slideshow
                    if (sc.ScreenContentTypeID == 1000001 && !String.IsNullOrEmpty(sc.CustomField1))
                        slideshows.Add(slideshowrep.GetSlideShow(Convert.ToInt32(sc.CustomField1)));

                    // If Video, add the video
                    if (sc.ScreenContentTypeID == 1000002 && !String.IsNullOrEmpty(sc.CustomField1))
                        videos.Add(videorep.GetVideo(Convert.ToInt32(sc.CustomField1)));

                    // If PlayList, add the playlist
                    if (sc.ScreenContentTypeID == 1000003 && !String.IsNullOrEmpty(sc.CustomField1))
                        playlists.Add(playlistrep.GetPlayList(Convert.ToInt32(sc.CustomField1)));

                    // If Survey, add the survey and its image
                    if (sc.ScreenContentTypeID == 1000007 && !String.IsNullOrEmpty(sc.CustomField1))
                    {
                        Survey survey = surveyrep.GetSurvey(Convert.ToInt32(sc.CustomField1));
                        images.Add(imagerep.GetImage(survey.SurveyImageID));
                        surveys.Add(survey);
                    }

                }
                sb.Append("</ScreenContents>");


                // Surveys ---------------------------------------------------------------------------------
                surveys = surveys.Distinct().ToList();
                sb.Append("<Surveys>");
                foreach (Survey sv in surveys)
                {
                    sb.Append("<Survey ");
                    sb.Append("SurveyID=\"" + sv.SurveyID + "\" ");
                    sb.Append("SurveyName=\"" + Utility.EncodeXMLString(sv.SurveyName) + "\" ");
                    sb.Append("SurveyImageID=\"" + sv.SurveyImageID + "\" ");
                    sb.Append(" />");

                    List<SurveyQuestion> svqs = surveyquestionrep.GetSurveyQuestions(sv.SurveyID).ToList();
                    foreach (SurveyQuestion svq in svqs)
                    {
                        surveyquestions.Add(svq);
                    }
                }
                sb.Append("</Surveys>");


                // SurveyQuestions ----------------------------------------------------------------------------
                surveyquestions = surveyquestions.Distinct().ToList();
                sb.Append("<SurveyQuestions>");
                foreach (SurveyQuestion svq in surveyquestions)
                {
                    sb.Append("<SurveyQuestion ");
                    sb.Append("SurveyQuestionID=\"" + svq.SurveyQuestionID + "\" ");
                    sb.Append("SurveyID=\"" + svq.SurveyID + "\" ");
                    sb.Append("SurveyQuestionText=\"" + Utility.EncodeXMLString(svq.SurveyQuestionText) + "\" ");
                    sb.Append("AllowMultiselect=\"" + svq.AllowMultiSelect.ToString() + "\" ");
                    sb.Append("SortOrder=\"" + svq.SortOrder.ToString() + "\" ");
                    sb.Append(" />");

                    List<SurveyQuestionOption> svqos = surveyquestionoptionrep.GetSurveyQuestionOptions(svq.SurveyQuestionID).ToList();
                    foreach (SurveyQuestionOption svqo in svqos)
                    {
                        surveyquestionoptions.Add(svqo);
                    }
                }
                sb.Append("</SurveyQuestions>");


                // SurveyQuestionOptions ----------------------------------------------------------------------------
                surveyquestionoptions = surveyquestionoptions.Distinct().ToList();
                sb.Append("<SurveyQuestionOptions>");
                foreach (SurveyQuestionOption svqo in surveyquestionoptions)
                {
                    sb.Append("<SurveyQuestionOption ");
                    sb.Append("SurveyQuestionOptionID=\"" + svqo.SurveyQuestionOptionID + "\" ");
                    sb.Append("SurveyQuestionID=\"" + svqo.SurveyQuestionID + "\" ");
                    sb.Append("SurveyQuestionOptionText=\"" + Utility.EncodeXMLString(svqo.SurveyQuestionOptionText) + "\" ");
                    sb.Append("SortOrder=\"" + svqo.SortOrder.ToString() + "\" ");
                    sb.Append(" />");
                }
                sb.Append("</SurveyQuestionOptions>");


                // SlideShows ---------------------------------------------------------------------------------
                slideshows = slideshows.Distinct().ToList();
                sb.Append("<SlideShows>");
                foreach (SlideShow ss in slideshows)
                {
                    sb.Append("<SlideShow ");
                    sb.Append("SlideShowID=\"" + ss.SlideShowID.ToString() + "\" ");
                    sb.Append("IntervalInSecs=\"" + ss.IntervalInSecs.ToString() + "\" ");
                    sb.Append("TransitionType=\"" + Utility.EncodeXMLString(ss.TransitionType) + "\" ");
                    sb.Append(" />");

                    List<SlideShowImageXref> ssixrefs = slideshowimagexrefrep.GetSlideShowImageXrefs(ss.SlideShowID).ToList();
                    foreach (SlideShowImageXref ssixref in ssixrefs)
                    {
                        slideshowimagexrefs.Add(ssixref);
                    }

                    List<SlideShowMusicXref> ssmxrefs = slideshowmusicxrefrep.GetSlideShowMusicXrefs(ss.SlideShowID).ToList();
                    foreach (SlideShowMusicXref ssmxref in ssmxrefs)
                    {
                        slideshowmusicxrefs.Add(ssmxref);
                    }
                }
                sb.Append("</SlideShows>");


                // SlideshowImageXrefs ---------------------------------------------------------------------------------
                slideshowimagexrefs = slideshowimagexrefs.Distinct().ToList();
                sb.Append("<SlideShowImageXrefs>");
                foreach (SlideShowImageXref ssixref in slideshowimagexrefs)
                {
                    sb.Append("<SlideShowImageXref ");
                    sb.Append("SlideShowImageXrefID=\"" + ssixref.SlideShowImageXrefID.ToString() + "\" ");
                    sb.Append("SlideShowID=\"" + ssixref.SlideShowID.ToString() + "\" ");
                    sb.Append("ImageID=\"" + ssixref.ImageID.ToString() + "\" ");
                    sb.Append("PlayOrder=\"" + ssixref.PlayOrder.ToString() + "\" ");
                    sb.Append(" />");

                    // Add the image
                    images.Add(imagerep.GetImage(ssixref.ImageID));
                }
                sb.Append("</SlideShowImageXrefs>");


                // SlideshowMusicXrefs ---------------------------------------------------------------------------------
                slideshowmusicxrefs = slideshowmusicxrefs.Distinct().ToList();
                sb.Append("<SlideShowMusicXrefs>");
                foreach (SlideShowMusicXref ssmxref in slideshowmusicxrefs)
                {
                    sb.Append("<SlideShowMusicXref ");
                    sb.Append("SlideShowMusicXrefID=\"" + ssmxref.SlideShowMusicXrefID.ToString() + "\" ");
                    sb.Append("SlideShowID=\"" + ssmxref.SlideShowID.ToString() + "\" ");
                    sb.Append("MusicID=\"" + ssmxref.MusicID.ToString() + "\" ");
                    sb.Append("PlayOrder=\"" + ssmxref.PlayOrder.ToString() + "\" ");
                    sb.Append(" />");

                    // Add the music
                    musics.Add(musicrep.GetMusic(ssmxref.MusicID));
                }
                sb.Append("</SlideShowMusicXrefs>");


                // Timelines ---------------------------------------------------------------------------------
                timelines = timelines.Distinct().ToList();
                sb.Append("<Timelines>");
                foreach (Timeline tl in timelines)
                {
                    sb.Append("<Timeline ");
                    sb.Append("TimelineID=\"" + tl.TimelineID.ToString() + "\" ");
                    sb.Append("DurationInSecs=\"" + tl.DurationInSecs.ToString() + "\" ");
                    sb.Append("MuteMusicOnPlayback=\"" + (tl.MuteMusicOnPlayback ? "true": "false") + "\" ");
                    sb.Append(" />");

                    List<TimelineImageXref> tlixrefs = timelineimagexrefrep.GetTimelineImageXrefs(tl.TimelineID).ToList();
                    foreach (TimelineImageXref tlixref in tlixrefs)
                    {
                        timelineimagexrefs.Add(tlixref);
                    }

                    List<TimelineVideoXref> tlvxrefs = timelinevideoxrefrep.GetTimelineVideoXrefs(tl.TimelineID).ToList();
                    foreach (TimelineVideoXref tlvxref in tlvxrefs)
                    {
                        timelinevideoxrefs.Add(tlvxref);
                    }

                    List<TimelineMusicXref> tlmxrefs = timelinemusicxrefrep.GetTimelineMusicXrefs(tl.TimelineID).ToList();
                    foreach (TimelineMusicXref tlmxref in tlmxrefs)
                    {
                        timelinemusicxrefs.Add(tlmxref);
                    }
                }
                sb.Append("</Timelines>");


                // TimelineImageXrefs ---------------------------------------------------------------------------------
                timelineimagexrefs = timelineimagexrefs.Distinct().ToList();
                sb.Append("<TimelineImageXrefs>");
                foreach (TimelineImageXref tlixref in timelineimagexrefs)
                {
                    sb.Append("<TimelineImageXref ");
                    sb.Append("TimelineImageXrefID=\"" + tlixref.TimelineImageXrefID.ToString() + "\" ");
                    sb.Append("TimelineID=\"" + tlixref.TimelineID.ToString() + "\" ");
                    sb.Append("ImageID=\"" + tlixref.ImageID.ToString() + "\" ");
                    sb.Append("DisplayOrder=\"" + tlixref.DisplayOrder.ToString() + "\" ");
                    sb.Append(" />");

                    // Add the image
                    images.Add(imagerep.GetImage(tlixref.ImageID));
                }
                sb.Append("</TimelineImageXrefs>");


                // TimelineMusicXrefs ---------------------------------------------------------------------------------
                timelinemusicxrefs = timelinemusicxrefs.Distinct().ToList();
                sb.Append("<TimelineMusicXrefs>");
                foreach (TimelineMusicXref tlmxref in timelinemusicxrefs)
                {
                    sb.Append("<TimelineMusicXref ");
                    sb.Append("TimelineMusicXrefID=\"" + tlmxref.TimelineMusicXrefID.ToString() + "\" ");
                    sb.Append("TimelineID=\"" + tlmxref.TimelineID.ToString() + "\" ");
                    sb.Append("MusicID=\"" + tlmxref.MusicID.ToString() + "\" ");
                    sb.Append("PlayOrder=\"" + tlmxref.PlayOrder.ToString() + "\" ");
                    sb.Append(" />");

                    // Add the music
                    musics.Add(musicrep.GetMusic(tlmxref.MusicID));
                }
                sb.Append("</TimelineMusicXrefs>");


                // TimelineVideoXrefs ---------------------------------------------------------------------------------
                timelinevideoxrefs = timelinevideoxrefs.Distinct().ToList();
                sb.Append("<TimelineVideoXrefs>");
                foreach (TimelineVideoXref tlvxref in timelinevideoxrefs)
                {
                    sb.Append("<TimelineVideoXref ");
                    sb.Append("TimelineVideoXrefID=\"" + tlvxref.TimelineVideoXrefID.ToString() + "\" ");
                    sb.Append("TimelineID=\"" + tlvxref.TimelineID.ToString() + "\" ");
                    sb.Append("VideoID=\"" + tlvxref.VideoID.ToString() + "\" ");
                    sb.Append("DisplayOrder=\"" + tlvxref.DisplayOrder.ToString() + "\" ");
                    sb.Append(" />");

                    // Add the video
                    videos.Add(videorep.GetVideo(tlvxref.VideoID));
                }
                sb.Append("</TimelineVideoXrefs>");


                // Images ---------------------------------------------------------------------------------
                images = images.Distinct().ToList();
                sb.Append("<Images>");
                foreach (Image image in images)
                {
                    sb.Append("<Image ");
                    sb.Append("ImageID=\"" + image.ImageID.ToString() + "\" ");
                    sb.Append("StoredFilename=\"" + image.StoredFilename + "\" ");
                    sb.Append("ImageName=\"" + Utility.EncodeXMLString(image.ImageName) + "\" ");
                    sb.Append(" />");
                }
                sb.Append("</Images>");


                // PlayLists ---------------------------------------------------------------------------------
                playlists = playlists.Distinct().ToList();
                sb.Append("<PlayLists>");
                foreach (PlayList pl in playlists)
                {
                    sb.Append("<PlayList ");
                    sb.Append("PlayListID=\"" + pl.PlayListID.ToString() + "\" ");
                    sb.Append(" />");

                    List<PlayListVideoXref> plvxrefs = playlistvideoxrefrep.GetPlayListVideoXrefs(pl.PlayListID).ToList();
                    foreach (PlayListVideoXref plvxref in plvxrefs)
                    {
                        playlistvideoxrefs.Add(plvxref);
                    }
                }
                sb.Append("</PlayLists>");


                // PlaylistVideoXrefs ---------------------------------------------------------------------------------
                playlistvideoxrefs = playlistvideoxrefs.Distinct().ToList();
                sb.Append("<PlayListVideoXrefs>");
                foreach (PlayListVideoXref plvxref in playlistvideoxrefs)
                {
                    sb.Append("<PlayListVideoXref ");
                    sb.Append("PlayListVideoXrefID=\"" + plvxref.PlayListVideoXrefID.ToString() + "\" ");
                    sb.Append("PlayListID=\"" + plvxref.PlayListID.ToString() + "\" ");
                    sb.Append("VideoID=\"" + plvxref.VideoID.ToString() + "\" ");
                    sb.Append("PlayOrder=\"" + plvxref.PlayOrder.ToString() + "\" ");
                    sb.Append(" />");

                    videos.Add(videorep.GetVideo(plvxref.VideoID));
                }
                sb.Append("</PlayListVideoXrefs>");


                // Videos ---------------------------------------------------------------------------------
                videos = videos.Distinct().ToList();
                sb.Append("<Videos>");
                foreach (Video video in videos)
                {
                    sb.Append("<Video ");
                    sb.Append("VideoID=\"" + video.VideoID.ToString() + "\" ");
                    sb.Append("StoredFilename=\"" + video.StoredFilename + "\" ");
                    sb.Append("VideoName=\"" + video.VideoName + "\" ");
                    sb.Append(" />");
                }
                sb.Append("</Videos>");


                // Musics ---------------------------------------------------------------------------------
                musics = musics.Distinct().ToList();
                sb.Append("<Musics>");
                foreach (Music music in musics)
                {
                    sb.Append("<Music ");
                    sb.Append("MusicID=\"" + music.MusicID.ToString() + "\" ");
                    sb.Append("StoredFilename=\"" + music.StoredFilename + "\" ");
                    sb.Append("MusicName=\"" + music.MusicName + "\" ");
                    sb.Append(" />");
                }
                sb.Append("</Musics>");

                // Player Settings ---------------------------------------------------------------------------------
                // Start with System Defaults - loop through the system defaults - then override account settings - then override player settings
                IEnumerable<PlayerSettingSystemDefault> systemdefaults = playersettingsystemrep.GetAllPlayerSettingSystemDefaults();
                IEnumerable<PlayerSettingAccountDefault> accountdefaults = playersettingaccountrep.GetAllPlayerSettingAccountDefaults(player.AccountID);
                IEnumerable<PlayerSetting> playerdefaults = playersettingrep.GetAllPlayerSettings(playerid);

                if (systemdefaults != null && systemdefaults.Count() > 0)
                {
                    foreach (PlayerSettingSystemDefault systemdefault in systemdefaults)
                    {
                        if (accountdefaults != null && accountdefaults.Count() > 0)
                        {
                            foreach (PlayerSettingAccountDefault accountdefault in accountdefaults)
                            {
                                if (systemdefault.PlayerSettingName == accountdefault.PlayerSettingName)
                                    systemdefault.PlayerSettingSystemDefaultValue = accountdefault.PlayerSettingAccountDefaultValue;
                            }
                        }
                        if (playerdefaults != null && playerdefaults.Count() > 0)
                        {
                            foreach (PlayerSetting playerdefault in playerdefaults)
                            {
                                if (systemdefault.PlayerSettingName == playerdefault.PlayerSettingName)
                                    systemdefault.PlayerSettingSystemDefaultValue = playerdefault.PlayerSettingValue;
                            }
                        }
                    }
                }

                sb.Append("<PlayerSettings>");
                foreach (PlayerSettingSystemDefault playersetting in systemdefaults)
                {
                    sb.Append("<PlayerSetting ");
                    sb.Append("PlayerSettingName=\"" + playersetting.PlayerSettingName + "\" ");
                    sb.Append("PlayerSettingTypeID=\"" + playersetting.PlayerSettingTypeID.ToString() + "\" ");
                    sb.Append("PlayerSettingValue=\"" + playersetting.PlayerSettingSystemDefaultValue + "\" ");
                    sb.Append(" />");
                }
                sb.Append("</PlayerSettings>");

                // Close the XML and return
                sb.Append("</xml>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "<xml><Error>" + ex.Message + "</Error></xml>";
            }
        }

        [WebMethod]
        public string Player_GetMediaToDownload(int accountid)
        {
            try
            {
                IImageRepository imagerep = new EntityImageRepository();
                IVideoRepository videorep = new EntityVideoRepository();
                IMusicRepository musicrep = new EntityMusicRepository();

                // Returns all the media files for the account
                List<Image> images = new List<Image>();
                List<Video> videos = new List<Video>();
                List<Music> musics = new List<Music>();

                StringBuilder sb = new StringBuilder();

                sb.Append("<xml>");

                // Images ---------------------------------------------------------------------------------
                images = imagerep.GetAllImages(accountid).ToList();
                sb.Append("<Images>");
                foreach (Image image in images)
                {
                    sb.Append("<Image ");
                    sb.Append("ImageID=\"" + image.ImageID.ToString() + "\" ");
                    sb.Append("StoredFilename=\"" + image.StoredFilename + "\" ");
                    sb.Append("ImageName=\"" + Utility.EncodeXMLString(image.ImageName) + "\" ");
                    sb.Append(" />");
                }
                sb.Append("</Images>");

                // Videos ---------------------------------------------------------------------------------
                videos = videorep.GetAllVideos(accountid).ToList();
                sb.Append("<Videos>");
                foreach (Video video in videos)
                {
                    sb.Append("<Video ");
                    sb.Append("VideoID=\"" + video.VideoID.ToString() + "\" ");
                    sb.Append("StoredFilename=\"" + video.StoredFilename + "\" ");
                    sb.Append("VideoName=\"" + video.VideoName + "\" ");
                    sb.Append(" />");
                }
                sb.Append("</Videos>");

                // Music ---------------------------------------------------------------------------------
                musics = musicrep.GetAllMusics(accountid).ToList();
                sb.Append("<Musics>");
                foreach (Music music in musics)
                {
                    sb.Append("<Music ");
                    sb.Append("MusicID=\"" + music.MusicID.ToString() + "\" ");
                    sb.Append("StoredFilename=\"" + music.StoredFilename + "\" ");
                    sb.Append("MusicName=\"" + music.MusicName + "\" ");
                    sb.Append(" />");
                }
                sb.Append("</Musics>");

                // Close the XML and return
                sb.Append("</xml>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "<xml><Error>" + ex.Message + "</Error></xml>";
            }
        }

        [WebMethod]
        public int PlayerScreenContentLog_Create(int accountid, int playerid, string playername, int screenid, string screenname,
                                                int screencontentid, string screencontentname, int screencontenttypeid, string screencontenttypename,
                                                DateTime displaydatetime, DateTime closedatetime, string contentdetails)
        {
            try
            {
                IPlayerScreenContentLogRepository playerscreencontentlogrep = new EntityPlayerScreenContentLogRepository();

                PlayerScreenContentLog playerscreencontentlog = new PlayerScreenContentLog();
                playerscreencontentlog.AccountID = accountid;
                playerscreencontentlog.PlayerID = playerid;
                playerscreencontentlog.PlayerName = playername;
                playerscreencontentlog.ScreenID = screenid;
                playerscreencontentlog.ScreenName = screenname;
                playerscreencontentlog.ScreenContentID = screencontentid;
                playerscreencontentlog.ScreenContentName = screencontentname;
                playerscreencontentlog.ScreenContentTypeID = screencontenttypeid;
                playerscreencontentlog.ScreenContentTypeName = screencontenttypename;
                playerscreencontentlog.DisplayDateTime = displaydatetime;
                playerscreencontentlog.CloseDateTime = closedatetime;
                playerscreencontentlog.ContentDetails = contentdetails;

                playerscreencontentlogrep.CreatePlayerScreenContentLog(playerscreencontentlog);

                return playerscreencontentlog.PlayerScreenContentLogID;
            }
            catch { return 0; }
        }

        [WebMethod]
        public void PlayerScreenContentLog_UpdateCloseDateTime(int playerscreencontentlogid, DateTime closedatetime)
        {
            try
            {
                IPlayerScreenContentLogRepository playerscreencontentlogrep = new EntityPlayerScreenContentLogRepository();

                PlayerScreenContentLog playerscreencontentlog = playerscreencontentlogrep.GetPlayerScreenContentLog(playerscreencontentlogid);
                playerscreencontentlog.CloseDateTime = closedatetime;

                playerscreencontentlogrep.UpdatePlayerScreenContentLog(playerscreencontentlog);
            }
            catch { }
        }

        [WebMethod]
        public int PlayerScreenLog_Create(int accountid, int playerid, string playername, int screenid, string screenname,
                                            DateTime displaydatetime, DateTime closedatetime, string screendetails)
        {
            try
            {
                IPlayerScreenLogRepository playerscreenlogrep = new EntityPlayerScreenLogRepository();

                PlayerScreenLog playerscreenlog = new PlayerScreenLog();
                playerscreenlog.AccountID = accountid;
                playerscreenlog.PlayerID = playerid;
                playerscreenlog.PlayerName = playername;
                playerscreenlog.ScreenID = screenid;
                playerscreenlog.ScreenName = screenname;
                playerscreenlog.DisplayDateTime = displaydatetime;
                playerscreenlog.CloseDateTime = closedatetime;
                playerscreenlog.ScreenDetails = screendetails;

                playerscreenlogrep.CreatePlayerScreenLog(playerscreenlog);

                return playerscreenlog.PlayerScreenLogID;
            }
            catch { return 0; }
        }

        [WebMethod]
        public void PlayerScreenLog_UpdateCloseDateTime(int playerscreenlogid, DateTime closedatetime)
        {
            try
            {
                IPlayerScreenLogRepository playerscreenlogrep = new EntityPlayerScreenLogRepository();

                PlayerScreenLog playerscreenlog = playerscreenlogrep.GetPlayerScreenLog(playerscreenlogid);
                playerscreenlog.CloseDateTime = closedatetime;

                playerscreenlogrep.UpdatePlayerScreenLog(playerscreenlog);
            }
            catch { }
        }

        [WebMethod]
        public UserAccount User_Validate(string username, string password)
        {
            try
            {
                IUserRepository userrep = new EntityUserRepository();
                User user = userrep.ValidateUser(username, password);
                if (user == null)
                    return null;

                IAccountRepository acctrep = new EntityAccountRepository();
                Account acct = acctrep.GetAccount(user.AccountID);
                if (acct == null || !acct.IsActive)
                    return null;

                UserAccount useracct = new UserAccount();
                useracct.UserID = user.UserID;
                useracct.Username = user.Username;
                useracct.FirstName = user.FirstName;
                useracct.LastName = user.LastName;
                useracct.EmailAddress = user.EmailAddress;
                useracct.IsAdmin = user.IsAdmin;
                useracct.UserIsActive = user.IsActive;
                useracct.AccountID = acct.AccountID;
                useracct.AccountName = acct.AccountName;
                useracct.AccountDescription = acct.AccountDescription;
                useracct.FTPServer = acct.FTPServer;
                useracct.FTPUsername = acct.FTPUsername;
                useracct.FTPPassword = acct.FTPPassword;
                useracct.AccountIsActive = acct.IsActive;

                return useracct;
            }
            catch
            {
                return null;
            }
        }

    }
}
