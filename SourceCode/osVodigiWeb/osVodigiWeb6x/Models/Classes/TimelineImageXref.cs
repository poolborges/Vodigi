using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class TimelineImageXref
    {
        public int TimelineImageXrefID { get; set; }
        public int TimelineID { get; set; }
        public int ImageID { get; set; }
        public int DisplayOrder { get; set; }
    }
}