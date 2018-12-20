using REST_API.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api
{
    public class Response
    {
        public StatusCode StatusCode { get; set; }
        public object Data { get; set; }
    }
}