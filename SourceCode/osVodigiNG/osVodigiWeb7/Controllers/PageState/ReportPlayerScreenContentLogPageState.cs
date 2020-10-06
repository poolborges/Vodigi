using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Controllers
{
    public class ReportPlayerScreenContentLogPageState
    {
        public int AccountID { get; set; }
        public string PlayerName { get; set; }
        public string ScreenName { get; set; }
        public string ContentName { get; set; }
        public string ContentType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SortBy { get; set; }
        public string AscDesc { get; set; }
        public int PageNumber { get; set; }
    }
}