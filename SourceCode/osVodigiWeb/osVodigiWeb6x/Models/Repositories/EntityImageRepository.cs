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
    public class EntityImageRepository : IImageRepository
    {
        private VodigiContext db = new VodigiContext();

        public Image GetImage(int id)
        {
            Image image = db.Images.Find(id);

            return image;
        }

        public Image GetImageByGuid(int accountid, string guid)
        {
            var query = from image in db.Images
                        select image;
            query = query.Where(imgs => imgs.AccountID.Equals(accountid));
            query = query.Where(imgs => imgs.StoredFilename.ToLower().Contains(guid.ToLower()));

            List<Image> images = query.ToList();

            if (images.Count > 0)
                return images[0];
            else  
                return null;
        }

        public IEnumerable<Image> GetActiveImages(int accountid)
        {
            var query = from image in db.Images
                        select image;
            query = query.Where(imgs => imgs.AccountID.Equals(accountid));
            query = query.Where(imgs => imgs.IsActive == true);
            query = query.OrderBy("ImageName", false);
            
            List<Image> images = query.ToList();

            return images;
        }

        public IEnumerable<Image> GetAllImages(int accountid)
        {
            var query = from image in db.Images
                        select image;
            query = query.Where(imgs => imgs.AccountID.Equals(accountid));
            query = query.OrderBy("ImageName", false);

            List<Image> images = query.ToList();

            return images;
        }


        public IEnumerable<Image> GetImagePage(int accountid, string imagename, string tag, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from image in db.Images
                        select image;
            query = query.Where(imgs => imgs.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(imagename))
                query = query.Where(imgs => imgs.ImageName.StartsWith(imagename));
            if (!String.IsNullOrEmpty(tag))
                query = query.Where(imgs => imgs.Tags.Contains(tag));
            if (!includeinactive)
                query = query.Where(imgs => imgs.IsActive == true);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<Image> images = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return images;
        }

        public int GetImageRecordCount(int accountid, string imagename, string tag, bool includeinactive)
        {
            var query = from image in db.Images
                        select image;
            query = query.Where(imgs => imgs.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(imagename))
                query = query.Where(imgs => imgs.ImageName.StartsWith(imagename));
            if (!String.IsNullOrEmpty(tag))
                query = query.Where(imgs => imgs.Tags.Contains(tag));
            if (!includeinactive)
                query = query.Where(imgs => imgs.IsActive == true);

            // Get a Count of all filtered records
            return query.Count();
        }

        public void CreateImage(Image image)
        {
            db.Images.Add(image);
            db.SaveChanges();
        }

        public void UpdateImage(Image image)
        {
            db.Entry(image).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

    }
}