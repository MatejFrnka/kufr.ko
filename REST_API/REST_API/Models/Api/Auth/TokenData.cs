using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace REST_API.Models.Api.Auth
{
    public class TokenData : IResponseData
    {
        public string Token { get; set; }
    }
}