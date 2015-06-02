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
    public class EntityScreenContentTypeRepository : IScreenContentTypeRepository
    {
        private VodigiContext db = new VodigiContext();

        public ScreenContentType GetScreenContentType(int id)
        {
            ScreenContentType sct = db.ScreenContentTypes.Find(id);

            return sct;
        }

        public IEnumerable<ScreenContentType> GetAllScreenContentTypes()
        {
            var query = from screencontenttype in db.ScreenContentTypes
                        select screencontenttype;
            query = query.OrderBy("ScreenContentTypeName", false);
            query = query.Where(xrefs => xrefs.ScreenContentTypeName != "Webcam");
            query = query.Where(xrefs => xrefs.ScreenContentTypeName != "Weather");
            List<ScreenContentType> types = query.ToList();

            return types;
        }
    }
}