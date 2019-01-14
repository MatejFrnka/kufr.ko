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
using REST_API.Models.Database;
using REST_API.Models.Api.Attachments;

namespace REST_API.Controllers
{
    [UserAuth]
    public class MessageController : ApiController
    {

        MessageRepository repository = new MessageRepository(new DbManager());
        UserRepository userRepository = new UserRepository(new DbManager());
        AttachmentRepository attachmentRepository = new AttachmentRepository(new DbManager());
        /// <summary>
        /// Adds a message to target group.
        /// </summary>
        /// <param name="sendMessage"><see cref="Models.Api.Message.SendMessage"/></param>
        /// <returns>Returns <see cref="Models.Enums.StatusCode"/> and ulong Id of the message.</returns>
        [HttpPost]
        public Response SendMessage(SendMessage sendMessage)
        {
            if (sendMessage == null || sendMessage.Id_Group == 0)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_REQUEST };
            }
            else
            {
                uint Id_User = ((UserPrincipal)User).DbUser.Id;

                if (!InGroup(Id_User, sendMessage.Id_Group))
                {
                    return new Response() { StatusCode = Models.Enums.StatusCode.FORBIDDEN };
                }
                else
                {
                    try
                    {
                        ulong MessageId = repository.SendMessage(Id_User, sendMessage);
                        return new Response() { StatusCode = Models.Enums.StatusCode.OK, Data = MessageId };
                    }
                    catch (MySql.Data.MySqlClient.MySqlException ex)
                    {
                        return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                    }
                }
            }
        }


        /// <summary>
        /// Edits target message. User can only edit its messages.
        /// </summary>
        /// <param name="editMessage"><see cref="Models.Api.Message.EditMessage"/></param>
        /// <returns>Returns a <see cref="StatusCode"/></returns>
        [HttpPost]
        public Response EditMessage(EditMessage editMessage)
        {
            uint Id_User = ((UserPrincipal)User).DbUser.Id;
            Message message = repository.FindById(editMessage.Id_Message);
            if (editMessage == null || editMessage.Id_Message == 0 || message == null)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_REQUEST };
            }
            if (message.Id_User != Id_User)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.FORBIDDEN };
            }
            try
            {
                repository.EditMessage(editMessage,Id_User);
                return new Response() { StatusCode = Models.Enums.StatusCode.OK };
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw ex;
            }

        }
        /// <summary>
        /// Returns a list of edits on a message.
        /// </summary>
        /// <param name="Id_Message">ulong Id of target message.</param>
        /// <returns>Returns a list of edits on a message and a <see cref="StatusCode"/>.</returns>
        [HttpPost]
        public Response GetMessageHistory([FromBody]ulong Id_Message)
        {
            uint Id_User = ((UserPrincipal)User).DbUser.Id;
            Message message = repository.FindById(Id_Message);
            if (Id_Message == 0)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_REQUEST };
            }
            if (!this.InGroup(Id_User,message.Id_Group))
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.FORBIDDEN };
            }
            try
            {
                List<MessageHistory> MessageHistory = repository.GetMessageHistory(Id_Message);
                return new Response() { StatusCode = Models.Enums.StatusCode.OK, Data = MessageHistory };
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw ex;
            }
        }
        /// <summary>
        /// Returns messages.
        /// </summary>
        /// <param name="getMessage"><see cref="Models.Api.Message.GetMessage"/></param>
        /// <returns>Returns a <see cref="Models.Enums.StatusCode"/></returns>
        [HttpPost]
        public Response GetMessages(GetMessage getMessage)
        {
            uint Id_User = ((UserPrincipal)User).DbUser.Id;

            if (getMessage == null || getMessage.Id_Group == 0 || getMessage.Amount == 0)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_REQUEST };
            }
            if (!this.InGroup(Id_User, getMessage.Id_Group))
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.FORBIDDEN };
            }
            try
            {
                List<SingleMessage> messages = repository.GetMessages(getMessage.StartId, getMessage.Amount, getMessage.Id_Group, Id_User);
                return new Response() { StatusCode = Models.Enums.StatusCode.OK, Data = messages };
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw ex;
            }
        }
        [HttpPost]
        public Response GetNewMessages(GetNewMessages getNewMessages)
        {
            uint Id_User = ((UserPrincipal)User).DbUser.Id;
            if (getNewMessages.Groups == null)
            {
                getNewMessages.Groups = new List<uint>();
                userRepository.GetGroups(Id_User).ForEach((a) => getNewMessages.Groups.Add(a.Id_Group));
            }
            foreach (var item in getNewMessages.Groups)
            {
                if (!InGroup(Id_User, item))
                {
                    return new Response() { StatusCode = Models.Enums.StatusCode.FORBIDDEN };

                }
            }
            List<SingleMessage> result = repository.GetNewMessages(getNewMessages.Id_Last, getNewMessages.Groups, Id_User);

            return new Response() { StatusCode = Models.Enums.StatusCode.OK, Data = result };
        }
        /// <summary>
        /// Sets a message state to recieved or seen (Seen = false -> recieved, Seen = true -> recieved and seen).
        /// </summary>
        /// <param name="setMessageState">List of <see cref="Models.Api.Message.SetMessageState"/></param>
        /// <returns>Returns a <see cref="Models.Enums.StatusCode"/></returns>
        [HttpPost]
        public Response SetMessageState(List<SetMessageState> setMessageState)
        {
            uint Id_User = ((UserPrincipal)User).DbUser.Id;
            if (setMessageState == null || setMessageState.Count == 0)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_REQUEST };
            }
            foreach (var item in setMessageState)
            {
                if(repository.FindById(item.Id_Message).Id_User != Id_User)
                    return new Response() { StatusCode = Models.Enums.StatusCode.FORBIDDEN };
            }
            foreach (var item in setMessageState)
            {
                try
                {
                    repository.SetMessageState(Id_User, item);
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                    throw ex;
                }
            }
            return new Response() { StatusCode = Models.Enums.StatusCode.OK };
        }
        [HttpGet]
        public Response LoadAttachmentInfo(ulong Id_Message, uint Id_Attachment)
        {
            uint Id_User = ((UserPrincipal)User).DbUser.Id;
            Response response = new Response();
            AttachmentMessage attachmentMessage = attachmentRepository.FindByPrimaryKeysSecure(Id_Message,Id_Attachment ,Id_User);

            if (attachmentMessage == null)
            {
                response.StatusCode = Models.Enums.StatusCode.INVALID_REQUEST;
            }
            else
            {
                response.Data = attachmentMessage;
                response.StatusCode = Models.Enums.StatusCode.OK;
            }
            return response;
        }
        /*
        /// <summary>
        /// Returns new messages.
        /// </summary>
        /// <param name="OldestMessageDate">Latest date that the message can be sent.</param>
        /// <returns>Returns a <see cref="StatusCode"/> and a List of <see cref="SingleMessage"/></returns>
        [HttpPost]
        public Response GetNewMessages([FromBody]DateTime OldestMessageDate)
        {
            uint Id_User = ((UserPrincipal)User).DbUser.Id;
            List<uint> Id_Groups = new List<uint>();
            try
            {
                Id_Groups.AddRange(userRepository.GetGroups(Id_User).Select((item) => item.Id_Group));
                List<SingleMessage> result = repository.GetNewMessages(OldestMessageDate, Id_Groups);
                return new Response() { StatusCode = Models.Enums.StatusCode.OK, Data = result };
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw ex;
            }
        }*/
        private bool InGroup(uint Id_User, uint Id_Group)
        {
            foreach (var item in userRepository.GetGroups(Id_User))
            {
                if (item.Id_Group == Id_Group)
                {
                    return true;
                }
            };
            return false;
        }
    }
}
