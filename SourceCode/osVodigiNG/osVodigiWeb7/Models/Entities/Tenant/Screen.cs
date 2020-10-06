using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class Screen
    {
        public int ScreenID { get; set; }
        public int AccountID { get; set; }
        public string ScreenName { get; set; }
        public string ScreenDescription { get; set; }
        public int SlideShowID { get; set; }
        public int PlayListID { get; set; }
        public bool IsInteractive { get; set; }
        public int ButtonImageID { get; set; }
        public bool IsActive { get; set; }
        public int TimelineID { get; set; }
    }
}