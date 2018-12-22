using REST_API.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api
{
    public class UserInfo
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint Id_Attachment { get; set; }
        public string Nickname { get; set; }
    }
}