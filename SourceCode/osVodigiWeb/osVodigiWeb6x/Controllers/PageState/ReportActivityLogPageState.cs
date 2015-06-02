using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Controllers
{
    public class ReportActivityLogPageState
    {
        public int AccountID { get; set; }
        public string Username { get; set; }
        public string EntityType { get; set; }
        public string EntityAction { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string SortBy { get; set; }
        public string AscDesc { get; set; }
        public int PageNumber { get; set; }
    }
}