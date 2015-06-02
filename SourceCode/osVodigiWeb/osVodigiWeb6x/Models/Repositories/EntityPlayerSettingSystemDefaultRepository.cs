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
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class EntityPlayerSettingSystemDefaultRepository : IPlayerSettingSystemDefaultRepository
    {
        private VodigiContext db = new VodigiContext();

        public PlayerSettingSystemDefault GetByPlayerSettingSystemDefaultID(int id)
        {
            PlayerSettingSystemDefault setting = db.PlayerSettingSystemDefaults.Find(id);

            return setting;
        }

        public PlayerSettingSystemDefault GetByPlayerSettingName(string playersettingname)
        {
            var query = from playersettingsystemdefault in db.PlayerSettingSystemDefaults
                        select playersettingsystemdefault;
            query = query.Where(pssds => pssds.PlayerSettingName.Equals(playersettingname));

            List<PlayerSettingSystemDefault> defaults = query.ToList();

            if (defaults.Count > 0)
                return defaults[0];
            else
                return null;
        }

        public IEnumerable<PlayerSettingSystemDefault> GetAllPlayerSettingSystemDefaults()
        {
            var query = from playersettingsystemdefault in db.PlayerSettingSystemDefaults
                        select playersettingsystemdefault;
            query = query.OrderBy("PlayerSettingName", false);

            List<PlayerSettingSystemDefault> defaults = query.ToList();

            return defaults;
        }

    }
}