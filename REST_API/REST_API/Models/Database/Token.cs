using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Database
{
    public class Token
    {
        public int Id { get; set; }
        public int Id_User { get; set; }
        public string Value { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool Active { get; set; }
    }
}