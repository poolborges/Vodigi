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
    public class EntityPlayerSettingAccountDefaultRepository : IPlayerSettingAccountDefaultRepository
    {
        private VodigiContext db = new VodigiContext();

        public PlayerSettingAccountDefault GetByPlayerSettingAccountDefaultID(int id)
        {
            PlayerSettingAccountDefault setting = db.PlayerSettingAccountDefaults.Find(id);

            return setting;
        }

        public PlayerSettingAccountDefault GetByPlayerSettingName(int accountid, string playersettingname)
        {
            var query = from playersettingaccountdefault in db.PlayerSettingAccountDefaults
                        select playersettingaccountdefault;
            query = query.Where(psads => psads.AccountID.Equals(accountid));
            query = query.Where(psads => psads.PlayerSettingName.Equals(playersettingname));

            List<PlayerSettingAccountDefault> defaults = query.ToList();

            if (defaults.Count > 0)
                return defaults[0];
            else
                return null;
        }

        public IEnumerable<PlayerSettingAccountDefault> GetAllPlayerSettingAccountDefaults(int accountid)
        {
            var query = from playersettingaccountdefault in db.PlayerSettingAccountDefaults
                        select playersettingaccountdefault;
            query = query.Where(psads => psads.AccountID.Equals(accountid));

            query = query.OrderBy("PlayerSettingName", false);

            List<PlayerSettingAccountDefault> defaults = query.ToList();

            return defaults;
        }

        public void CreatePlayerSettingAccountDefault(PlayerSettingAccountDefault playersettingaccountdefault)
        {
            db.PlayerSettingAccountDefaults.Add(playersettingaccountdefault);
            db.SaveChanges();
        }

        public void UpdatePlayerSettingAccountDefault(PlayerSettingAccountDefault playersettingaccountdefault)
        {
            db.Entry(playersettingaccountdefault).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

    }
}