using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class PlayList
    {
        public int PlayListID { get; set; }
        public int AccountID { get; set; }
        public string PlayListName { get; set; }
        public string Tags { get; set; }
        public bool IsActive { get; set; }
    }
}