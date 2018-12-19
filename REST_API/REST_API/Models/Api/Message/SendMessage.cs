using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Message
{
    public class SendMessage : IRequestType
    {
        public int?[] Id_Attachment { get; set; }
        public int Id_Group { get; set; }
        public string Text { get; set; }
    }
}