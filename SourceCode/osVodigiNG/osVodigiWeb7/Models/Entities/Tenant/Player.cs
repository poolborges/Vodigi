using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class Player
    {
        public int PlayerID { get; set; }
        public int AccountID { get; set; }
        public int PlayerGroupID { get; set; }
        public string PlayerName { get; set; }
        public string PlayerLocation { get; set; }
        public string PlayerDescription { get; set; }
        public bool IsActive { get; set; }
    }
}