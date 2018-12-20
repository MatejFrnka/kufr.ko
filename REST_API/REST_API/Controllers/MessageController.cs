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
using REST_API.Authentication;

namespace REST_API.Controllers
{
    [UserAuth]
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

        /// <summary>
        /// Adds a message to target group.
        /// </summary>
        /// <param name="sendMessage"><see cref="Models.Api.Message.SendMessage"/></param>
        /// <returns>Returns <see cref="Models.Enums.StatusCode"/> and Id of the message.</returns>
        [HttpPost]
        public Response SendMessage(SendMessage sendMessage)
        {
            //test
            ulong Id_User = ((UserPrincipal)User).DbUser.Id;
            ulong MessageId = repository.SendMessage(Id_User, sendMessage);
            return new Response() { StatusCode = Models.Enums.StatusCode.OK, Data = MessageId };
        }
        [HttpPost]
        public void EditMessage(EditMessage editMessage)
        {

        }
        [HttpPost]
        public void GetMessageHistory()
        {

        }
        /// <summary>
        /// Returns messages.
        /// </summary>
        /// <param name="getMessage"><see cref="Models.Api.Message.GetMessage"/></param>
        /// <returns>Returns a <see cref="Models.Enums.StatusCode"/></returns>
        [HttpPost]
        public Response GetMessages(GetMessage getMessage)
        {
            List<SingleMessage> messages = repository.GetMessages(getMessage.StartId, getMessage.Amount, getMessage.Id_Group);
            return new Response() { StatusCode = Models.Enums.StatusCode.OK, Data = messages };
        }
        [HttpPost]
        public Response SetMessageState(SetMessageState setMessageState)
        {
            ulong Id_User = ((UserPrincipal)User).DbUser.Id;
            repository.SetMessageState(Id_User, setMessageState);
            return new Response() { StatusCode = Models.Enums.StatusCode.OK };
        }
    }
}
