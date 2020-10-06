using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class Music
    {
        public int MusicID { get; set; }
        public int AccountID { get; set; }
        public string OriginalFilename { get; set; }
        public string StoredFilename { get; set; }
        public string MusicName { get; set; }
        public string Tags { get; set; }
        public bool IsActive { get; set; }
    }
}