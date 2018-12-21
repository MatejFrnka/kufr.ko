using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Message
{
    public class SingleMessage
    {
        public ulong Id { get; set; }
        public bool Edited { get; set; }
        public string Text { get; set; }
        public List<uint> Id_Attachment { get; set; }
        public uint Id_Group { get; set; }
        public UserInfo UserInfo { get; set; }
        public DateTime Sent { get; set; }
    }
}