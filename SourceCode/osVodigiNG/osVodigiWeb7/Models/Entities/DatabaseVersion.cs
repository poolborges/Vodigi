using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class DatabaseVersion
    {
        public int DatabaseVersionID { get; set; }
        public string Version { get; set; }
        public DateTime DateInstalled { get; set; }
    }
}