﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using REST_API.Models.Database;

namespace REST_API.Models.Api.Attachments
{
    public class AttachmentData
    {
        public AttachmentMessage MessageInfo { get; set; }
        public string Data { get; set; }
    }
}