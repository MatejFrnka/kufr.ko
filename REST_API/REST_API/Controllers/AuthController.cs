using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using REST_API.Models.Api;
using REST_API.Models.Api.Auth;

namespace REST_API.Controllers
{
    public class AuthController : ApiController
    {
        public Response Login([FromBody]UserCredentials user)
        {
            return new Response();
        }

        public Response Register([FromBody]UserCredentials user)
        {
            return new Response();
        }
    }
}
