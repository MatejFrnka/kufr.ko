using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using REST_API.Models.Api;
using REST_API.Models.Api.Message;

namespace REST_API.Controllers
{
    public class MessageController : ApiController
    {
        [HttpPost]
        public void SendMessage(SendMessage sendMessage)
        {

        }
        [HttpPost]
        public void EditMessage(EditMessage editMessage)
        {

        }
        [HttpPost]
        public List<SingleMessage> GetMessages(GetMessage getMessage)
        {
            return null;
        }
        [HttpPost]
        public void SetMessageState(SetMessageState messageState)
        {

        }
    }
}
