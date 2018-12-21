using MySql.Data.MySqlClient;
using REST_API.Authentication;
using REST_API.Models.Api;
using REST_API.Models.Api.Group;
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
    public class GroupController : ApiController
    {
        private DbManager dbManager;
        private GroupRepository groupRepository;
        private UserRepository userRepository;

        public GroupController()
        {
            this.dbManager = new DbManager();
            this.groupRepository = new GroupRepository(this.dbManager);
            this.userRepository = new UserRepository(this.dbManager);
        }

        [HttpGet]
        public Response GetAll()
        {
            List<GroupInfo> groups;

            try
            {
                groups = this.groupRepository.FindAllForUser(((UserPrincipal)User).DbUser.Id);
            }
            catch (MySqlException)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
            }

            return new Response() { StatusCode = Models.Enums.StatusCode.OK, Data = groups };
        }
    }
}
