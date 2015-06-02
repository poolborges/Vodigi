using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class PlayerGroupSchedule
    {
        public int PlayerGroupScheduleID { get; set; }
        public int PlayerGroupID { get; set; }
        public int ScreenID { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}