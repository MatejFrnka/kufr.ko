using MySql.Data.MySqlClient;
using REST_API.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Database
{
    public class Group_User
    {
        public uint Id_User { get; set; }
        public uint Id_Group { get; set; }
        public string NickName { get; set; }
        public Permission Permission { get; set; }
        public DateTime? IgnoreExpire { get; set; }
    }
}