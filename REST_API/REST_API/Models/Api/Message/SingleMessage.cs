using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Message
{
    public class SingleMessage
    {
        public string Text { get; set; }
        public int[] Id_Attachment { get; set; }
        public int Id_Group { get; set; }
        public int Id_UserSender { get; set; }
        public DateTime Sent { get; set; }
    }
}