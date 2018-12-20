using REST_API.Models.Api;
using REST_API.Models.Database;
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
                List<uint> friends = friendRepository.FindAcceptedFriends(Id);

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

    }
}
