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
    class CurrentScreen
    {
        public static Screen ScreenInfo;
        public static List<ScreenScreenContentXref> ScreenScreenContentXrefs;
        public static List<ScreenContent> ScreenContents;
        public static List<SlideShow> SlideShows;
        public static List<SlideShowImageXref> SlideShowImageXrefs;
        public static List<SlideShowMusicXref> SlideShowMusicXrefs;
        public static List<Timeline> Timelines;
        public static List<TimelineImageXref> TimelineImageXrefs;
        public static List<TimelineMusicXref> TimelineMusicXrefs;
        public static List<TimelineVideoXref> TimelineVideoXrefs;
        public static List<Image> Images;
        public static List<Music> Musics;
        public static List<PlayList> PlayLists;
        public static List<PlayListVideoXref> PlayListVideoXrefs;
        public static List<Video> Videos;
        public static List<Survey> Surveys;
        public static List<SurveyQuestion> SurveyQuestions;
        public static List<SurveyQuestionOption> SurveyQuestionOptions;
    }
}
