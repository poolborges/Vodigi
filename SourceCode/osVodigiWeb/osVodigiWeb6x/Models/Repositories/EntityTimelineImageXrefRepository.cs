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

using System.Collections.Generic;
using System.Linq;

namespace osVodigiWeb6x.Models
{
    public class EntityTimelineImageXrefRepository : ITimelineImageXrefRepository
    {
        private VodigiContext db = new VodigiContext();

        public IEnumerable<TimelineImageXref> GetTimelineImageXrefs(int timelineid)
        {
            var query = from timelineimagexref in db.TimelineImageXrefs
                        select timelineimagexref;
            query = query.Where(xrefs => xrefs.TimelineID.Equals(timelineid));
            query = query.OrderBy("DisplayOrder", false);

            List<TimelineImageXref> tixs = query.ToList();

            return tixs;
        }

        public void DeleteTimelineImageXrefs(int timelineid)
        {
            var query = from timelineimagexref in db.TimelineImageXrefs
                        select timelineimagexref;
            query = query.Where(xrefs => xrefs.TimelineID.Equals(timelineid));

            List<TimelineImageXref> tixs = query.ToList();

            foreach (TimelineImageXref tix in tixs)
            {
                db.TimelineImageXrefs.Remove(tix);
            }

            db.SaveChanges();
        }

        public void CreateTimelineImageXref(TimelineImageXref xref)
        {
            db.TimelineImageXrefs.Add(xref);
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}