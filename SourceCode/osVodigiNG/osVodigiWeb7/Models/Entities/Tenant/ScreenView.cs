﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb7x.Models
{
    public class ScreenView
    {
        public int ScreenID { get; set; }
        public int AccountID { get; set; }
        public string ScreenName { get; set; }
        public string ScreenDescription { get; set; }
        public int SlideShowID { get; set; }
        public string SlideShowName { get; set; }
        public int PlayListID { get; set; }
        public string PlayListName { get; set; }
        public bool IsInteractive { get; set; }
        public int ButtonImageID { get; set; }
        public bool IsActive { get; set; }
        public int TimelineID { get; set; }
    }
}