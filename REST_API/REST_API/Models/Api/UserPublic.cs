using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Database
{
    public class UserPublic
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint Id_Attachement { get; set; }
        public uint Id_Group { get; set; }
    }
}