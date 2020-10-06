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

namespace osVodigiWeb6x.Models
{
    public interface IReportRepository
    {
        IEnumerable<LoginLog> ReportLoginLogPage(int accountid, string username, DateTime startdate, DateTime enddate, string sortby, bool isdescending, int pagenumber, int pagecount);
        int ReportLoginLogRecordCount(int accountid, string username, DateTime startdate, DateTime enddate);

        IEnumerable<ActivityLog> ReportActivityLogPage(int accountid, string username, string entitytype, string entityaction, DateTime startdate, DateTime enddate, string sortby, bool isdescending, int pagenumber, int pagecount);
        int ReportActivityLogRecordCount(int accountid, string username, string entitytype, string entityaction, DateTime startdate, DateTime enddate);

        IEnumerable<PlayerScreenContentLog> ReportPlayerScreenContentLogPage(int accountid, string playername, string screenname, string screencontentname, string screencontenttypename, DateTime startdate, DateTime enddate, string sortby, bool isdescending, int pagenumber, int pagecount);
        int ReportPlayerScreenContentLogRecordCount(int accountid, string playername, string screenname, string screencontentname, string screencontenttypename, DateTime startdate, DateTime enddate);

        IEnumerable<PlayerScreenLog> ReportPlayerScreenLogPage(int accountid, string playername, string screenname, DateTime startdate, DateTime enddate, string sortby, bool isdescending, int pagenumber, int pagecount);
        int ReportPlayerScreenLogRecordCount(int accountid, string playername, string screenname, DateTime startdate, DateTime enddate);
    }
}