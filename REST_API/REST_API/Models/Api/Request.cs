using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api
{
    public class Request : IRequestType
    {
        public int Id_User { get; set; }
        public string Token { get; set; }
        public IRequestType RequestType { get; set; }
    }
}