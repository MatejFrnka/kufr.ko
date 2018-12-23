using REST_API.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Group
{
    public class Group_UserInfo
    {
        public uint Id_User { get; set; }
        public uint Id_Group { get; set; }
        public string Nickname { get; set; }
        public string Name { get; set; }
        public Permission Permission { get; set; }


    }
}