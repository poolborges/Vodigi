using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class SlideShowMusicXref
    {
        public int SlideShowMusicXrefID { get; set; }
        public int SlideShowID { get; set; }
        public int MusicID { get; set; }
        public int PlayOrder { get; set; }
    }
}