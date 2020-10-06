using System;
using Microsoft.AspNetCore.Mvc;

namespace osVodigiWeb7.Controllers.ViewModel
{
    public class LoginViewModel
    {

        [FromQuery(Name = "txtUsername")]
        public string Username { get; set; }

        [FromQuery(Name = "txtPassword")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public string RememberMe { get; set; }
    }
}
