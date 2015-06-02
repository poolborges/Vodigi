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
    public class EntityTimelineScreenContentXrefRepository : ITimelineScreenContentXrefRepository
    {
        private VodigiContext db = new VodigiContext();

        public IEnumerable<TimelineScreenContentXref> GetTimelineScreenContentXrefs(int timelineid)
        {
            var query = from timelinescreencontentxref in db.TimelineScreenContentXrefs
                        select timelinescreencontentxref;
            query = query.Where(tscxs => tscxs.TimelineID.Equals(timelineid));
            query = query.OrderBy("DisplayOrder", false);

            List<TimelineScreenContentXref> timelinescreencontentxrefs = query.ToList();

            return timelinescreencontentxrefs;
        }

        public TimelineScreenContentXref GetTimelineScreenContentXref(int id)
        {
            TimelineScreenContentXref timelinescreencontentxref = db.TimelineScreenContentXrefs.Find(id);
            return timelinescreencontentxref;
        }

        public void DeleteTimelineScreenContentXref(TimelineScreenContentXref xref)
        {
            var query = from timelinescreencontentxref in db.TimelineScreenContentXrefs
                        select timelinescreencontentxref;

            query = query.Where(tscxs => tscxs.TimelineID.Equals(xref.TimelineID));
            query = query.OrderBy("DisplayOrder", false);

            List<TimelineScreenContentXref> timelinescreencontentxrefs = query.ToList();

            bool found = false;
            foreach (TimelineScreenContentXref timelinescreencontentxref in timelinescreencontentxrefs)
            {
                if (found)
                {
                    timelinescreencontentxref.DisplayOrder -= 1;
                    db.Entry(timelinescreencontentxref).State = EntityState.Modified;
                }
                if (timelinescreencontentxref.TimelineScreenContentXrefID == xref.TimelineScreenContentXrefID)
                {
                    found = true;
                    db.TimelineScreenContentXrefs.Remove(timelinescreencontentxref);
                }
            }

            db.SaveChanges();
        }

        public void CreateTimelineScreenContentXref(TimelineScreenContentXref xref)
        {
            // Get the maximum display order
            var query = from timelinescreencontentxref in db.TimelineScreenContentXrefs
                        select timelinescreencontentxref;

            query = query.Where(tscxs => tscxs.TimelineID.Equals(xref.TimelineID));
            query = query.OrderBy("DisplayOrder", true);

            List<TimelineScreenContentXref> timelinescreencontentxrefs = query.ToList();

            int maxdisplayorder = 0;
            if (timelinescreencontentxrefs.Count > 0)
                maxdisplayorder = timelinescreencontentxrefs[0].DisplayOrder;

            xref.DisplayOrder = maxdisplayorder + 1;
            db.TimelineScreenContentXrefs.Add(xref);
            db.SaveChanges();
        }

        public void MoveTimelineScreenContentXref(TimelineScreenContentXref xref, bool ismoveup)
        {
            var query = from timelinescreencontentxref in db.TimelineScreenContentXrefs
                        select timelinescreencontentxref;
            query = query.Where(tscxs => tscxs.TimelineID.Equals(xref.TimelineID));
            query = query.OrderBy("DisplayOrder", false);

            List<TimelineScreenContentXref> timelinescreencontentxrefs = query.ToList();

            // Get the current and max display orders
            int currentdisplayorder = xref.DisplayOrder;
            int maxdisplayorder = 1;
            foreach (TimelineScreenContentXref timelinescreencontentxref in timelinescreencontentxrefs)
            {
                if (timelinescreencontentxref.DisplayOrder > maxdisplayorder)
                    maxdisplayorder = timelinescreencontentxref.DisplayOrder;
            }

            // Adjust the appropriate display orders
            foreach (TimelineScreenContentXref timelinescreencontentxref in timelinescreencontentxrefs)
            {
                if (ismoveup)
                {
                    if (timelinescreencontentxref.TimelineScreenContentXrefID == xref.TimelineScreenContentXrefID) // move current question up
                    {
                        if (currentdisplayorder > 1)
                            xref.DisplayOrder -= 1;
                    }
                    else // find the previous item and increment it
                    {
                        if (timelinescreencontentxref.DisplayOrder == currentdisplayorder - 1)
                        {
                            timelinescreencontentxref.DisplayOrder += 1;
                            db.Entry(timelinescreencontentxref).State = EntityState.Modified;
                        }
                    }
                }
                else
                {
                    if (timelinescreencontentxref.TimelineScreenContentXrefID == xref.TimelineScreenContentXrefID) // move current question down
                    {
                        if (currentdisplayorder < maxdisplayorder)
                            xref.DisplayOrder += 1;
                    }
                    else // find the next item and decrement it
                    {
                        if (timelinescreencontentxref.DisplayOrder == currentdisplayorder + 1)
                        {
                            timelinescreencontentxref.DisplayOrder -= 1;
                            db.Entry(timelinescreencontentxref).State = EntityState.Modified;
                        }
                    }
                }
            }

            db.SaveChanges();
        }

        public void UpdateTimelineScreenContentXref(TimelineScreenContentXref xref)
        {
            db.Entry(xref).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}