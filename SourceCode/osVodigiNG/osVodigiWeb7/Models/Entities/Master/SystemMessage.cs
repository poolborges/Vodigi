using System;

namespace osVodigiWeb7x.Models
{
    public class SystemMessage
    {
        public int SystemMessageID { get; set; }
        public string SystemMessageTitle { get; set; }
        public string SystemMessageBody { get; set; }
        public DateTime DisplayDateStart { get; set; }
        public DateTime DisplayDateEnd { get; set; }
        public int Priority { get; set; }
    }
}