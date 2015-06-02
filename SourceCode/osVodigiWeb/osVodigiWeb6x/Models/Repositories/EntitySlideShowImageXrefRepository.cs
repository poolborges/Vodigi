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
    public class EntitySlideShowImageXrefRepository : ISlideShowImageXrefRepository
    {
        private VodigiContext db = new VodigiContext();

        public IEnumerable<SlideShowImageXref> GetSlideShowImageXrefs(int slideshowid)
        {
            var query = from slideshowimagexref in db.SlideShowImageXrefs
                        select slideshowimagexref;
            query = query.Where(xrefs => xrefs.SlideShowID.Equals(slideshowid));
            query = query.OrderBy("PlayOrder", false);

            List<SlideShowImageXref> ssixs = query.ToList();

            return ssixs;
        }

        public void DeleteSlideShowImageXrefs(int slideshowid)
        {
            var query = from slideshowimagexref in db.SlideShowImageXrefs
                        select slideshowimagexref;
            query = query.Where(xrefs => xrefs.SlideShowID.Equals(slideshowid));

            List<SlideShowImageXref> ssixs = query.ToList();

            foreach (SlideShowImageXref ssix in ssixs)
            {
                db.SlideShowImageXrefs.Remove(ssix);
            }

            db.SaveChanges();
        }

        public void CreateSlideShowImageXref(SlideShowImageXref xref)
        {
            db.SlideShowImageXrefs.Add(xref);
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}