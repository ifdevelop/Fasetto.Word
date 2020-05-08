using Fasetto.Word.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        #region Protected Members

        /// <summary>
        /// The scoped Application context
        /// </summary>
        protected ApplicationDbContext mContext;

        /// <summary>
        /// The manger for handling user creation, deletion, searching, roles etc...
        /// </summary>
        protected UserManager<ApplicationUser> mUserManager;

        /// <summary>
        /// The manger for handling signing in and out for our users
        /// </summary>
        protected SignInManager<ApplicationUser> mSignInManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="context"> The injected context </param>
        /// <param name="userManager"> The Identity sign in manager </param>
        /// <param name="signInManager"> The Identity user manager </param>
        public ApiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            mContext = context;
            mUserManager = userManager;
            mSignInManager = signInManager;
        }

        #endregion

        public static List<(string id, string token)> SomeAuthenticatedTokens = new List<(string id, string token)>();

        [Route("api/login")]
        public async Task<ApiResponse<LoginResultApiModel>> LogInAsync([FromBody]LoginCredentialsApiModel loginCredentials)
        {
            // TODO: Localize all strings
            // The message when we fail to login
            var invalidErrorMessage = "Invalid username or password";

            var errorResponse = new ApiResponse<LoginResultApiModel>
            {
                // Set error message
                ErrorMessage = invalidErrorMessage
            };

            // Make sure we have a user name
            if (loginCredentials?.UsernameOrEmail == null || string.IsNullOrWhiteSpace(loginCredentials.UsernameOrEmail))
                // Return error message
                return errorResponse;

            // Validate if the users credentials are correct

            // Is it an email?
            var isEmail = loginCredentials.UsernameOrEmail.Contains("@");

            // Get the user details
            var user = isEmail ? 
                // Find by email
                await mUserManager.FindByEmailAsync(loginCredentials.UsernameOrEmail) : 
                // Find by username
                await mUserManager.FindByNameAsync(loginCredentials.UsernameOrEmail);

            // If we failed to find a user
            if(user == null)
                // Return error message
                return errorResponse;

            // If we got here we have a user
            // Let's validate the password

            // Get if password is valid
            var isValidPassword = await mUserManager.CheckPasswordAsync(user, loginCredentials.Password);

            // If the password was wrong
            if(!isValidPassword)
                // Return error message
                return errorResponse;

            // If we get here, we are valid and the user passed the correct login details

            // Get the username
            var username = user.UserName;

            // Set our tokens claims
            var claims = new[]
            {
                // Unique ID for this token
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),

                // The username using the Identity name so it fills out HttpContext.User.Identity.Name value
                new Claim(ClaimsIdentity.DefaultNameClaimType, username),
            };


            // Create the credentials used to generate the token
            var credentials = new SigningCredentials(
                // Get the secret key from configuration
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(IoCContainer.Configuration["Jwt:SecretKey"])),
                // Use H256 algorithm
                SecurityAlgorithms.HmacSha256);

            // Generate the Jwt Token
            var token = new JwtSecurityToken(
                issuer: IoCContainer.Configuration["Jwt:Issuer"],
                audience: IoCContainer.Configuration["Jwt:Audience"],
                claims: claims,
                // Expire if not used for 3 months
                expires: DateTime.Now.AddMonths(3),
                signingCredentials: credentials
                );

            

            // Return token to user
            return new ApiResponse<LoginResultApiModel>
            {
                // Pass back the user details and the token
                Response = new LoginResultApiModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Username = user.UserName,
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                }   
            };
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
            //FasettoEmailSender.SendUserVerificationEmailAsync();



            // не нужно
            return new SendEmailResponse
            {
                Errors = new List<string>(new[] { "Unknown error occurred" })
            };
        }
    }
}