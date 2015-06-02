using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class TimelineVideoXref
    {
        public int TimelineVideoXrefID { get; set; }
        public int TimelineID { get; set; }
        public int VideoID { get; set; }
        public int DisplayOrder { get; set; }
    }
}