using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class LoginLog
    {
        public int LoginLogID { get; set; }
        public int AccountID { get; set; }
        public int UserID { get; set; }
        public string Username { get; set; }
        public DateTime LoginDateTime { get; set; }
    }
}