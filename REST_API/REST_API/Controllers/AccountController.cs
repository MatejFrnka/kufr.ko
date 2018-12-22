using REST_API.Authentication;
using REST_API.Models.Api;
using REST_API.Models.Database;
using REST_API.Models.Enums;
using REST_API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace REST_API.Controllers
{
    [UserAuth]
    public class AccountController : ApiController
    {
        UserRepository repository = new UserRepository(new Utilities.DbManager());
        public Response EditProfilePicture([FromBody]uint Id_Attachment)
        {
            try
            {
                uint Id_User = ((UserPrincipal)User).DbUser.Id;
                User user = repository.FindById(Id_User);
                user.Id_Attachment = Id_Attachment;
                repository.Update(user);
                return new Response() { StatusCode = Models.Enums.StatusCode.OK };
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw ex;
            }

        }
        public Response UpdateVisibility([FromBody]Visibility visibility)
        {
            try
            {
                uint Id_User = ((UserPrincipal)User).DbUser.Id;
                User user = repository.FindById(Id_User);
                user.Visibility = visibility;
                repository.Update(user);
                return new Response() { StatusCode = Models.Enums.StatusCode.OK };
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
                throw ex;
            }
        }
    }
}
