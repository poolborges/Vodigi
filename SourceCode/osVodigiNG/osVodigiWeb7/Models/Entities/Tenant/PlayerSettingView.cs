using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class PlayerSettingView : IComparable<PlayerSettingView>
    {
        public int PlayerSettingID { get; set; }
        public int PlayerID { get; set; }
        public string PlayerName { get; set; }
        public string PlayerSettingName { get; set; }
        public int PlayerSettingTypeID { get; set; }
        public string PlayerSettingTypeName { get; set; }
        public string PlayerSettingValue { get; set; }
        public string PlayerSettingDescription { get; set; }

        public int CompareTo(PlayerSettingView psv)
        {
            return this.PlayerSettingName.CompareTo(psv.PlayerSettingName);
        }

    }
}