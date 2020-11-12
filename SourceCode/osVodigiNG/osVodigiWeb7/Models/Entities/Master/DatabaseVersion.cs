using System;

namespace osVodigiWeb7x.Models
{
    public class DatabaseVersion
    {
        public int DatabaseVersionID { get; set; }
        public string Version { get; set; }
        public DateTime DateInstalled { get; set; }
    }
}