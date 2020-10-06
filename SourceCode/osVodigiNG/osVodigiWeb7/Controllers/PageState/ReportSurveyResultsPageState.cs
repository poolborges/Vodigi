using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Controllers
{
    public class ReportSurveyResultsPageState
    {
        public int AccountID { get; set; }
        public int SurveyID { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}