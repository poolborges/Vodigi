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
using System.Data;

namespace osVodigiWeb6x.Models
{
    public class EntityScreenContentRepository : IScreenContentRepository
    {
        private VodigiContext db = new VodigiContext();

        public ScreenContent GetScreenContent(int id)
        {
            ScreenContent screencontent = db.ScreenContents.Find(id);

            return screencontent;
        }

        public IEnumerable<ScreenContent> GetAllScreenContents(int accountid)
        {
            var query = from screencontent in db.ScreenContents
                        select screencontent;
            query = query.Where(scs => scs.AccountID.Equals(accountid));
            query = query.OrderBy("ScreenContentName", false);

            List<ScreenContent> screencontents = query.ToList();

            return screencontents;
        }

        public IEnumerable<ScreenContent> GetScreenContentPage(int accountid, string screencontentname, int screencontenttypeid, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from screencontent in db.ScreenContents
                        select screencontent;
            query = query.Where(scs => scs.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(screencontentname))
                query = query.Where(scs => scs.ScreenContentName.StartsWith(screencontentname));
            if (screencontenttypeid != 0)
                query = query.Where(scs => scs.ScreenContentTypeID.Equals(screencontenttypeid));
            if (!includeinactive)
                query = query.Where(scs => scs.IsActive == true);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<ScreenContent> screencontents = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return screencontents;
        }

        public int GetScreenContentRecordCount(int accountid, string screencontentname, int screencontenttypeid, bool includeinactive)
        {
            var query = from screencontent in db.ScreenContents
                        select screencontent;
            query = query.Where(scs => scs.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(screencontentname))
                query = query.Where(scs => scs.ScreenContentName.StartsWith(screencontentname));
            if (screencontenttypeid != 0)
                query = query.Where(scs => scs.ScreenContentTypeID.Equals(screencontenttypeid));
            if (!includeinactive)
                query = query.Where(scs => scs.IsActive == true);

            // Get a Count of all filtered records
            return query.Count();
        }

        public void CreateScreenContent(ScreenContent screencontent)
        {
            db.ScreenContents.Add(screencontent);
            db.SaveChanges();
        }

        public void UpdateScreenContent(ScreenContent screencontent)
        {
            db.Entry(screencontent).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}