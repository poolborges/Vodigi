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

using System.Data;

namespace osVodigiWeb6x.Models
{
    public class EntityPlayerScreenContentLogRepository : IPlayerScreenContentLogRepository
    {
        private VodigiLogsContext db = new VodigiLogsContext();

        public void CreatePlayerScreenContentLog(PlayerScreenContentLog playerscreencontentlog)
        {
            db.PlayerScreenContentLogs.Add(playerscreencontentlog);
            db.SaveChanges();
        }

        public PlayerScreenContentLog GetPlayerScreenContentLog(int id)
        {
            PlayerScreenContentLog playerscreencontentlog = db.PlayerScreenContentLogs.Find(id);

            return playerscreencontentlog;
        }

        public void UpdatePlayerScreenContentLog(PlayerScreenContentLog playerscreencontentlog)
        {
            db.Entry(playerscreencontentlog).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}