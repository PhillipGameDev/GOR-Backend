using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using GameOfRevenge.Business;
using GameOfRevenge.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace GameOfRevenge.WebServer.Services
{
    public static class AuthHelper
    {
        public const string PlayerId = "PlayerId";
        public const string UserData = "UserData";
        public const string Remember = "Remember";
        public const string AuthenticationSchemeCookie = "ChurchMainCookieScheme";
        public const string AuthenticationSchemeJwt = "ChurchMainJwtScheme";

        public static List<Claim> GetClaims(this Player user) => user.GetClaims(true);
        public static List<Claim> GetClaims(this Player user, bool remember)
        {
            var userdata = new List<Claim>()
            {
                new Claim(PlayerId, user.PlayerId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(UserData, JsonConvert.SerializeObject(user)),
                new Claim(Remember, remember.ToString())
            };

            return userdata;
        }

        public static T GetValue<T>(this ClaimsPrincipal claims, string name)
        {
            var dataString = claims.GetValue(name);
            return JsonConvert.DeserializeObject<T>(dataString);
        }
        public static string GetValue(this ClaimsPrincipal claims, string name)
        {
            if (claims != null && claims.HasClaim(c => c.Type == name)) return claims.Claims.FirstOrDefault(c => c.Type == name).Value;
            else return string.Empty;
        }

        public static Player GetUser(this ClaimsPrincipal claims) => claims.GetValue<Player>(UserData);
        public static Player GetUser(this HttpContext context)
        {
            if (context != null) return GetUser(context.User);
            else return default;
        }
        public static Player GetUser(this IHttpContextAccessor context)
        {
            if (context != null) return GetUser(context.HttpContext);
            else return default;
        }
        public static bool RememberMe(this ClaimsPrincipal claims) => claims.GetValue(Remember) == "true";
        public static bool RememberMe(this HttpContext context)
        {
            if (context != null) return RememberMe(context.User);
            else return default;
        }
        public static bool RememberMe(this IHttpContextAccessor context)
        {
            if (context != null) return RememberMe(context.HttpContext);
            else return default;
        }

        public static void GenerateToken(this Player user)
        {
            if (user != null)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(Config.Secret);
                    var claims = user.GetClaims().Where(x => x.Type != Remember);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);
                    user.Token = tokenString;
                }
                catch (Exception)
                {
                    user.Token = string.Empty;
                }
            }
        }
    }
}
