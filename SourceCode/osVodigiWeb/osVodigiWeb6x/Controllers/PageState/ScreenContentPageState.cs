using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Controllers
{
    public class ScreenContentPageState
    {
        public int AccountID { get; set; }
        public string ScreenContentName { get; set; }
        public int ScreenContentTypeID { get; set; }
        public bool IncludeInactive { get; set; }
        public string SortBy { get; set; }
        public string AscDesc { get; set; }
        public int PageNumber { get; set; }
    }
}