﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb7x.Models
{
    public class Video
    {
        public int VideoID { get; set; }
        public int AccountID { get; set; }
        public string OriginalFilename { get; set; }
        public string StoredFilename { get; set; }
        public string VideoName { get; set; }
        public string Tags { get; set; }
        public bool IsActive { get; set; }
    }
}