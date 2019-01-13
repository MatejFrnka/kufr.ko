using REST_API.Authentication;
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
    [UserAuth]
    public class FriendController : ApiController
    {
        private DbManager dbManager;
        private UserRepository userRepository;
        private FriendRepository friendRepository;
        private uint userId;
        
        public FriendController()
        {
            this.dbManager = new DbManager();
            this.userRepository = new UserRepository(this.dbManager);
            this.friendRepository = new FriendRepository(this.dbManager);
        }
        [HttpGet]
        public Response LoadExistingFriends()
        {
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                List<UserPublic> friends = friendRepository.FindAcceptedFriends(userId);
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
        public Response SearchNewFriends(string fulltext)
        {
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                List<UserPublic> friends = friendRepository.SearchPossibleFriends(userId,fulltext);
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
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                List<UserPublic> friends = friendRepository.FindByState(userId,FriendRequestState.PENDING);
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
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                List<UserPublic> friends = friendRepository.FindByState(userId,FriendRequestState.BLOCKED);
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
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                Response response = new Response();
                bool insert = friendRepository.CreateRequest(userId, IdReceiver);
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
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                Response response = new Response();
                bool insert = friendRepository.RespondToRequest(userId,IdReceiver, friendStatus);
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
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                Response response = new Response();
                bool delete = friendRepository.DeleteFriend(userId, IdFriend);
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
