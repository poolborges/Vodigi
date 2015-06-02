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
    public class EntityPlayListRepository : IPlayListRepository
    {
        private VodigiContext db = new VodigiContext();

        public PlayList GetPlayList(int id)
        {
            PlayList playlist = db.PlayLists.Find(id);

            return playlist;
        }

        public IEnumerable<PlayList> GetAllPlayLists(int accountid)
        {
            var query = from playlist in db.PlayLists
                        select playlist;
            query = query.Where(pls => pls.AccountID.Equals(accountid));
            query = query.OrderBy("PlayListName", false);

            List<PlayList> playlists = query.ToList();

            return playlists;
        }

        public IEnumerable<PlayList> GetActivePlayLists(int accountid)
        {
            var query = from playlist in db.PlayLists
                        select playlist;
            query = query.Where(pls => pls.AccountID.Equals(accountid));
            query = query.Where(pls => pls.IsActive == true);
            query = query.OrderBy("PlayListName", false);

            List<PlayList> playlists = query.ToList();

            return playlists;
        }

        public IEnumerable<PlayList> GetPlayListPage(int accountid, string playlistname, string tag, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from playlist in db.PlayLists
                        select playlist;
            query = query.Where(pls => pls.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(playlistname))
                query = query.Where(pls => pls.PlayListName.StartsWith(playlistname));
            if (!String.IsNullOrEmpty(tag))
                query = query.Where(pls => pls.Tags.Contains(tag));
            if (!includeinactive)
                query = query.Where(pls => pls.IsActive == true);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<PlayList> playlists = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return playlists;
        }

        public int GetPlayListRecordCount(int accountid, string playlistname, string tag, bool includeinactive)
        {
            var query = from playlist in db.PlayLists
                        select playlist;
            query = query.Where(pls => pls.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(playlistname))
                query = query.Where(pls => pls.PlayListName.StartsWith(playlistname));
            if (!String.IsNullOrEmpty(tag))
                query = query.Where(pls => pls.Tags.Contains(tag));
            if (!includeinactive)
                query = query.Where(pls => pls.IsActive == true);

            // Get a Count of all filtered records
            return query.Count();
        }

        public void CreatePlayList(PlayList playlist)
        {
            db.PlayLists.Add(playlist);
            db.SaveChanges();
        }

        public void UpdatePlayList(PlayList playlist)
        {
            db.Entry(playlist).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}