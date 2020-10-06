using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class Image
    {
        public int ImageID { get; set; }
        public int AccountID { get; set; }
        public string OriginalFilename { get; set; }
        public string StoredFilename { get; set; }
        public string ImageName { get; set; }
        public string Tags { get; set; }
        public bool IsActive { get; set; }
    }
}