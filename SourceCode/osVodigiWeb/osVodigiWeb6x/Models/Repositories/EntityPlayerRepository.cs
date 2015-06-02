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
    public class EntityPlayerRepository : IPlayerRepository
    {
        private VodigiContext db = new VodigiContext();

        public Player GetPlayer(int id)
        {
            Player player = db.Players.Find(id);

            return player;
        }

        public IEnumerable<Player> GetPlayerPage(int accountid, int playergroupid, string playername, bool includeinactive, string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            var query = from player in db.Players
                        select player;
            query = query.Where(pls => pls.AccountID.Equals(accountid));
            if (playergroupid > 0)
                query = query.Where(pls => pls.PlayerGroupID.Equals(playergroupid));
            if (!String.IsNullOrEmpty(playername))
                query = query.Where(pls => pls.PlayerName.StartsWith(playername));
            if (!includeinactive)
                query = query.Where(pls => pls.IsActive == true);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<Player> players = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return players;
        }

        public IEnumerable<Player> GetPlayersByPlayerGroup(int playergroupid)
        {
            var query = from player in db.Players
                        select player;
            query = query.Where(pls => pls.PlayerGroupID.Equals(playergroupid));
            query = query.OrderBy("PlayerName", false);

            List<Player> players = query.ToList();

            return players;
        }

        public IEnumerable<Player> GetAllPlayers(int accountid)
        {
            var query = from player in db.Players
                        select player;
            query = query.Where(pls => pls.AccountID.Equals(accountid));
            query = query.OrderBy("PlayerName", false);

            List<Player> players = query.ToList();

            return players;
        }

        public IEnumerable<Player> GetPlayerByName(int accountid, string playername)
        {
            var query = from player in db.Players
                        select player;
            query = query.Where(pls => pls.AccountID.Equals(accountid));
            query = query.Where(pls => pls.PlayerName.Equals(playername));
            query = query.OrderBy("IsActive", true);

            List<Player> players = query.ToList();

            return players;
        }

        public int GetPlayerRecordCount(int accountid, int playergroupid, string playername, bool includeinactive)
        {
            var query = from player in db.Players
                        select player;
            query = query.Where(pls => pls.AccountID.Equals(accountid));
            if (playergroupid > 0)
                query = query.Where(pls => pls.PlayerGroupID.Equals(playergroupid));
            if (!String.IsNullOrEmpty(playername))
                query = query.Where(pls => pls.PlayerName.StartsWith(playername));
            if (!includeinactive)
                query = query.Where(pls => pls.IsActive == true);

            // Get a Count of all filtered records
            return query.Count();
        }

        public void CreatePlayer(Player player)
        {
            db.Players.Add(player);
            db.SaveChanges();
        }

        public void UpdatePlayer(Player player)
        {
            db.Entry(player).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}