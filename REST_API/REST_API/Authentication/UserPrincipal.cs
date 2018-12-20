using REST_API.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace REST_API.Authentication
{
    public class UserPrincipal : IPrincipal
    {
        public User DbUser { get; set; }
        public IIdentity Identity => new GenericIdentity("AAA");

        public UserPrincipal(User user)
        {
            this.DbUser = user;
        }

        public bool IsInRole(string role)
        {
            return false;
        }
    }
}