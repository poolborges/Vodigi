using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class PlayerSetting
    {
        public int PlayerSettingID { get; set; }
        public int PlayerID { get; set; }
        public string PlayerSettingName { get; set; }
        public int PlayerSettingTypeID { get; set; }
        public string PlayerSettingValue { get; set; }
    }
}