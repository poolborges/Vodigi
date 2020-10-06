using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class AnsweredSurvey
    {
        public int AnsweredSurveyID { get; set; }
        public int AccountID { get; set; }
        public int SurveyID { get; set; }
        public int PlayerID { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}