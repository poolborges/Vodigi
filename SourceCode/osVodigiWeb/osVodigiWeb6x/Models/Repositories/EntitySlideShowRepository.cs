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
    public class EntitySlideShowRepository : ISlideShowRepository
    {
        private VodigiContext db = new VodigiContext();

        public SlideShow GetSlideShow(int id)
        {
            SlideShow slideshow = db.SlideShows.Find(id);

            return slideshow;
        }

        public IEnumerable<SlideShow> GetAllSlideShows(int accountid)
        {
            var query = from slideshow in db.SlideShows
                        select slideshow;
            query = query.Where(sss => sss.AccountID.Equals(accountid));
            query = query.OrderBy("SlideShowName", false);

            List<SlideShow> slideshows = query.ToList();

            return slideshows;
        }

        public IEnumerable<SlideShow> GetActiveSlideShows(int accountid)
        {
            var query = from slideshow in db.SlideShows
                        select slideshow;
            query = query.Where(sss => sss.AccountID.Equals(accountid));
            query = query.Where(sss => sss.IsActive == true);
            query = query.OrderBy("SlideShowName", false);

            List<SlideShow> slideshows = query.ToList();

            return slideshows;
        }

        public IEnumerable<SlideShow> GetSlideShowPage(int accountid, string slideshowname, string tag, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from slideshow in db.SlideShows
                        select slideshow;
            query = query.Where(sss => sss.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(slideshowname))
                query = query.Where(sss => sss.SlideShowName.StartsWith(slideshowname));
            if (!String.IsNullOrEmpty(tag))
                query = query.Where(sss => sss.Tags.Contains(tag));
            if (!includeinactive)
                query = query.Where(sss => sss.IsActive == true);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<SlideShow> slideshows = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return slideshows;
        }

        public int GetSlideShowRecordCount(int accountid, string slideshowname, string tag, bool includeinactive)
        {
            var query = from slideshow in db.SlideShows
                        select slideshow;
            query = query.Where(sss => sss.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(slideshowname))
                query = query.Where(sss => sss.SlideShowName.StartsWith(slideshowname));
            if (!String.IsNullOrEmpty(tag))
                query = query.Where(sss => sss.Tags.Contains(tag));
            if (!includeinactive)
                query = query.Where(sss => sss.IsActive == true);

            // Get a Count of all filtered records
            return query.Count();
        }

        public void CreateSlideShow(SlideShow slideshow)
        {
            db.SlideShows.Add(slideshow);
            db.SaveChanges();
        }

        public void UpdateSlideShow(SlideShow slideshow)
        {
            db.Entry(slideshow).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}