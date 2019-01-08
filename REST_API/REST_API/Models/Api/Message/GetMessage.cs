using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Message
{
    public class GetMessage
    {
        public uint Id_Group { get; set; }
        //0 = newest message
        public ulong StartId { get; set; }
        public uint Amount { get; set; }
    }
}