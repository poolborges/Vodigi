using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class PlayerSettingAccountDefault
    {
        public int PlayerSettingAccountDefaultID { get; set; }
        public int AccountID { get; set; }
        public string PlayerSettingName { get; set; }
        public int PlayerSettingTypeID { get; set; }
        public string PlayerSettingAccountDefaultValue { get; set; }
    }
}