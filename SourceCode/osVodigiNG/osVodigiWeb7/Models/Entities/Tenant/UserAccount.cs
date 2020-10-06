using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class UserAccount
    {
        public int UserID { get; set; }
        public int AccountID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsAdmin { get; set; }
        public bool UserIsActive { get; set; }
        public string AccountName { get; set; }
        public string AccountDescription { get; set; }
        public string FTPServer { get; set; }
        public string FTPUsername { get; set; }
        public string FTPPassword { get; set; }
        public bool AccountIsActive { get; set; }
    }
}