using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using REST_API.Models.Api;
using REST_API.Models.Api.Message;
using REST_API.Repositories;
using REST_API.Utilities;
using REST_API.Models.Enums;

namespace REST_API.Controllers
{
    public class MessageController : ApiController
    {
        MessageRepository repository = new MessageRepository(new DbManager());
        //[HttpGet]
        //public string test()
        //{
        //    Request r = new Request() { Id_User = 1, Token = "test"};
        //    SendMessage s = new SendMessage() { Id_Group = 1, Text = "karel" };
        //    s.Id_Attachment = new int[] { 1,3 };
        //    r.RequestType = s;
        //    string result = JsonSerializationUtility.Serialize(r);
        //    Debug.WriteLine(result);
        //    return result;
        //}
        [HttpPost]
        public Response SendMessage(Request request)
        {
            if (request.RequestType is SendMessage)
            {
                repository.SendMessage(request.Id_User, (SendMessage)request.RequestType);
                return new Response() { StatusCode = Models.Enums.StatusCode.OK };
            }
            else
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_REQUEST };
            }
        }
        [HttpPost]
        public void EditMessage(EditMessage editMessage)
        {

        }
        [HttpPost]
        public Response GetMessages(GetMessage getMessage)
        {
            return null;
        }
        [HttpPost]
        public Response SetMessageState(Request request)
        {
            request.RequestType = new SetMessageState() {Id_Message = 1, Seen = false };

            if (request.RequestType is SetMessageState)
            {
                repository.SetMessageState(request.Id_User, (SetMessageState)request.RequestType);
            }
            else
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_REQUEST };
            }
            return new Response() { StatusCode = Models.Enums.StatusCode.OK };
        }
    }
}
