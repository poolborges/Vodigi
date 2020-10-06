using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Controllers
{
    public class PlayerGroupPageState
    {
        public int AccountID { get; set; }
        public string PlayerGroupName { get; set; }
        public string Description { get; set; }
        public bool IncludeInactive { get; set; }
        public string SortBy { get; set; }
        public string AscDesc { get; set; }
        public int PageNumber { get; set; }
    }
}