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

using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace osVodigiWeb6x.Models
{
    public class VodigiContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AnsweredSurvey> AnsweredSurveys { get; set; }
        public DbSet<AnsweredSurveyQuestionOption> AnsweredSurveyQuestionOptions { get; set; }
        public DbSet<DatabaseVersion> DatabaseVersions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Music> Musics { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerGroup> PlayerGroups { get; set; }
        public DbSet<PlayerGroupSchedule> PlayerGroupSchedules { get; set; }
        public DbSet<PlayerSetting> PlayerSettings { get; set; }
        public DbSet<PlayerSettingAccountDefault> PlayerSettingAccountDefaults { get; set; }
        public DbSet<PlayerSettingSystemDefault> PlayerSettingSystemDefaults { get; set; }
        public DbSet<PlayerSettingType> PlayerSettingTypes { get; set; }
        public DbSet<PlayListVideoXref> PlayListVideoXrefs { get; set; }
        public DbSet<PlayList> PlayLists { get; set; }
        public DbSet<ScreenContent> ScreenContents { get; set; }
        public DbSet<ScreenContentType> ScreenContentTypes { get; set; }
        public DbSet<Screen> Screens { get; set; }
        public DbSet<ScreenScreenContentXref> ScreenScreenContentXrefs { get; set; }
        public DbSet<SlideShowImageXref> SlideShowImageXrefs { get; set; }
        public DbSet<SlideShowMusicXref> SlideShowMusicXrefs { get; set; }
        public DbSet<SlideShow> SlideShows { get; set; }
        public DbSet<Survey> Surveys { get; set; }
        public DbSet<SurveyQuestion> SurveyQuestions { get; set; }
        public DbSet<SurveyQuestionOption> SurveyQuestionOptions { get; set; }
        public DbSet<SystemMessage> SystemMessages { get; set; }
        public DbSet<Timeline> Timelines { get; set; }
        public DbSet<TimelineImageXref> TimelineImageXrefs { get; set; }
        public DbSet<TimelineMusicXref> TimelineMusicXrefs { get; set; }
        public DbSet<TimelineVideoXref> TimelineVideoXrefs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Video> Videos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}