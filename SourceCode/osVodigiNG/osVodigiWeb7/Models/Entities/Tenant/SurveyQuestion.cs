using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class SurveyQuestion
    {
        public int SurveyQuestionID { get; set; }
        public int SurveyID { get; set; }
        public string SurveyQuestionText { get; set; }
        public bool AllowMultiSelect { get; set; }
        public int SortOrder { get; set; }
    }
}