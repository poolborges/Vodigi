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
    public class EntityPlayerScreenLogRepository : IPlayerScreenLogRepository
    {
        private VodigiLogsContext db = new VodigiLogsContext();

        public void CreatePlayerScreenLog(PlayerScreenLog playerscreenlog)
        {
            db.PlayerScreenLogs.Add(playerscreenlog);
            db.SaveChanges();
        }

        public PlayerScreenLog GetPlayerScreenLog(int id)
        {
            PlayerScreenLog playerscreenlog = db.PlayerScreenLogs.Find(id);

            return playerscreenlog;
        }

        public void UpdatePlayerScreenLog(PlayerScreenLog playerscreenlog)
        {
            db.Entry(playerscreenlog).State = EntityState.Modified;
            db.SaveChanges();
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}