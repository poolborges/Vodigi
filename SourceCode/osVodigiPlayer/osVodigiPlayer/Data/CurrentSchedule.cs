using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;

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
    class CurrentSchedule
    {
        public static List<PlayerGroupSchedule> PlayerGroupSchedules; // Should only be one
        public static List<Screen> Screens;
        public static List<PlayList> PlayLists;
        public static List<PlayListVideoXref> PlayListVideoXrefs;
        public static List<SlideShow> SlideShows;
        public static List<SlideShowImageXref> SlideShowImageXrefs;
        public static List<SlideShowMusicXref> SlideShowMusicXrefs;
        public static List<ScreenScreenContentXref> ScreenScreenContentXrefs;
        public static List<ScreenContent> ScreenContents;
        public static List<Image> Images;
        public static List<Video> Videos;
        public static List<Timeline> Timelines;
        public static List<TimelineImageXref> TimelineImageXrefs;
        public static List<TimelineVideoXref> TimelineVideoXrefs;
        public static List<TimelineMusicXref> TimelineMusicXrefs;
        public static List<Music> Musics;
        public static List<Survey> Surveys;
        public static List<SurveyQuestion> SurveyQuestions;
        public static List<SurveyQuestionOption> SurveyQuestionOptions;
        public static List<PlayerSetting> PlayerSettings;
        public static string LastScheduleXML;

        public static void ParseScheduleXml(string xml)
        {
            try
            {
                // Get the PlayerGroupSchedule(s)
                try
                {
                    List<PlayerGroupSchedule> pgs = new List<PlayerGroupSchedule>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    pgs = (from PlayerGroupSchedule in xmldoc.Descendants("PlayerGroupSchedule")
                           select new PlayerGroupSchedule
                           {
                               PlayerGroupScheduleID = Convert.ToInt32(PlayerGroupSchedule.Attribute("PlayerGroupScheduleID").Value),
                               PlayerGroupID = Convert.ToInt32(PlayerGroupSchedule.Attribute("PlayerGroupID").Value),
                               ScreenID = Convert.ToInt32(PlayerGroupSchedule.Attribute("ScreenID").Value),
                               Day = Convert.ToInt32(PlayerGroupSchedule.Attribute("Day").Value),
                               Hour = Convert.ToInt32(PlayerGroupSchedule.Attribute("Hour").Value),
                               Minute = Convert.ToInt32(PlayerGroupSchedule.Attribute("Minute").Value),
                           }
                    ).ToList();

                    PlayerGroupSchedules = pgs;
                }
                catch { }

                // Parse out the Screens
                try
                {
                    List<Screen> ss = new List<Screen>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    ss = (from Screen in xmldoc.Descendants("Screen")
                          select new Screen
                          {
                              ScreenID = Convert.ToInt32(Screen.Attribute("ScreenID").Value),
                              AccountID = Convert.ToInt32(Screen.Attribute("AccountID").Value),
                              ScreenName = Utility.DecodeXMLString(Convert.ToString(Screen.Attribute("ScreenName").Value)),
                              PlayListID = Convert.ToInt32(Screen.Attribute("PlayListID").Value),
                              SlideShowID = Convert.ToInt32(Screen.Attribute("SlideShowID").Value),
                              TimelineID = Convert.ToInt32(Screen.Attribute("TimelineID").Value),
                              ButtonImageID = Convert.ToInt32(Screen.Attribute("ButtonImageID").Value),
                              IsInteractive = Convert.ToBoolean(Screen.Attribute("IsInteractive").Value),
                          }
                    ).ToList();

                    Screens = ss;
                }
                catch { }

                // Parse out the ScreenScreenContentXrefs
                try
                {
                    List<ScreenScreenContentXref> sscxrefs = new List<ScreenScreenContentXref>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    sscxrefs = (from ScreenScreenContentXref in xmldoc.Descendants("ScreenScreenContentXref")
                                select new ScreenScreenContentXref
                                {
                                    ScreenScreenContentXrefID = Convert.ToInt32(ScreenScreenContentXref.Attribute("ScreenScreenContentXrefID").Value),
                                    ScreenID = Convert.ToInt32(ScreenScreenContentXref.Attribute("ScreenID").Value),
                                    ScreenContentID = Convert.ToInt32(ScreenScreenContentXref.Attribute("ScreenContentID").Value),
                                    DisplayOrder = Convert.ToInt32(ScreenScreenContentXref.Attribute("ScreenID").Value),
                                }
                    ).ToList();

                    ScreenScreenContentXrefs = sscxrefs;
                }
                catch { }

                // Parse out the ScreenContents
                try
                {
                    List<ScreenContent> scs = new List<ScreenContent>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    scs = (from ScreenContent in xmldoc.Descendants("ScreenContent")
                           select new ScreenContent
                           {
                               ScreenContentID = Convert.ToInt32(ScreenContent.Attribute("ScreenContentID").Value),
                               ScreenContentTypeID = Convert.ToInt32(ScreenContent.Attribute("ScreenContentTypeID").Value),
                               ScreenContentTypeName = Utility.DecodeXMLString(Convert.ToString(ScreenContent.Attribute("ScreenContentTypeName").Value)),
                               ScreenContentName = Utility.DecodeXMLString(Convert.ToString(ScreenContent.Attribute("ScreenContentName").Value)),
                               ScreenContentTitle = Convert.ToString(ScreenContent.Attribute("ScreenContentTitle").Value),
                               ThumbnailImageID = Convert.ToInt32(ScreenContent.Attribute("ThumbnailImageID").Value),
                               CustomField1 = Utility.DecodeXMLString(Convert.ToString(ScreenContent.Attribute("CustomField1").Value)),
                               CustomField2 = Utility.DecodeXMLString(Convert.ToString(ScreenContent.Attribute("CustomField2").Value)),
                               CustomField3 = Utility.DecodeXMLString(Convert.ToString(ScreenContent.Attribute("CustomField3").Value)),
                               CustomField4 = Utility.DecodeXMLString(Convert.ToString(ScreenContent.Attribute("CustomField4").Value)),
                           }
                    ).ToList();

                    ScreenContents = scs;
                }
                catch { }

                // Parse out the SlideShows
                try
                {
                    List<SlideShow> sss = new List<SlideShow>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    sss = (from SlideShow in xmldoc.Descendants("SlideShow")
                           select new SlideShow
                           {
                               SlideShowID = Convert.ToInt32(SlideShow.Attribute("SlideShowID").Value),
                               IntervalInSecs = Convert.ToInt32(SlideShow.Attribute("IntervalInSecs").Value),
                               TransitionType = Utility.DecodeXMLString(Convert.ToString(SlideShow.Attribute("TransitionType").Value)),
                           }
                    ).ToList();

                    SlideShows = sss;
                }
                catch { }

                // Parse out the SlideShowImageXrefs
                try
                {
                    List<SlideShowImageXref> ssis = new List<SlideShowImageXref>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    ssis = (from SlideShowImageXref in xmldoc.Descendants("SlideShowImageXref")
                            select new SlideShowImageXref
                            {
                                SlideShowImageXrefID = Convert.ToInt32(SlideShowImageXref.Attribute("SlideShowImageXrefID").Value),
                                SlideShowID = Convert.ToInt32(SlideShowImageXref.Attribute("SlideShowID").Value),
                                ImageID = Convert.ToInt32(SlideShowImageXref.Attribute("ImageID").Value),
                                PlayOrder = Convert.ToInt32(SlideShowImageXref.Attribute("PlayOrder").Value),
                            }
                    ).ToList();

                    SlideShowImageXrefs = ssis;
                }
                catch { }

                // Parse out the SlideShowMusicXrefs
                try
                {
                    List<SlideShowMusicXref> ssms = new List<SlideShowMusicXref>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    ssms = (from SlideShowMusicXref in xmldoc.Descendants("SlideShowMusicXref")
                            select new SlideShowMusicXref
                            {
                                SlideShowMusicXrefID = Convert.ToInt32(SlideShowMusicXref.Attribute("SlideShowMusicXrefID").Value),
                                SlideShowID = Convert.ToInt32(SlideShowMusicXref.Attribute("SlideShowID").Value),
                                MusicID = Convert.ToInt32(SlideShowMusicXref.Attribute("MusicID").Value),
                                PlayOrder = Convert.ToInt32(SlideShowMusicXref.Attribute("PlayOrder").Value),
                            }
                    ).ToList();

                    SlideShowMusicXrefs = ssms;
                }
                catch { }

                // Parse out the Timelines
                try
                {
                    List<Timeline> tls = new List<Timeline>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    tls = (from Timeline in xmldoc.Descendants("Timeline")
                           select new Timeline
                           {
                               TimelineID = Convert.ToInt32(Timeline.Attribute("TimelineID").Value),
                               DurationInSecs = Convert.ToInt32(Timeline.Attribute("DurationInSecs").Value),
                               MuteMusicOnPlayback = Convert.ToBoolean(Timeline.Attribute("MuteMusicOnPlayback").Value),
                           }
                    ).ToList();

                    Timelines = tls;
                }
                catch { }

                // Parse out the TimelineImageXrefs
                try
                {
                    List<TimelineImageXref> tlis = new List<TimelineImageXref>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    tlis = (from TimelineImageXref in xmldoc.Descendants("TimelineImageXref")
                            select new TimelineImageXref
                            {
                                TimelineImageXrefID = Convert.ToInt32(TimelineImageXref.Attribute("TimelineImageXrefID").Value),
                                TimelineID = Convert.ToInt32(TimelineImageXref.Attribute("TimelineID").Value),
                                ImageID = Convert.ToInt32(TimelineImageXref.Attribute("ImageID").Value),
                                DisplayOrder = Convert.ToInt32(TimelineImageXref.Attribute("DisplayOrder").Value),
                            }
                    ).ToList();

                    TimelineImageXrefs = tlis;
                }
                catch { }

                // Parse out the TimelineMusicXrefs
                try
                {
                    List<TimelineMusicXref> tlms = new List<TimelineMusicXref>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    tlms = (from TimelineMusicXref in xmldoc.Descendants("TimelineMusicXref")
                            select new TimelineMusicXref
                            {
                                TimelineMusicXrefID = Convert.ToInt32(TimelineMusicXref.Attribute("TimelineMusicXrefID").Value),
                                TimelineID = Convert.ToInt32(TimelineMusicXref.Attribute("TimelineID").Value),
                                MusicID = Convert.ToInt32(TimelineMusicXref.Attribute("MusicID").Value),
                                PlayOrder = Convert.ToInt32(TimelineMusicXref.Attribute("PlayOrder").Value),
                            }
                    ).ToList();

                    TimelineMusicXrefs = tlms;
                }
                catch { }

                // Parse out the TimelineVideoXrefs
                try
                {
                    List<TimelineVideoXref> tlvs = new List<TimelineVideoXref>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    tlvs = (from TimelineVideoXref in xmldoc.Descendants("TimelineVideoXref")
                            select new TimelineVideoXref
                            {
                                TimelineVideoXrefID = Convert.ToInt32(TimelineVideoXref.Attribute("TimelineVideoXrefID").Value),
                                TimelineID = Convert.ToInt32(TimelineVideoXref.Attribute("TimelineID").Value),
                                VideoID = Convert.ToInt32(TimelineVideoXref.Attribute("VideoID").Value),
                                DisplayOrder = Convert.ToInt32(TimelineVideoXref.Attribute("DisplayOrder").Value),
                            }
                    ).ToList();

                    TimelineVideoXrefs = tlvs;
                }
                catch { }

                // Parse out the Images
                try
                {
                    List<Image> images = new List<Image>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    images = (from Image in xmldoc.Descendants("Image")
                              select new Image
                              {
                                  ImageID = Convert.ToInt32(Image.Attribute("ImageID").Value),
                                  StoredFilename = Convert.ToString(Image.Attribute("StoredFilename").Value),
                                  ImageName = Utility.DecodeXMLString(Image.Attribute("ImageName").Value),
                              }
                    ).ToList();

                    Images = images;
                }
                catch { }

                // Parse out the Musics
                try
                {
                    List<Music> musics = new List<Music>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    musics = (from Music in xmldoc.Descendants("Music")
                              select new Music
                              {
                                  MusicID = Convert.ToInt32(Music.Attribute("MusicID").Value),
                                  StoredFilename = Convert.ToString(Music.Attribute("StoredFilename").Value),
                                  MusicName = Utility.DecodeXMLString(Music.Attribute("MusicName").Value),
                              }
                    ).ToList();

                    Musics = musics;
                }
                catch { }

                // Parse out the PlayLists
                try
                {
                    List<PlayList> pls = new List<PlayList>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    pls = (from PlayList in xmldoc.Descendants("PlayList")
                           select new PlayList
                           {
                               PlayListID = Convert.ToInt32(PlayList.Attribute("PlayListID").Value),
                           }
                    ).ToList();

                    PlayLists = pls;
                }
                catch { }

                // Parse out the PlayListVideoXrefs
                try
                {
                    List<PlayListVideoXref> plvs = new List<PlayListVideoXref>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    plvs = (from PlayListVideoXref in xmldoc.Descendants("PlayListVideoXref")
                            select new PlayListVideoXref
                            {
                                PlayListVideoXrefID = Convert.ToInt32(PlayListVideoXref.Attribute("PlayListVideoXrefID").Value),
                                PlayListID = Convert.ToInt32(PlayListVideoXref.Attribute("PlayListID").Value),
                                VideoID = Convert.ToInt32(PlayListVideoXref.Attribute("VideoID").Value),
                                PlayOrder = Convert.ToInt32(PlayListVideoXref.Attribute("PlayOrder").Value),
                            }
                    ).ToList();

                    PlayListVideoXrefs = plvs;
                }
                catch { }

                // Parse out the Videos
                try
                {
                    List<Video> videos = new List<Video>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    videos = (from Video in xmldoc.Descendants("Video")
                              select new Video
                              {
                                  VideoID = Convert.ToInt32(Video.Attribute("VideoID").Value),
                                  StoredFilename = Convert.ToString(Video.Attribute("StoredFilename").Value),
                                  VideoName = Utility.DecodeXMLString(Convert.ToString(Video.Attribute("VideoName").Value)),
                              }
                    ).ToList();

                    Videos = videos;
                }
                catch { }

                // Parse out the Surveys
                try
                {
                    List<Survey> surveys = new List<Survey>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    surveys = (from Survey in xmldoc.Descendants("Survey")
                               select new Survey
                               {
                                   SurveyID = Convert.ToInt32(Survey.Attribute("SurveyID").Value),
                                   SurveyName = Utility.DecodeXMLString(Convert.ToString(Survey.Attribute("SurveyName").Value)),
                                   SurveyImageID = Convert.ToInt32(Survey.Attribute("SurveyImageID").Value),
                               }
                    ).ToList();

                    Surveys = surveys;
                }
                catch { }

                // Parse out the SurveyQuestions
                try
                {
                    List<SurveyQuestion> questions = new List<SurveyQuestion>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    questions = (from SurveyQuestion in xmldoc.Descendants("SurveyQuestion")
                                 select new SurveyQuestion
                                 {
                                     SurveyQuestionID = Convert.ToInt32(SurveyQuestion.Attribute("SurveyQuestionID").Value),
                                     SurveyID = Convert.ToInt32(SurveyQuestion.Attribute("SurveyID").Value),
                                     SurveyQuestionText = Utility.DecodeXMLString(Convert.ToString(SurveyQuestion.Attribute("SurveyQuestionText").Value)),
                                     AllowMultiselect = Convert.ToBoolean(SurveyQuestion.Attribute("AllowMultiselect").Value),
                                     SortOrder = Convert.ToInt32(SurveyQuestion.Attribute("SortOrder").Value),
                                 }
                    ).ToList();

                    SurveyQuestions = questions;
                }
                catch { }

                // Parse out the SurveyQuestionOptions
                try
                {
                    List<SurveyQuestionOption> options = new List<SurveyQuestionOption>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    options = (from SurveyQuestionOption in xmldoc.Descendants("SurveyQuestionOption")
                               select new SurveyQuestionOption
                               {

                                   SurveyQuestionOptionID = Convert.ToInt32(SurveyQuestionOption.Attribute("SurveyQuestionOptionID").Value),
                                   SurveyQuestionID = Convert.ToInt32(SurveyQuestionOption.Attribute("SurveyQuestionID").Value),
                                   SurveyQuestionOptionText = Utility.DecodeXMLString(Convert.ToString(SurveyQuestionOption.Attribute("SurveyQuestionOptionText").Value)),
                                   SortOrder = Convert.ToInt32(SurveyQuestionOption.Attribute("SortOrder").Value),
                               }
                    ).ToList();

                    SurveyQuestionOptions = options;
                }
                catch { }

                // Parse out the PlayerSettings
                try
                {
                    List<PlayerSetting> settings = new List<PlayerSetting>();
                    XDocument xmldoc = XDocument.Parse(xml);
                    settings = (from PlayerSetting in xmldoc.Descendants("PlayerSetting")
                               select new PlayerSetting
                               {
                                   PlayerSettingName = Utility.DecodeXMLString(Convert.ToString(PlayerSetting.Attribute("PlayerSettingName").Value)),
                                   PlayerSettingTypeID = Convert.ToInt32(PlayerSetting.Attribute("PlayerSettingTypeID").Value),
                                   PlayerSettingValue = Utility.DecodeXMLString(Convert.ToString(PlayerSetting.Attribute("PlayerSettingValue").Value)),
                               }
                    ).ToList();

                    PlayerSettings = settings;
                    osVodigiPlayer.Helpers.PlayerSettings.AllPlayerSettings = settings;
                }
                catch { }

            }

            catch { }
        }

        public static void ClearSchedule()
        {
            try
            {
                // Clear the global data used to store schedule data
                PlayerGroupSchedules = new List<PlayerGroupSchedule>();
                Screens = new List<Screen>();
                PlayListVideoXrefs = new List<PlayListVideoXref>();
                SlideShowImageXrefs = new List<SlideShowImageXref>();
                ScreenScreenContentXrefs = new List<ScreenScreenContentXref>();
                ScreenContents = new List<ScreenContent>();
                Images = new List<Image>();
                Videos = new List<Video>();
                Timelines = new List<Timeline>();
                TimelineImageXrefs = new List<TimelineImageXref>();
                TimelineVideoXrefs = new List<TimelineVideoXref>();
                TimelineMusicXrefs = new List<TimelineMusicXref>();
                Surveys = new List<Survey>();
                SurveyQuestions = new List<SurveyQuestion>();
                SurveyQuestionOptions = new List<SurveyQuestionOption>();
            }
            catch { }
        }
    }
}
