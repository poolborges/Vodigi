using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class Account
    {
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public string FTPServer { get; set; }
        public string FTPUsername { get; set; }
        public string FTPPassword { get; set; }
        public bool IsActive { get; set; }
    }
}