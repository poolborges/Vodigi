using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class TimelineMusicXref
    {
        public int TimelineMusicXrefID { get; set; }
        public int TimelineID { get; set; }
        public int MusicID { get; set; }
        public int PlayOrder { get; set; }
    }
}