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
    public class EntityPlayerSettingTypeRepository : IPlayerSettingTypeRepository
    {
        private VodigiContext db = new VodigiContext();

        public PlayerSettingType GetPlayerSettingType(int id)
        {
            PlayerSettingType playersettingtype = db.PlayerSettingTypes.Find(id);

            return playersettingtype;
        }

        public IEnumerable<PlayerSettingType> GetAllPlayerSettingTypes()
        {
            var query = from playersettingtype in db.PlayerSettingTypes
                        select playersettingtype;
            query = query.OrderBy("PlayerSettingType", false);

            List<PlayerSettingType> playersettingtypes = query.ToList();

            return playersettingtypes;
        }
    }
}