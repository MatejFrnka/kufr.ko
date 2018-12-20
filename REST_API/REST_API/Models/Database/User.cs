using REST_API.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Database
{
    public class User
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastOnline { get; set; }
        public Visibility Visibility { get; set; }
        public uint Id_Attachment { get; set; }
    }
}