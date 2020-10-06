using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class SurveyQuestionOption
    {
        public int SurveyQuestionOptionID { get; set; }
        public int SurveyQuestionID { get; set; }
        public string SurveyQuestionOptionText { get; set; }
        public int SortOrder { get; set; }
    }
}