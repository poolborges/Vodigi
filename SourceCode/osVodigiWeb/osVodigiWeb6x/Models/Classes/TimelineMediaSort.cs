using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class TimelineMediaSort : IComparable<TimelineMediaSort>
    {
        public int DisplayOrder { get; set; }
        public string guid { get; set; }

        public int CompareTo(TimelineMediaSort tlms)
        {
            return this.DisplayOrder.CompareTo(tlms.DisplayOrder);
        }
    }
}