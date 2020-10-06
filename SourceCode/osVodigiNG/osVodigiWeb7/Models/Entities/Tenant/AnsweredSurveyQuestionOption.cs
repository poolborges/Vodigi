using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class AnsweredSurveyQuestionOption
    {
        public int AnsweredSurveyQuestionOptionID { get; set; }
        public int AnsweredSurveyID { get; set; }
        public int SurveyQuestionOptionID { get; set; }
        public bool IsSelected { get; set; }
    }
}