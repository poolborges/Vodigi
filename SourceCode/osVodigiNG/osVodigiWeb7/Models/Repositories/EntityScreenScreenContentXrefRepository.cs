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
    public class EntityScreenScreenContentXrefRepository : IScreenScreenContentXrefRepository
    {
        private VodigiContext db = new VodigiContext();

        public IEnumerable<ScreenScreenContentXref> GetScreenScreenContentXrefs(int screenid)
        {
            var query = from screenscreencontentxref in db.ScreenScreenContentXrefs
                        select screenscreencontentxref;
            query = query.Where(xrefs => xrefs.ScreenID.Equals(screenid));
            query = query.OrderBy("DisplayOrder", false);
             
            List<ScreenScreenContentXref> sscxs = query.ToList();

            return sscxs;
        }

        public void DeleteScreenScreenContentXrefs(int screenid)
        {
            var query = from screenscreencontentxref in db.ScreenScreenContentXrefs
                        select screenscreencontentxref;
            query = query.Where(xrefs => xrefs.ScreenID.Equals(screenid));

            List<ScreenScreenContentXref> sscxs = query.ToList();

            foreach (ScreenScreenContentXref sscx in sscxs)
            {
                db.ScreenScreenContentXrefs.Remove(sscx);
            }

            db.SaveChanges();
        }

        public void CreateScreenScreenContentXref(ScreenScreenContentXref xref)
        {
            db.ScreenScreenContentXrefs.Add(xref);
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}