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
using System.Web.Mvc;
using System.Text;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class PlayerGroupScheduleController : Controller
    {
        private VodigiContext db = new VodigiContext();

        //
        // GET: /PlayerGroupSchedule/Edit/5

        public ActionResult Edit(int id)
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    ViewData["txtIsAdmin"] = "false";

                // Delete a schedule entry if specified
                IPlayerGroupScheduleRepository schedulerep = new EntityPlayerGroupScheduleRepository();
                string scheduleid = Request.QueryString["delete"];
                if (!String.IsNullOrEmpty(scheduleid))
                {
                    schedulerep.DeletePlayerGroupSchedule(Convert.ToInt32(scheduleid));
                }

                // Delete all schedule entries if specified
                string deleteall = Request.QueryString["deleteall"];
                if (!String.IsNullOrEmpty(deleteall))
                {
                    schedulerep.DeletePlayerGroupSchedulesByPlayerGroup(id);
                }

                PlayerGroup playergroup = db.PlayerGroups.Find(id);
                ViewData["PlayerGroupID"] = id;
                ViewData["PlayerGroupName"] = playergroup.PlayerGroupName;

                ViewData["ScreenList"] = new SelectList(BuildScreenList(), "Value", "Text", "");
                ViewData["HourList"] = new SelectList(BuildHourList(), "Value", "Text", "");
                ViewData["MinuteList"] = new SelectList(BuildMinuteList(), "Value", "Text", "");

                ViewData["ScheduleHTML"] = BuildScheduleTable(id);

                PlayerGroupSchedule playergroupschedule = new PlayerGroupSchedule();
                return View(playergroupschedule);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("PlayerGroupSchedule", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /PlayerGroupSchedule/Edit/5

        [HttpPost]
        public ActionResult Edit(PlayerGroupSchedule playergroupschedule)
        {
            try
            {
                if (Session["UserAccountID"] == null)
                    return RedirectToAction("Validate", "Login");
                User user = (User)Session["User"];
                ViewData["LoginInfo"] = Utility.BuildUserAccountString(user.Username, Convert.ToString(Session["UserAccountName"]));
                if (user.IsAdmin)
                    ViewData["txtIsAdmin"] = "true";
                else
                    ViewData["txtIsAdmin"] = "false";

                int playergroupid = Convert.ToInt32(Request.Form["txtPlayerGroupID"]);
                int screenid = Convert.ToInt32(Request.Form["lstScreen"]);
                int hour = Convert.ToInt32(Request.Form["lstHour"]);
                int minute = Convert.ToInt32(Request.Form["lstMinute"]);

                if (!String.IsNullOrEmpty(Request.Form["chkSunday"]))
                    CreatePlayerGroupSchedule(playergroupid, screenid, 0, hour, minute);
                if (!String.IsNullOrEmpty(Request.Form["chkMonday"]))
                    CreatePlayerGroupSchedule(playergroupid, screenid, 1, hour, minute);
                if (!String.IsNullOrEmpty(Request.Form["chkTuesday"]))
                    CreatePlayerGroupSchedule(playergroupid, screenid, 2, hour, minute);
                if (!String.IsNullOrEmpty(Request.Form["chkWednesday"]))
                    CreatePlayerGroupSchedule(playergroupid, screenid, 3, hour, minute);
                if (!String.IsNullOrEmpty(Request.Form["chkThursday"]))
                    CreatePlayerGroupSchedule(playergroupid, screenid, 4, hour, minute);
                if (!String.IsNullOrEmpty(Request.Form["chkFriday"]))
                    CreatePlayerGroupSchedule(playergroupid, screenid, 5, hour, minute);
                if (!String.IsNullOrEmpty(Request.Form["chkSaturday"]))
                    CreatePlayerGroupSchedule(playergroupid, screenid, 6, hour, minute);

                IPlayerGroupRepository pgrep = new EntityPlayerGroupRepository();
                PlayerGroup playergroup = pgrep.GetPlayerGroup(playergroupid);
                ViewData["PlayerGroupID"] = playergroupid;
                ViewData["PlayerGroupName"] = playergroup.PlayerGroupName;

                ViewData["ScreenList"] = new SelectList(BuildScreenList(), "Value", "Text", "");
                ViewData["HourList"] = new SelectList(BuildHourList(), "Value", "Text", "");
                ViewData["MinuteList"] = new SelectList(BuildMinuteList(), "Value", "Text", "");

                ViewData["ScheduleHTML"] = BuildScheduleTable(playergroupid);

                return View(playergroupschedule);
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("PlayerGroupSchedule", "Edit", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        private string BuildScheduleTable(int playergroupid)
        {
            try
            {
                // Get the player group schedules for this player group
                IPlayerGroupScheduleRepository schedulerep = new EntityPlayerGroupScheduleRepository();
                List<PlayerGroupSchedule> schedules = schedulerep.GetPlayerGroupSchedulesByPlayerGroup(playergroupid).ToList();

                string uri = Request.Url.AbsoluteUri;
                if (uri.Contains("?"))
                    uri = uri.Substring(0, uri.IndexOf('?'));

                // Build out the HTML to display the schedules
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("");
                sb.AppendLine("<table style=\"border-spacing:0;border-collapse:collapse;\" class=\"gridtable\">");
                sb.AppendLine("   <tr>");
                sb.AppendLine("      <td style=\"text-align:right;\" class=\"gridheader\" colspan=\"8\"><span id=\"clearlink\"><a href=\"" + uri + "?deleteall=1\">Clear Schedule</a></span></td>");
                sb.AppendLine("   </tr>");
                sb.AppendLine("   <tr>");
                sb.AppendLine("      <td class=\"gridheader\">Hour</td>");
                sb.AppendLine("      <td class=\"gridheader\">Sunday</td>");
                sb.AppendLine("      <td class=\"gridheader\">Monday</td>");
                sb.AppendLine("      <td class=\"gridheader\">Tuesday</td>");
                sb.AppendLine("      <td class=\"gridheader\">Wednesday</td>");
                sb.AppendLine("      <td class=\"gridheader\">Thursday</td>");
                sb.AppendLine("      <td class=\"gridheader\">Friday</td>");
                sb.AppendLine("      <td class=\"gridheader\">Saturday</td>");
                sb.AppendLine("   </tr>");

                for (int hour = 0; hour < 24; hour += 1)
                {
                    sb.AppendLine("   <tr class=\"gridrow\" style=\"min-height:35px;\">");
                    // Hour
                    sb.Append("      <td style=\"border-right: 1px solid #CCCCCC;\">");
                    sb.Append("<b>" + String.Format("{0:00}", hour) + ":00</b>");
                    sb.AppendLine("</td>");
                    // Sunday
                    sb.Append("      <td style=\"border-right: 1px solid #CCCCCC;\">");
                    sb.Append(GetSchedulesByDayHour(schedules, 0, hour));
                    sb.AppendLine("</td>");
                    // Monday
                    sb.Append("      <td style=\"border-right: 1px solid #CCCCCC;\">");
                    sb.Append(GetSchedulesByDayHour(schedules, 1, hour));
                    sb.AppendLine("</td>");
                    // Tuesday
                    sb.Append("      <td style=\"border-right: 1px solid #CCCCCC;\">");
                    sb.Append(GetSchedulesByDayHour(schedules, 2, hour));
                    sb.AppendLine("</td>");
                    // Wednesday
                    sb.Append("      <td style=\"border-right: 1px solid #CCCCCC;\">");
                    sb.Append(GetSchedulesByDayHour(schedules, 3, hour));
                    sb.AppendLine("</td>");
                    // Thursday
                    sb.Append("      <td style=\"border-right: 1px solid #CCCCCC;\">");
                    sb.Append(GetSchedulesByDayHour(schedules, 4, hour));
                    sb.AppendLine("</td>");
                    // Friday
                    sb.Append("      <td style=\"border-right: 1px solid #CCCCCC;\">");
                    sb.Append(GetSchedulesByDayHour(schedules, 5, hour));
                    sb.AppendLine("</td>");
                    // Saturday
                    sb.Append("      <td>");
                    sb.Append(GetSchedulesByDayHour(schedules, 6, hour));
                    sb.AppendLine("</td>");
                    sb.AppendLine("   </tr>");
                }

                sb.AppendLine("</table>");
                return sb.ToString();
            }
            catch { return String.Empty; }
        }

        private void CreatePlayerGroupSchedule(int playergroupid, int screenid, int day, int hour, int minute)
        {
            try
            {
                IPlayerGroupScheduleRepository schedulerep = new EntityPlayerGroupScheduleRepository();
                schedulerep.DeletePlayerGroupSchedulesByDayTime(playergroupid, day, hour, minute);

                PlayerGroupSchedule pgs = new PlayerGroupSchedule();
                pgs.PlayerGroupID = playergroupid;
                pgs.ScreenID = screenid;
                pgs.Day = day;
                pgs.Hour = hour;
                pgs.Minute = minute;

                schedulerep.CreatePlayerGroupSchedule(pgs);
            }
            catch { }
        }

        private string GetSchedulesByDayHour(List<PlayerGroupSchedule> schedules, int day, int hour)
        {
            try
            {
                string html = String.Empty;
                IScreenRepository screenrep = new EntityScreenRepository();
                string uri = Request.Url.AbsoluteUri;
                if (uri.Contains("?"))
                    uri = uri.Substring(0, uri.IndexOf('?'));

                foreach (PlayerGroupSchedule schedule in schedules)
                {
                    if (schedule.Day == day && schedule.Hour == hour)
                    {
                        Screen screen = new Screen();
                        if (schedule.ScreenID == 0)
                        {
                            screen.ScreenID = 0;
                            screen.ScreenName = "Blank Screen";
                        }
                        else
                        {
                            screen = screenrep.GetScreen(schedule.ScreenID);
                        }
                        if (screen != null)
                        {
                            html += "<div class=\"schedule\">" + screen.ScreenName + "</br>" +
                                String.Format("{0:00}", schedule.Hour) + ":" + String.Format("{0:00}", schedule.Minute) +
                                "&nbsp;&nbsp;<span id=\"schedulelink\"><a href=\"" + uri + "?delete=" + schedule.PlayerGroupScheduleID.ToString() + "\">Delete</a></span></div>";
                        }
                    }
                }

                return html;
            }
            catch { return String.Empty; }
        }

        private List<SelectListItem> BuildScreenList()
        {
            int accountid = Convert.ToInt32(Session["UserAccountID"]);

            // Build the screen list
            List<SelectListItem> screenitems = new List<SelectListItem>();

            // Add a Blank Screen item
            SelectListItem blankitem = new SelectListItem();
            blankitem.Text = "Blank Screen";
            blankitem.Value = "0";
            screenitems.Add(blankitem);

            IScreenRepository screenrep = new EntityScreenRepository();
            IEnumerable<Screen> screens = screenrep.GetAllScreens(accountid);
            foreach (Screen screen in screens)
            {
                SelectListItem item = new SelectListItem();
                item.Text = screen.ScreenName;
                item.Value = screen.ScreenID.ToString();
                screenitems.Add(item);
            }

            return screenitems;
        }

        private List<SelectListItem> BuildHourList()
        {
            List<SelectListItem> houritems = new List<SelectListItem>();
            for (int hour = 0; hour < 24; hour += 1)
            {
                SelectListItem item = new SelectListItem();
                item.Text = String.Format("{0:00}", hour);
                item.Value = hour.ToString();
                houritems.Add(item);
            }

            return houritems;
        }

        private List<SelectListItem> BuildMinuteList()
        {
            List<SelectListItem> minuteitems = new List<SelectListItem>();
            for (int minute = 0; minute < 60; minute += 1)
            {
                SelectListItem item = new SelectListItem();
                item.Text = String.Format("{0:00}", minute);
                item.Value = minute.ToString();
                minuteitems.Add(item);
            }

            return minuteitems;
        }
    }
}
