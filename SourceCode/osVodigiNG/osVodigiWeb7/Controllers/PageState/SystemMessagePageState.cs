using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Controllers
{
    public class SystemMessagePageState
    {
        public string SystemMessageTitle { get; set; }
        public string SystemMessageBody { get; set; }
        public string SortBy { get; set; }
        public string AscDesc { get; set; }
        public int PageNumber { get; set; }
    }
}