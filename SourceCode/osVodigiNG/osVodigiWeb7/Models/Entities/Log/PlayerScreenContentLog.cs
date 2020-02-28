using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class PlayerScreenContentLog
    {
        public int PlayerScreenContentLogID { get; set; }
        public int AccountID { get; set; }
        public int PlayerID { get; set; }
        public string PlayerName { get; set; }
        public int ScreenID { get; set; }
        public string ScreenName { get; set; }
        public int ScreenContentID { get; set; }
        public string ScreenContentName { get; set; }
        public int ScreenContentTypeID { get; set; }
        public string ScreenContentTypeName { get; set; }
        public DateTime DisplayDateTime { get; set; }
        public DateTime CloseDateTime { get; set; }
        public string ContentDetails { get; set; }
    }
}