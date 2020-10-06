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
    public class EntityReportRepository : IReportRepository
    {

        public IEnumerable<ActivityLog> ReportActivityLogPage(int accountid, string username,
                                                        string entitytype, string entityaction,
                                                        DateTime startdate, DateTime enddate,
                                                        string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            VodigiLogsContext db = new VodigiLogsContext();

            var query = from activitylog in db.ActivityLogs
                        select activitylog;
            query = query.Where(als => als.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(username))
                query = query.Where(als => als.Username.Equals(username));
            if (!String.IsNullOrEmpty(entitytype))
                query = query.Where(als => als.EntityType.Equals(entitytype));
            if (!String.IsNullOrEmpty(entityaction))
                query = query.Where(als => als.EntityAction.Equals(entityaction));
            query = query.Where(als => als.ActivityDateTime >= startdate);
            query = query.Where(als => als.ActivityDateTime < enddate);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<ActivityLog> activitylogs = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return activitylogs;
        }

        public int ReportActivityLogRecordCount(int accountid, string username,
                                                string entitytype, string entityaction,
                                                DateTime startdate, DateTime enddate)
        {
            VodigiLogsContext db = new VodigiLogsContext();

            var query = from activitylog in db.ActivityLogs
                        select activitylog;
            query = query.Where(als => als.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(username))
                query = query.Where(als => als.Username.Equals(username));
            if (!String.IsNullOrEmpty(entitytype))
                query = query.Where(als => als.EntityType.Equals(entitytype));
            if (!String.IsNullOrEmpty(entityaction))
                query = query.Where(als => als.EntityAction.Equals(entityaction));
            query = query.Where(als => als.ActivityDateTime >= startdate);
            query = query.Where(als => als.ActivityDateTime < enddate);

            // Get a Count of all filtered records
            return query.Count();
        }

        public IEnumerable<LoginLog> ReportLoginLogPage(int accountid, string username, DateTime startdate,
                                                        DateTime enddate,
                                                        string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            VodigiLogsContext db = new VodigiLogsContext();

            var query = from loginlog in db.LoginLogs
                        select loginlog;
            query = query.Where(lls => lls.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(username))
                query = query.Where(lls => lls.Username.Equals(username));
            query = query.Where(lls => lls.LoginDateTime >= startdate);
            query = query.Where(lls => lls.LoginDateTime < enddate);
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber*Constants.PageSize) - Constants.PageSize;

            List<LoginLog> loginlogs = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return loginlogs;
        }

        public int ReportLoginLogRecordCount(int accountid, string username, DateTime startdate, DateTime enddate)
        {
            VodigiLogsContext db = new VodigiLogsContext();

            var query = from loginlog in db.LoginLogs
                        select loginlog;
            query = query.Where(lls => lls.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(username))
                query = query.Where(lls => lls.Username.Equals(username));
            query = query.Where(lls => lls.LoginDateTime >= startdate);
            query = query.Where(lls => lls.LoginDateTime < enddate);

            // Get a Count of all filtered records
            return query.Count();
        }

        public IEnumerable<PlayerScreenContentLog> ReportPlayerScreenContentLogPage(int accountid, string playername,
                                                        string screenname, string screencontentname, string screencontenttypename,
                                                        DateTime startdate, DateTime enddate,
                                                        string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            VodigiLogsContext db = new VodigiLogsContext();

            var query = from playerscreencontentlog in db.PlayerScreenContentLogs
                        select playerscreencontentlog;
            query = query.Where(pscls => pscls.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(playername))
                query = query.Where(pscls => pscls.PlayerName.Equals(playername));
            if (!String.IsNullOrEmpty(screenname))
                query = query.Where(pscls => pscls.ScreenName.Equals(screenname));
            if (!String.IsNullOrEmpty(screencontentname))
                query = query.Where(pscls => pscls.ScreenContentName.Equals(screencontentname));
            if (!String.IsNullOrEmpty(screencontenttypename))
                query = query.Where(pscls => pscls.ScreenContentTypeName.Equals(screencontenttypename));
            query = query.Where(pscls => pscls.DisplayDateTime >= startdate);
            query = query.Where(pscls => pscls.DisplayDateTime < enddate);

            // Apply the ordering
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<PlayerScreenContentLog> playerscreencontentlogs = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return playerscreencontentlogs;
        }

        public int ReportPlayerScreenContentLogRecordCount(int accountid, string playername,
                                                        string screenname, string screencontentname, string screencontenttypename,
                                                        DateTime startdate, DateTime enddate)
        {
            VodigiLogsContext db = new VodigiLogsContext();

            var query = from playerscreencontentlog in db.PlayerScreenContentLogs
                        select playerscreencontentlog;
            query = query.Where(pscls => pscls.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(playername))
                query = query.Where(pscls => pscls.PlayerName.Equals(playername));
            if (!String.IsNullOrEmpty(screenname))
                query = query.Where(pscls => pscls.ScreenName.Equals(screenname));
            if (!String.IsNullOrEmpty(screencontentname))
                query = query.Where(pscls => pscls.ScreenContentName.Equals(screencontentname));
            if (!String.IsNullOrEmpty(screencontenttypename))
                query = query.Where(pscls => pscls.ScreenContentTypeName.Equals(screencontenttypename));
            query = query.Where(pscls => pscls.DisplayDateTime >= startdate);
            query = query.Where(pscls => pscls.DisplayDateTime < enddate);

            // Get a Count of all filtered records
            return query.Count();
        }

        public IEnumerable<PlayerScreenLog> ReportPlayerScreenLogPage(int accountid, string playername, string screenname, 
                                                        DateTime startdate, DateTime enddate,
                                                        string sortby, bool isdescending, int pagenumber, int pagecount)
        {
            VodigiLogsContext db = new VodigiLogsContext();

            var query = from playerscreenlog in db.PlayerScreenLogs
                        select playerscreenlog;
            query = query.Where(psls => psls.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(playername))
                query = query.Where(psls => psls.PlayerName.Equals(playername));
            if (!String.IsNullOrEmpty(screenname))
                query = query.Where(psls => psls.ScreenName.Equals(screenname));
            query = query.Where(psls => psls.DisplayDateTime >= startdate);
            query = query.Where(psls => psls.DisplayDateTime < enddate);

            // Apply the ordering
            if (!String.IsNullOrEmpty(sortby))
                query = query.OrderBy(sortby, isdescending);

            // Get a single page from the filtered records
            int iSkip = (pagenumber * Constants.PageSize) - Constants.PageSize;

            List<PlayerScreenLog> playerscreenlogs = query.Skip(iSkip).Take(Constants.PageSize).ToList();

            return playerscreenlogs;
        }

        public int ReportPlayerScreenLogRecordCount(int accountid, string playername, string screenname,
                                                        DateTime startdate, DateTime enddate)
        {
            VodigiLogsContext db = new VodigiLogsContext();

            var query = from playerscreenlog in db.PlayerScreenLogs
                        select playerscreenlog;
            query = query.Where(psls => psls.AccountID.Equals(accountid));
            if (!String.IsNullOrEmpty(playername))
                query = query.Where(psls => psls.PlayerName.Equals(playername));
            if (!String.IsNullOrEmpty(screenname))
                query = query.Where(psls => psls.ScreenName.Equals(screenname));
            query = query.Where(psls => psls.DisplayDateTime >= startdate);
            query = query.Where(psls => psls.DisplayDateTime < enddate);

            // Get a Count of all filtered records
            return query.Count();
        }

    }
}