using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace osVodigiWeb6x.Models
{
    public class ApplicationError
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ErrorMessage { get; set; }
    }
}