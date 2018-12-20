using REST_API.Models.Api;
using REST_API.Models.Database;
using REST_API.Repositories;
using REST_API.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace REST_API.Authentication
{
    public class UserAuthAttribute : System.Web.Mvc.ActionFilterAttribute, IAuthenticationFilter
    {
        public async Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
        {
            string token = null;
            IEnumerable<string> tokenHeader;
            if (context.Request.Headers.TryGetValues("Token", out tokenHeader))
                token = tokenHeader.FirstOrDefault();

            if (token != null)
            {
                DbManager dbManager = new DbManager();
                TokenRepository tokenRepository = new TokenRepository(dbManager);
                

                Token dbToken = tokenRepository.FindByValue(token);

                if(dbToken != null)
                {
                    if(dbToken.Active)
                    {
                        if(dbToken.ExpireDate >= DateTime.Now)
                        {
                            UserRepository userRepository = new UserRepository(dbManager);

                            User dbUser = userRepository.FindById(dbToken.Id_User);

                            if(dbUser != null)
                            {
                                userRepository.UpdateLastOnline(dbUser.Id);
                                context.Principal = new UserPrincipal(dbUser);
                            }else
                                context.ErrorResult = new AuthFailResponse(Models.Enums.StatusCode.TOKEN_INVALID);
                        }
                        else
                            context.ErrorResult = new AuthFailResponse(Models.Enums.StatusCode.TOKEN_EXPIRED);
                    }
                    else
                        context.ErrorResult = new AuthFailResponse(Models.Enums.StatusCode.TOKEN_INACTIVE);
                    
                }else
                    context.ErrorResult = new AuthFailResponse(Models.Enums.StatusCode.TOKEN_INVALID);

            }
            else
                context.ErrorResult = new AuthFailResponse(Models.Enums.StatusCode.INVALID_REQUEST);
        }

        public async Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
        {

        }
    }
}