using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class SlideShowImageXref
    {
        public int SlideShowImageXrefID { get; set; }
        public int SlideShowID { get; set; }
        public int ImageID { get; set; }
        public int PlayOrder { get; set; }
    }
}