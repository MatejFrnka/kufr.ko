using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Message
{
    public class EditMessage
    {
        public ulong? Id_Message { get; set; }
        public int[] Id_Attachment { get; set; }
    }
}