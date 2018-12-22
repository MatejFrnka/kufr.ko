using REST_API.Models.Api;
using REST_API.Models.Database;
using REST_API.Models.Enums;
using REST_API.Repositories;
using REST_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace REST_API.Controllers
{
    public class FriendController : ApiController
    {
        private DbManager dbManager;
        private UserRepository userRepository;
        private FriendRepository friendRepository;

        public FriendController()
        {
            this.dbManager = new DbManager();
            this.userRepository = new UserRepository(this.dbManager);
            this.friendRepository = new FriendRepository(this.dbManager);
        }
        [HttpGet]
        public Response LoadExistingFriends()
        {
            uint Id = 1;
            try
            {
                List<UserPublic> friends = friendRepository.FindAcceptedFriends(Id);
                FileController.LoadAttachements(friends);
                Response response = new Response();
                response.StatusCode = Models.Enums.StatusCode.OK;
                response.Data = friends;
                return response;
            }
            catch (Exception)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw;
            }
        }
        [HttpGet]
        public Response LoadPending()
        {
            uint Id = 1;
            try
            {
                List<UserPublic> friends = friendRepository.FindByState(Id,FriendRequestState.PENDING);
                FileController.LoadAttachements(friends);
                Response response = new Response();
                response.StatusCode = Models.Enums.StatusCode.OK;
                response.Data = friends;
                return response;
            }
            catch (Exception)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw;
            }
        }
        [HttpGet]
        public Response LoadBlocked()
        {
            uint Id = 1;
            try
            {
                List<UserPublic> friends = friendRepository.FindByState(Id,FriendRequestState.BLOCKED);
                FileController.LoadAttachements(friends);
                Response response = new Response();
                response.StatusCode = Models.Enums.StatusCode.OK;
                response.Data = friends;
                return response;
            }
            catch (Exception)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw;
            }
        }
        [HttpPost]
        public Response CreateFriendRequest(uint IdReceiver)
        {
            uint Id = 1;

            try
            {
                Response response = new Response();
                bool insert = friendRepository.CreateRequest(Id, IdReceiver);
                if (insert)
                {
                    response.StatusCode = Models.Enums.StatusCode.OK;
                }
                else
                {
                    response.StatusCode = Models.Enums.StatusCode.INVALID_REQUEST;
                }

                
                return response;
            }
            catch (Exception)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw;
            }
        }
        [HttpPatch]
        public Response ChangeFriendStatus(uint IdReceiver,FriendRequestState friendStatus)
        {
            uint Id = 1;

            try
            {
                Response response = new Response();
                bool insert = friendRepository.RespondToRequest(Id,IdReceiver, friendStatus);
                if (insert)
                {
                    response.StatusCode = Models.Enums.StatusCode.OK;
                }
                else
                {
                    response.StatusCode = Models.Enums.StatusCode.INVALID_REQUEST;
                }


                return response;
            }
            catch (Exception)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw;
            }
        }
        [HttpDelete]
        public Response RemoveFriend(uint IdFriend)
        {
            uint Id = 1;

            try
            {
                Response response = new Response();
                bool delete = friendRepository.DeleteFriend(Id, IdFriend);
                if (delete)
                {
                    response.StatusCode = Models.Enums.StatusCode.OK;
                }
                else
                {
                    response.StatusCode = Models.Enums.StatusCode.INVALID_REQUEST;
                }


                return response;
            }
            catch (Exception)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw;
            }
        }
    }
}
