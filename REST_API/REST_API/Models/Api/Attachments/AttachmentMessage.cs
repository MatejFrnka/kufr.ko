using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Attachments
{
    public class AttachmentMessage
    {
        public uint Id_Attachment { get; set; }
        public string Filename { get; set; }
        public string Mime { get; set; }
    }
}