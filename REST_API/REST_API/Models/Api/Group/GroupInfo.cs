using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Group
{
    public class GroupInfo
    {
        public uint Id { get; set; }
        public string Name { get; set; }
        public uint Id_Attachment { get; set; }
        public DateTime? IgnoreExpire { get; set; }

        public string LastMessageSender { get; set; }
        public string LastMessageText { get; set; }
        public DateTime? LastMessageDate { get; set; }

        public int NewMessages { get; set; }
    }
}