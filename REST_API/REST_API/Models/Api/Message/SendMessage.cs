﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Message
{
    public class SendMessage
    {
        public List<uint> Id_Attachment { get; set; }
        public uint Id_Group { get; set; }
        public string Text { get; set; }
    }
}