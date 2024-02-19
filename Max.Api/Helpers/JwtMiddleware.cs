using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Max.Services.Interfaces;
using Newtonsoft.Json;
using Max.Core.Helpers;

namespace Max.Api.Helpers
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
        {
            _next = next;
            _appSettings = appSettings.Value;
        }

        public async Task Invoke(HttpContext context, IAuthenticationService authService)
        {
            //var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            string token = null;
            var autherizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
           
            //Hack for handling  Swagger auth header
            //Check if it contains the userModel if so just get token
            if (autherizationHeader != null)
            {
                if(autherizationHeader.IndexOf("Bearer") > -1  )
                {
                    token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                }
                else
                {
                    //Swagger Authroization  Header
                    dynamic authResponse = JsonConvert.DeserializeObject<dynamic>(autherizationHeader);
                    if (authResponse.id != null)
                    {
                        token = authResponse.token;
                    }
                }
               
            }

            if (token != null)
                attachUserToContext(context, authService, token);

            await _next(context);
        }

        private  void attachUserToContext(HttpContext context, IAuthenticationService authService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // attach user to context on successful jwt validation
                var staffUser = authService.GetStaffUserById(userId);
                if (staffUser != null)
                {
                    context.Items["StafffUser"] = staffUser;
                }
                else
                {
                    var entityUser = authService.GetEntityById(userId);
                    if (entityUser != null)
                    {
                        context.Items["EntityUser"] = entityUser;
                    }
                }
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }
    }
}