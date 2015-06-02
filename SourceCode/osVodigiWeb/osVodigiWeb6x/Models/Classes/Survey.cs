using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class Survey
    {
        public int SurveyID { get; set; }
        public int AccountID { get; set; }
        public string SurveyName { get; set; }
        public string SurveyDescription { get; set; }
        public int SurveyImageID { get; set; }
        public bool IsApproved { get; set; }
        public bool IsActive { get; set; }
    }
}