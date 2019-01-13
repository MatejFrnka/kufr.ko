using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Attachments
{
    public class AttachmentRequest
    {
        public uint Id_Attachment { get; set; }
        public ulong Id_Message { get; set; }
    }
}