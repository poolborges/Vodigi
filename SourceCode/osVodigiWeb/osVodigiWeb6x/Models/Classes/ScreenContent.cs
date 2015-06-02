using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class ScreenContent
    {
        public int ScreenContentID { get; set; }
        public int AccountID { get; set; }
        public int ScreenContentTypeID { get; set; }
        public string ScreenContentName { get; set; }
        public string ScreenContentTitle { get; set; }
        public int ThumbnailImageID { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }
        public bool IsActive { get; set; }
    }
}