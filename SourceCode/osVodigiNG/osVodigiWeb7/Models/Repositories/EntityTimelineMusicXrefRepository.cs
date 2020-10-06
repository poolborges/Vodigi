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
    public class EntityTimelineMusicXrefRepository : ITimelineMusicXrefRepository
    {
        private VodigiContext db = new VodigiContext();

        public IEnumerable<TimelineMusicXref> GetTimelineMusicXrefs(int timelineid)
        {
            var query = from timelinemusicxref in db.TimelineMusicXrefs
                        select timelinemusicxref;
            query = query.Where(xrefs => xrefs.TimelineID.Equals(timelineid));
            query = query.OrderBy("PlayOrder", false);

            List<TimelineMusicXref> tmxs = query.ToList();

            return tmxs;
        }

        public void DeleteTimelineMusicXrefs(int timelineid)
        {
            var query = from timelinemusicxref in db.TimelineMusicXrefs
                        select timelinemusicxref;
            query = query.Where(xrefs => xrefs.TimelineID.Equals(timelineid));

            List<TimelineMusicXref> tmxs = query.ToList();

            foreach (TimelineMusicXref tmx in tmxs)
            {
                db.TimelineMusicXrefs.Remove(tmx);
            }

            db.SaveChanges();
        }

        public void CreateTimelineMusicXref(TimelineMusicXref xref)
        {
            db.TimelineMusicXrefs.Add(xref);
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}