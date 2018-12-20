using REST_API.Models.Api;
using REST_API.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace REST_API.Authentication
{
    public class AuthFailResponse : IHttpActionResult
    {
        private StatusCode statusCode;

        public AuthFailResponse(StatusCode code)
        {
            this.statusCode = code;
        }

        public async Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            HttpResponseMessage message = new HttpResponseMessage();
            message.Content = new ObjectContent<Response>(new Response() { StatusCode = this.statusCode }, new JsonMediaTypeFormatter());
            return message;
        }
    }
}