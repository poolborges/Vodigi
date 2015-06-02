
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
    public class EntitySlideShowMusicXrefRepository : ISlideShowMusicXrefRepository
    {
        private VodigiContext db = new VodigiContext();

        public IEnumerable<SlideShowMusicXref> GetSlideShowMusicXrefs(int slideshowid)
        {
            var query = from slideshowmusicxref in db.SlideShowMusicXrefs
                        select slideshowmusicxref;
            query = query.Where(xrefs => xrefs.SlideShowID.Equals(slideshowid));
            query = query.OrderBy("PlayOrder", false);

            List<SlideShowMusicXref> ssmxs = query.ToList();

            return ssmxs;
        }

        public void DeleteSlideShowMusicXrefs(int slideshowid)
        {
            var query = from slideshowmusicxref in db.SlideShowMusicXrefs
                        select slideshowmusicxref;
            query = query.Where(xrefs => xrefs.SlideShowID.Equals(slideshowid));

            List<SlideShowMusicXref> ssmxs = query.ToList();

            foreach (SlideShowMusicXref ssmx in ssmxs)
            {
                db.SlideShowMusicXrefs.Remove(ssmx);
            }

            db.SaveChanges();
        }

        public void CreateSlideShowMusicXref(SlideShowMusicXref xref)
        {
            db.SlideShowMusicXrefs.Add(xref);
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}