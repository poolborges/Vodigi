using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class PlayListVideoXref
    {
        public int PlayListVideoXrefID { get; set; }
        public int PlayListID { get; set; }
        public int VideoID { get; set; }
        public int PlayOrder { get; set; }
    }
}