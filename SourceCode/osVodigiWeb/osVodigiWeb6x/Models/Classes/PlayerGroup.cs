using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class PlayerGroup
    {
        public int PlayerGroupID { get; set; }
        public int AccountID { get; set; }
        public string PlayerGroupName { get; set; }
        public string PlayerGroupDescription { get; set; }
        public bool IsActive { get; set; }
    }
}