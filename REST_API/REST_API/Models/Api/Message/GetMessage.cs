using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Message
{
    public class GetMessage : IRequestType
    {
        public int Id_Group { get; set; }
        //0 = newest message
        public int PaginationStart { get; set; }
        public int PaginationAmount { get; set; }
    }
}