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
    public class EntityPlayerGroupRepository : IPlayerGroupRepository
    {
        private VodigiContext db = new VodigiContext();

        public PlayerGroup GetPlayerGroup(int id)
        {
            PlayerGroup playergroup = db.PlayerGroups.Find(id);

            return playergroup;
        }

        public IEnumerable<PlayerGroup> GetPlayerGroupPage(int accountid, string playergroupname, string description, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from playergroup in db.PlayerGroups
                        select playergroup;
            query = query.Where(pgs => pgs.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(playergroupname))
                query = query.Where(pgs => pgs.PlayerGroupName.StartsWith(playergroupname));
            if (!String.IsNullOrEmpty(description))
                query = query.Where(pgs => pgs.PlayerGroupDescription.Contains(description));
            if (!includeinactive)
                query = query.Where(pgs => pgs.IsActive == true);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<PlayerGroup> playergroups = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return playergroups;
        }

        public IEnumerable<PlayerGroup> GetAllPlayerGroups(int accountid)
        {
            var query = from playergroup in db.PlayerGroups
                        select playergroup;
            query = query.Where(pgs => pgs.AccountID.Equals(accountid));
            query = query.OrderBy("PlayerGroupName", false);

            List<PlayerGroup> playergroups = query.ToList();

            return playergroups;
        }

        public int GetPlayerGroupRecordCount(int accountid, string playergroupname, string description, bool includeinactive)
        {
            var query = from playergroup in db.PlayerGroups
                        select playergroup;
            query = query.Where(pgs => pgs.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(playergroupname))
                query = query.Where(pgs => pgs.PlayerGroupName.StartsWith(playergroupname));
            if (!String.IsNullOrEmpty(description))
                query = query.Where(pgs => pgs.PlayerGroupDescription.Contains(description));
            if (!includeinactive)
                query = query.Where(pgs => pgs.IsActive == true);

            // Get a Count of all filtered records
            return query.Count();
        }

        public void CreatePlayerGroup(PlayerGroup playergroup)
        {
            db.PlayerGroups.Add(playergroup);
            db.SaveChanges();
        }

        public void UpdatePlayerGroup(PlayerGroup playergroup)
        {
            db.Entry(playergroup).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}