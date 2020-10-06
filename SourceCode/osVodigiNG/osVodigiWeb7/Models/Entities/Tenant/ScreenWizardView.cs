using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class ScreenWizardView
    {
        public int ScreenID { get; set; }
        public int AccountID { get; set; }
        public string ScreenName { get; set; }
        public string ScreenDescription { get; set; }
        public string MainFeatureType { get; set; }
        public string MainFeatureName { get; set; }
        public string InteractiveContent { get; set; }
        public bool IsActive { get; set; }
    }
}