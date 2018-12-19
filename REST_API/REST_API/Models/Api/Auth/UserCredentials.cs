using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Auth
{
    public class UserCredentials
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}