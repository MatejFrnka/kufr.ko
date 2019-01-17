using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Attachments
{
    public class AttachmentUpload
    {
        /// <summary>
        /// Attachment content in Base64
        /// </summary>
        public string Data { get; set; }
    }
}