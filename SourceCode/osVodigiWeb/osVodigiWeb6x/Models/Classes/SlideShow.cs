using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class SlideShow
    {
        public int SlideShowID { get; set; }
        public int AccountID { get; set; }
        public string SlideShowName { get; set; }
        public string Tags { get; set; }
        public int IntervalInSecs { get; set; }
        public string TransitionType { get; set; }
        public bool IsActive { get; set; }
    }
}