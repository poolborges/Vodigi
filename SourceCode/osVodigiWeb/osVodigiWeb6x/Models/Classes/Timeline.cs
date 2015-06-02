using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class Timeline
    {
        public int TimelineID { get; set; }
        public int AccountID { get; set; }
        public string TimelineName { get; set; }
        public string Tags { get; set; }
        public bool IsActive { get; set; }
        public int DurationInSecs { get; set; }
        public bool MuteMusicOnPlayback { get; set; }
    }
}