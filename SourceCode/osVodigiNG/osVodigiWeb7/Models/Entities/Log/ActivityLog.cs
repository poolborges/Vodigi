using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class ActivityLog
    {
        public int ActivityLogID { get; set; }
        public int AccountID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public string EntityType { get; set; }
        public string EntityAction { get; set; }
        public DateTime ActivityDateTime { get; set; }
        public string ActivityDetails { get; set; }
    }
}