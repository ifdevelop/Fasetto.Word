using Fasetto.Word.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fasetto.Word.Web.Server.Controllers
{
    public class AuthorizeTokenAttribute : AuthorizeAttribute
    {
        public AuthorizeTokenAttribute()
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        }
    }

    /// <summary>
    /// Manges the Web API calls
    /// </summary>
    public class ApiController : Controller
    {
        public static List<(string id, string token)> SomeAuthenticatedTokens = new List<(string id, string token)>();

        [Route("api/login")]
        public IActionResult LogIn()
        {
            // TODO: Get users login information and check it is correct

            var username = "ifdev";
            var email = "if.dev402@gmail.com";

            // Set our tokens claims
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(ClaimsIdentity.DefaultNameClaimType, username),
                new Claim(JwtRegisteredClaimNames.Jti, email),
                new Claim("my key", "my value"),

            };

            // Create the credentials used to generate the token
            var credentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(IoCContainer.Configuration["Jwt:SecretKey"])),
                SecurityAlgorithms.HmacSha256);

            // Generate the Jwt Token
            var token = new JwtSecurityToken(
                issuer: IoCContainer.Configuration["Jwt:Issuer"],
                audience: IoCContainer.Configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMonths(3),
                signingCredentials: credentials
                );

            

            // Return token to user
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        [AuthorizeToken]
        [Route("api/private")]
        public IActionResult Private()
        {
            var user = HttpContext.User;
            return Ok(new { privateData = $"some secret for {user.Identity.Name}" }); ;
        }

        /// <summary>
        /// Tries to register for a new account on the server
        /// </summary>
        /// <param name="registerCredentials">The registration details</param>
        /// <returns>Returns the result of the register request</returns>
        [Route("api/register")]
        public async Task<SendEmailResponse> RegisterAsync() //не нужно
        {
            //не нужно
            await Task.Delay(0);
            /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!! -------------asp lesson 6 1:27:20------------------- !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            // Email the user the verification code
            FasettoEmailSender.SendUserVerificationEmailAsync();



            // не нужно
            return new SendEmailResponse
            {
                Errors = new List<string>(new[] { "Unknown error occurred" })
            };
        }
    }
}