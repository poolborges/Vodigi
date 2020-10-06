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

namespace osVodigiWeb6x.Models
{
    public class EntityActivityLogRepository : IActivityLogRepository
    {
        private VodigiLogsContext db = new VodigiLogsContext();

        public void CreateActivityLog(ActivityLog activitylog)
        {
            db.ActivityLogs.Add(activitylog);
            db.SaveChanges();
        }

        public DateTime GetLastPlayerHeartbeat(int playerid)
        {
            string playeridinfo = "ID: " + playerid.ToString();
            var query = from activitylog in db.ActivityLogs
                        select activitylog;
            query = query.Where(als => als.EntityType.Equals("Player"));
            query = query.Where(als => als.EntityAction.Equals("Heartbeat"));
            query = query.Where(als => als.ActivityDetails.Contains(playeridinfo));
            query = query.OrderBy("ActivityDateTime", true);

            List<ActivityLog> activitylogs = query.ToList();

            if (activitylogs.Count > 0)
                return activitylogs[0].ActivityDateTime;
            else
                return DateTime.MinValue;
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}