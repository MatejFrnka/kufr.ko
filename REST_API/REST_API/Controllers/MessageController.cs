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
        public Response SendMessage(SendMessage sendMessage)
        {
            //test
            int Id_User = 1;
            repository.SendMessage(Id_User, sendMessage);
            return new Response() { StatusCode = Models.Enums.StatusCode.OK };
        }
        [HttpPost]
        public void EditMessage(EditMessage editMessage)
        {

        }
        [HttpPost]
        public void GetMessageHistory()
        {
            
        }
        [HttpPost]
        public Response GetMessages(GetMessage getMessage)
        {
            List<SingleMessage> messages = repository.GetMessages(getMessage.StartId, getMessage.Amount, getMessage.Id_Group);
            return new Response() { StatusCode = Models.Enums.StatusCode.OK, Data = messages};
        }
        [HttpPost]
        public Response SetMessageState(SetMessageState setMessageState)
        {
            int Id_User = 1;
            repository.SetMessageState(Id_User, setMessageState);
            return new Response() { StatusCode = Models.Enums.StatusCode.OK };
        }
    }
}
