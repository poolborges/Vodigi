using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace osVodigiWeb7x.Models
{
    public class User
    {
        public int UserID { get; set; }
        public int AccountID { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [EmailAddress]
        public string EmailAddress { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
    }
}