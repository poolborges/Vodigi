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
    public class EntityVideoRepository : IVideoRepository
    {
        private VodigiContext db = new VodigiContext();

        public Video GetVideo(int id)
        {
            Video video = db.Videos.Find(id);

            return video;
        }

        public Video GetVideoByGuid(int accountid, string guid)
        {
            var query = from video in db.Videos
                        select video;
            query = query.Where(vids => vids.AccountID.Equals(accountid));
            query = query.Where(vids => vids.StoredFilename.ToLower().Contains(guid.ToLower()));

            List<Video> videos = query.ToList();

            if (videos.Count > 0)
                return videos[0];
            else
                return null;
        }

        public IEnumerable<Video> GetActiveVideos(int accountid)
        {
            var query = from video in db.Videos
                        select video;
            query = query.Where(vids => vids.AccountID.Equals(accountid));
            query = query.Where(vids => vids.IsActive == true);
            query = query.OrderBy("VideoName", false);

            List<Video> videos = query.ToList();

            return videos;
        }

        public IEnumerable<Video> GetAllVideos(int accountid)
        {
            var query = from video in db.Videos
                        select video;

            query = query.Where(vids => vids.AccountID.Equals(accountid));
            query = query.OrderBy("VideoName", false);

            List<Video> videos = query.ToList();

            return videos;
        }

        public IEnumerable<Video> GetVideoPage(int accountid, string videoname, string tag, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from video in db.Videos
                        select video;
            query = query.Where(vids => vids.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(videoname))
                query = query.Where(vids => vids.VideoName.StartsWith(videoname));
            if (!String.IsNullOrEmpty(tag))
                query = query.Where(vids => vids.Tags.Contains(tag));
            if (!includeinactive)
                query = query.Where(vids => vids.IsActive == true);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<Video> videos = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return videos;
        }

        public int GetVideoRecordCount(int accountid, string videoname, string tag, bool includeinactive)
        {
            var query = from video in db.Videos
                        select video;
            query = query.Where(vids => vids.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(videoname))
                query = query.Where(vids => vids.VideoName.StartsWith(videoname));
            if (!String.IsNullOrEmpty(tag))
                query = query.Where(vids => vids.Tags.Contains(tag));
            if (!includeinactive)
                query = query.Where(vids => vids.IsActive == true);

            // Get a Count of all filtered records
            return query.Count();
        }

        public void CreateVideo(Video video)
        {
            db.Videos.Add(video);
            db.SaveChanges();
        }

        public void UpdateVideo(Video video)
        {
            db.Entry(video).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}