using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Database
{
    public class Message
    {
        public ulong Id { get; set; }
        public uint Id_User { get; set; }
        public uint Id_Group { get; set; }
        public DateTime Sent { get; set; }
        public string TextBody { get; set; }
        public bool Edited { get; set; }
    }
}