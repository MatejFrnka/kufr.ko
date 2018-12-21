using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Database
{
    public class MessageHistory
    {
        public ulong Id { get; set; }
        public ulong Id_Message { get; set; }
        public string TextBody { get; set; }
        public bool File { get; set; }
        public DateTime ChangedTime { get; set; }
    }
}