using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MySql.Data.MySqlClient;
using REST_API.Authentication;
using REST_API.Models.Api;
using REST_API.Models.Api.Auth;
using REST_API.Models.Database;
using REST_API.Repositories;
using REST_API.Utilities;

namespace REST_API.Controllers
{
    public class AuthController : ApiController
    {
        private DbManager dbManager;
        private UserRepository userRepository;
        private TokenRepository tokenRepository;

        public AuthController()
        {
            this.dbManager = new DbManager();
            this.userRepository = new UserRepository(this.dbManager);
            this.tokenRepository = new TokenRepository(this.dbManager);
        }

        [HttpPost]
        public Response Login([FromBody]UserCredentials user)
        {
            if (user == null)
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_REQUEST };

            if (String.IsNullOrWhiteSpace(user.Email))
                return new Response() { StatusCode = Models.Enums.StatusCode.EMPTY_EMAIL };

            if (String.IsNullOrWhiteSpace(user.Password))
                return new Response() { StatusCode = Models.Enums.StatusCode.EMPTY_PASSWORD };

            User dbUser;
            
            try
            {
                dbUser = this.userRepository.FindByEmail(user.Email);
            }
            catch (MySqlException)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
            }

            if (dbUser == null)
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_EMAIL };

            if (dbUser.Deleted)
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_EMAIL };

            if (!HashUtility.VerifyPassword(user.Password, dbUser.Password))
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_PASSWORD };

            Token token = new Token() { Id_User = dbUser.Id, Active = true, ExpireDate = DateTime.Now.AddDays(10), Value = HashUtility.GenerateNewToken() };

            try
            {
                this.tokenRepository.Create(token);
            }
            catch (MySqlException)
            {
                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
            }

            return new Response() { StatusCode = Models.Enums.StatusCode.OK, Data = token.Value };
        }

        [HttpPost]
        public Response Register([FromBody]UserCredentials user)
        {
            if (user == null)
                return new Response() { StatusCode = Models.Enums.StatusCode.INVALID_REQUEST };

            if (String.IsNullOrWhiteSpace(user.Name))
                return new Response() { StatusCode = Models.Enums.StatusCode.EMPTY_NAME };

            if (String.IsNullOrWhiteSpace(user.Email))
                return new Response() { StatusCode = Models.Enums.StatusCode.EMPTY_EMAIL };

            if (String.IsNullOrWhiteSpace(user.Password))
                return new Response() { StatusCode = Models.Enums.StatusCode.EMPTY_PASSWORD };

            User dbUser = new User() { Name = user.Name, Email = user.Email, Password = HashUtility.HashPassword(user.Password) };

            try
            {
                this.userRepository.CreateWithDefaults(dbUser);
            }
            catch (MySqlException ex)
            {
                if (ex.Message.StartsWith("Duplicate entry"))
                    return new Response() { StatusCode = Models.Enums.StatusCode.EMAIL_ALREADY_EXISTS };

                return new Response() { StatusCode = Models.Enums.StatusCode.DATABASE_ERROR };
            }

            return new Response() { StatusCode = Models.Enums.StatusCode.OK };
        }
    }
}
