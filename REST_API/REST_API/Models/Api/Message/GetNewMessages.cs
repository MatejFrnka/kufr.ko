using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Message
{
    public class GetNewMessages
    {
        public ulong Id_Last { get; set; }
        //leave null for all groups
        public List<uint> Groups { get; set; }
    }
}