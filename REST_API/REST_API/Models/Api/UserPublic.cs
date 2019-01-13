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
        public uint Id_Attachment { get; set; }
        public DateTime? LastOnline { get; set; }
        public uint DefaultGroup { get; set; }
    }
}