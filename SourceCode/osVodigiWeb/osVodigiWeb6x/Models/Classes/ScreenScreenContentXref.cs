using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class ScreenScreenContentXref
    {
        public int ScreenScreenContentXrefID { get; set; }
        public int ScreenID { get; set; }
        public int ScreenContentID { get; set; }
        public int DisplayOrder { get; set; }
    }
}