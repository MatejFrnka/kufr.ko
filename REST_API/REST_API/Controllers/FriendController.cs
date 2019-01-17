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
        private GroupRepository groupRepository;
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
        public Response LoadPendingToMe()
        {
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                List<UserPublic> friends = friendRepository.FindByStateToUser(userId,FriendRequestState.PENDING);
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
        public Response LoadPendingFromMe()
        {
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                List<UserPublic> friends = friendRepository.FindByStateFromUser(userId, FriendRequestState.PENDING);
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
                List<UserPublic> friends = friendRepository.FindByStateToUser(userId,FriendRequestState.BLOCKED);
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
        /// <summary>
        /// Creates new friend request between user that sends this and user specified in the parameter
        /// </summary>
        /// <param name="friend">User that you want to send friend request to</param>
        /// <returns></returns>
        [HttpPost]
        public Response CreateFriendRequest(Friend friend)
        {
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                Response response = new Response();
                bool insert = friendRepository.CreateRequest(userId, friend.User_id);
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
        /// <summary>
        /// Accepts friend request between user in parameter and user that sends this, creates their default chat group and returns its Id
        /// </summary>
        /// <param name="friend">Id of the user that you want accept friendship with</param>
        /// <returns>Response with Id of the default chat group in Data</returns>
        [HttpPatch]
        public Response AcceptFriend(Friend friend)
        {
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                Response response = new Response();
                bool insert = friendRepository.AcceptFriend(friend.User_id,userId);
                if (insert)
                {
                    response.Data = groupRepository.CreateForTwoUsersWithDefaults(friend.User_id, userId);
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
        /// <summary>
        /// Changes friedship that your (no longer) friend can't send you another friend request and your coversation with him will become read-only
        /// </summary>
        /// <param name="badFriend">Id of the friend that betrayed you</param>
        /// <returns></returns>
        [HttpPatch]
        public Response BlockFriend(Friend badFriend)
        {
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                Response response = new Response();
                bool insert = friendRepository.BlockFriend(userId, badFriend.User_id);
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

        /// <summary>
        /// Removes friendship between user who sends this and user in the param. (Deletes just the friendship, conversation and both users remain untouched.)
        /// </summary>
        /// <param name="IdFriend"></param>
        /// <returns></returns>
        [HttpDelete]
        public Response RemoveFriend(Friend friend)
        {
            userId = ((UserPrincipal)User).DbUser.Id;
            try
            {
                Response response = new Response();
                bool delete = friendRepository.DeleteFriend(userId, friend.User_id);
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
