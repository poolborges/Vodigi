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
using System.Data;

namespace osVodigiWeb6x.Models
{
    public class EntityTimelineRepository : ITimelineRepository
    {
        private VodigiContext db = new VodigiContext();

        public Timeline GetTimeline(int id)
        {
            Timeline timeline = db.Timelines.Find(id);

            return timeline;
        }

        public IEnumerable<Timeline> GetAllTimelines(int accountid)
        {
            var query = from timeline in db.Timelines
                        select timeline;
            query = query.Where(tls => tls.AccountID.Equals(accountid));
            query = query.OrderBy("TimelineName", false);

            List<Timeline> timelines = query.ToList();

            return timelines;
        }

        public IEnumerable<Timeline> GetActiveTimelines(int accountid)
        {
            var query = from timeline in db.Timelines
                        select timeline;
            query = query.Where(tls => tls.AccountID.Equals(accountid));
            query = query.Where(tls => tls.IsActive == true);
            query = query.OrderBy("TimelineName", false);

            List<Timeline> timelines = query.ToList();

            return timelines;
        }

        public IEnumerable<Timeline> GetTimelinePage(int accountid, string timelinename, string tag, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from timeline in db.Timelines
                        select timeline;
            query = query.Where(tls => tls.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(timelinename))
                query = query.Where(tls => tls.TimelineName.StartsWith(timelinename));
            if (!String.IsNullOrEmpty(tag))
                query = query.Where(tls => tls.Tags.Contains(tag));
            if (!includeinactive)
                query = query.Where(tls => tls.IsActive == true);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<Timeline> timelines = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return timelines;
        }

        public int GetTimelineRecordCount(int accountid, string timelinename, string tag, bool includeinactive)
        {
            var query = from timeline in db.Timelines
                        select timeline;
            query = query.Where(tls => tls.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(timelinename))
                query = query.Where(tls => tls.TimelineName.StartsWith(timelinename));
            if (!String.IsNullOrEmpty(tag))
                query = query.Where(tls => tls.Tags.Contains(tag));
            if (!includeinactive)
                query = query.Where(tls => tls.IsActive == true);

            // Get a Count of all filtered records
            return query.Count();
        }

        public void CreateTimeline(Timeline timeline)
        {
            db.Timelines.Add(timeline);
            db.SaveChanges();
        }

        public void UpdateTimeline(Timeline timeline)
        {
            db.Entry(timeline).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}