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
    public class EntityScreenRepository : IScreenRepository
    {
        private VodigiContext db = new VodigiContext();

        public Screen GetScreen(int id)
        {
            Screen screen = db.Screens.Find(id);

            return screen;
        }

        public IEnumerable<Screen> GetScreenPage(int accountid, string screenname, string description, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from screen in db.Screens
                        select screen;
            query = query.Where(ss => ss.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(screenname))
                query = query.Where(ss => ss.ScreenName.StartsWith(screenname));
            if (!String.IsNullOrEmpty(description))
                query = query.Where(ss => ss.ScreenDescription.Contains(description));
            if (!includeinactive)
                query = query.Where(ss => ss.IsActive == true);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<Screen> screens = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return screens;
        }

        public IEnumerable<Screen> GetActiveScreens(int accountid)
        {
            var query = from screen in db.Screens
                        select screen;
            query = query.Where(ss => ss.AccountID.Equals(accountid));
            query = query.Where(ss => ss.IsActive == true);
            query = query.OrderBy("AccountName", false);

            List<Screen> screens = query.ToList();

            return screens;
        }

        public IEnumerable<Screen> GetAllScreens(int accountid)
        {
            var query = from screen in db.Screens
                        select screen;
            query = query.Where(ss => ss.AccountID.Equals(accountid));
            query = query.OrderBy("ScreenName", false);

            List<Screen> screens = query.ToList();

            return screens;
        }

        public int GetScreenRecordCount(int accountid, string screenname, string description, bool includeinactive)
        {
            var query = from screen in db.Screens
                        select screen;
            query = query.Where(ss => ss.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(screenname))
                query = query.Where(ss => ss.ScreenName.StartsWith(screenname));
            if (!String.IsNullOrEmpty(description))
                query = query.Where(ss => ss.ScreenDescription.Contains(description));
            if (!includeinactive)
                query = query.Where(ss => ss.IsActive == true);

            // Get a Count of all filtered records
            return query.Count();
        }

        public void CreateScreen(Screen screen)
        {
            db.Screens.Add(screen);
            db.SaveChanges();
        }

        public void UpdateScreen(Screen screen)
        {
            db.Entry(screen).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}