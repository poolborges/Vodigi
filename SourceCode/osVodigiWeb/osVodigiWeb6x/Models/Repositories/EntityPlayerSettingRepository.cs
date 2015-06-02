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
    public class EntityPlayerSettingRepository : IPlayerSettingRepository
    {
        private VodigiContext db = new VodigiContext();

        public PlayerSetting GetByPlayerSettingID(int id)
        {
            PlayerSetting playersetting = db.PlayerSettings.Find(id);

            return playersetting;
        }

        public IEnumerable<PlayerSetting> GetAllPlayerSettings(int playerid)
        {
            var query = from playersetting in db.PlayerSettings
                        select playersetting;
            query = query.Where(pss => pss.PlayerID.Equals(playerid));
            query = query.OrderBy("PlayerSettingName", false);

            List<PlayerSetting> playersettings = query.ToList();

            return playersettings;
        }

        public PlayerSetting GetByPlayerSettingName(int playerid, string playersettingname)
        {
            var query = from playersetting in db.PlayerSettings
                        select playersetting;
            query = query.Where(pss => pss.PlayerID.Equals(playerid));
            query = query.Where(pss => pss.PlayerSettingName.Equals(playersettingname));

            List<PlayerSetting> playersettings = query.ToList();

            if (playersettings.Count > 0)
                return playersettings[0];
            else
                return null;
        }

        public void CreatePlayerSetting(PlayerSetting playersetting)
        {
            db.PlayerSettings.Add(playersetting);
            db.SaveChanges();
        }

        public void UpdatePlayerSetting(PlayerSetting playersetting)
        {
            db.Entry(playersetting).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

    }
}