﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb7x.Controllers
{
    public class MusicPageState
    {
        public int AccountID { get; set; }
        public string MusicName { get; set; }
        public string Tag { get; set; }
        public bool IncludeInactive { get; set; }
        public string SortBy { get; set; }
        public string AscDesc { get; set; }
        public int PageNumber { get; set; }
    }
}