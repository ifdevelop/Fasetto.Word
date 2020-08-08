using Fasetto.Word.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
    [AuthorizeToken]
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

        #region Login / register / Verify

        /// <summary>
        /// Try to register for a new account on the server
        /// </summary>
        /// <param name="registerCredentials"> The registrations details </param>
        /// <returns> Returns the result of the register request </returns>
        [AllowAnonymous]
        [Route(ApiRoutes.Register)]
        public async Task<ApiResponse<RegisterResultApiModel>> RegisterAsync([FromBody]RegisterCredentialsApiModel registerCredentials)
        {
            // TODO: Localize all strings
            // The message when we fail to login
            var invalidErrorMessage = "Please provide all required details to register for an account";

            var errorResponse = new ApiResponse<RegisterResultApiModel>
            {
                // Set error message
                ErrorMessage = invalidErrorMessage
            };

            // If we have no credentials...
            if (registerCredentials == null)
                // Return failed response
                return errorResponse;

            // Make sure we have a user name
            if (string.IsNullOrWhiteSpace(registerCredentials.Username))
                // Return error message to user
                return errorResponse;

            // Create the desired user from the given details
            var user = new ApplicationUser
            {
                UserName = registerCredentials.Username,
                FirstName = registerCredentials.FirstName,
                LastName = registerCredentials.LastName,
                Email = registerCredentials.Email,
            };

            // Try and create a user
            var result = await mUserManager.CreateAsync(user, registerCredentials.Password);

            // If the registration was successful...
            if (result.Succeeded)
            {
                // Get the user details
                var userIdentity = await mUserManager.FindByNameAsync(registerCredentials.Username);

                //// Generate an email verification code
                //var emailVerificationCode = await mUserManager.GenerateEmailConfirmationTokenAsync(user);

                //var confirmationUrl = $"https://{Request.Host.Value}/api/verify/email/{HttpUtility.UrlEncode(userIdentity.Id)}/{HttpUtility.UrlEncode(emailVerificationCode)}";

                // Email the user the verifiction code
                //await FasettoEmailSender.SendUserVerificationEmailAsync(null, userIdentity.Email, confirmationUrl);

                // Email the user the verifiction code
                await SendUserEmailVerificationAsync(user);
                //await FasettoEmailSender.SendUserVerificationEmailAsync(user.UserName, userIdentity.Email, confirmationUrl);

                // return valid response containing all users details
                return new ApiResponse<RegisterResultApiModel>
                {
                    ResponseGeneric = new RegisterResultApiModel
                    {
                        FirstName = userIdentity.FirstName,
                        LastName = userIdentity.LastName,
                        Email = userIdentity.Email,
                        Username = userIdentity.UserName,
                        Token = userIdentity.GenerateJwtToken()
                    }
                };


            }
            // Otherwise if it failed...
            else
                // Return the failed response
                return new ApiResponse<RegisterResultApiModel>
                {
                    // Aggregate all errors into a single error string
                    ErrorMessage = result.Errors.AggregateErrors()
                };

        }

        /// <summary>
        /// Logs in a user using token-based authentication
        /// </summary>
        /// <param name="loginCredentials"> The login details </param>
        /// <returns> Returns the result of the register request </returns>
        [AllowAnonymous]
        [Route(ApiRoutes.Login)]
        public async Task<ApiResponse<UserProfileDetailsApiModel>> LogInAsync([FromBody]LoginCredentialsApiModel loginCredentials)
        {
            // TODO: Localize all strings
            // The message when we fail to login
            var invalidErrorMessage = "Invalid username or password";

            var errorResponse = new ApiResponse<UserProfileDetailsApiModel>
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
            if (user == null)
                // Return error message
                return errorResponse;

            // If we got here we have a user
            // Let's validate the password

            // Get if password is valid
            var isValidPassword = await mUserManager.CheckPasswordAsync(user, loginCredentials.Password);

            // If the password was wrong
            if (!isValidPassword)
                // Return error message
                return errorResponse;

            // If we get here, we are valid and the user passed the correct login details

            // Get the username
            var username = user.UserName;

            // Return token to user
            return new ApiResponse<UserProfileDetailsApiModel>
            {
                // Pass back the user details and the token
                ResponseGeneric = new UserProfileDetailsApiModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Username = user.UserName,
                    Token = user.GenerateJwtToken()
                }
            };
        }

        [AllowAnonymous]
        [Route(ApiRoutes.VerifyEmail)]
        [HttpGet]
        public async Task<ActionResult> VerifyEmailAsync(string userId, string emailToken)
        {
            // Get the user
            var user = await mUserManager.FindByIdAsync(userId);

            // NOTE: Issue with Url Decoding that contains /'s does not replace them
            //       For now, manually fix that
            emailToken = emailToken.Replace("%2f", "/").Replace("%2F", "/");

            // If the user is null
            if (user == null)
                // TODO: Nice UI
                return Content("User not found");

            // If we have the user

            // Verify the email token
            var result = await mUserManager.ConfirmEmailAsync(user, emailToken);

            // If succeeded...
            if (result.Succeeded)
                // TODO: Nice UI
                return Content("Email verified :)");

            // TODO: Nice UI
            return Content("Invalid Email Verification Token :(");
        }

        #endregion

        /// <summary>
        /// Returns the user profile details based on the authenticated user
        /// </summary>
        /// <returns></returns>
        [Route(ApiRoutes.GetUserProfile)]
        public async Task<ApiResponse<UserProfileDetailsApiModel>> GetUserProfileAsync()
        {
            // Get user claims
            var user = await mUserManager.GetUserAsync(HttpContext.User);

            // If we have no user...
            if (user == null)
                // Return error
                return new ApiResponse<UserProfileDetailsApiModel>()
                {
                    // TODO: Localization
                    ErrorMessage = "User not found"
                };

            // Return token to user
            return new ApiResponse<UserProfileDetailsApiModel>
            {
                // Pass back the user details and the token
                ResponseGeneric = new UserProfileDetailsApiModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Username = user.UserName,
                }
            };
        }

        /// <summary>
        /// Attempts to update the users profile details
        /// </summary>
        /// <param name="model"> The user profile details to update </param>
        /// <returns>
        /// Returns successful respose if the update was successful,
        /// otherwise returns the error reasons for the failure
        /// </returns>
        [Route(ApiRoutes.UpdateUserProfile)]
        public async Task<ApiResponse> UpdateUserProfileAsync([FromBody]UpdateUserProfileApiModel model)
        {
            // Make a list of empty errors
            var errors = new List<string>();

            // Keep track of email change
            var emailChanged = false;

            // Get the current user
            var user = await mUserManager.GetUserAsync(HttpContext.User);

            // If we have no user...
            if (user == null)
                return new ApiResponse
                {
                    // TODO: Localization
                    ErrorMessage = "User not found"
                };

            // If we have a first name...
            if (model.FirstName != null)
                // Update profile details
                user.FirstName = model.FirstName;

            // If we have a last name...
            if (model.LastName != null)
                // Update profile details
                user.LastName = model.LastName;

            // If we have a email...
            if (model.Email != null &&
                // And it is not the same...
                !string.Equals(model.Email.Replace(" ", ""), user.NormalizedEmail))
            {
                // Update profile details
                user.Email = model.Email;

                // Unverify the email
                user.EmailConfirmed = false;

                // Flag we have changed email
                emailChanged = true;
            }

            // If we have a username...
            if (model.Username != null)
                // Update profile details
                user.UserName = model.Username;

            // Attempt to commit changes to data store
            var result = await mUserManager.UpdateAsync(user);

            // If successful and if email was changed, send out email verification
            if (result.Succeeded && emailChanged)
                // Email the user the verifiction code
                await SendUserEmailVerificationAsync(user);

            // If we were successful
            if (result.Succeeded)
                // Return succesful response
                return new ApiResponse();
            // Otherwise if it failed
            else
                // Return the failed response
                return new ApiResponse
                {
                    ErrorMessage = result.Errors.AggregateErrors()
                };
        }

        /// <summary>
        /// Attempts to update the users password
        /// </summary>
        /// <param name="model"> The user password details to update </param>
        /// <returns>
        /// Returns successful respose if the update was successful,
        /// otherwise returns the error reasons for the failure
        /// </returns>
        [Route(ApiRoutes.UpdateUserPassword)]
        public async Task<ApiResponse> UpdateUserPasswordAsync([FromBody]UpdateUserPasswordApiModel model)
        {
            // Make a list of empty errors
            var errors = new List<string>();

            // Get the current user
            var user = await mUserManager.GetUserAsync(HttpContext.User);

            // If we have no user...
            if (user == null)
                return new ApiResponse
                {
                    // TODO: Localization
                    ErrorMessage = "User not found"
                };

            // Attempt to commit changes to data store
            var result = await mUserManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            // If we were successful
            if (result.Succeeded)
                // Return succesful response
                return new ApiResponse();
            // Otherwise if it failed
            else
                // Return the failed response
                return new ApiResponse
                {
                    ErrorMessage = result.Errors.AggregateErrors()
                };
        }

        /// <summary>
        /// Sends the given user a new verify email link
        /// </summary>
        /// <param name="user"> The user to send the link to </param>
        /// <returns></returns>
        private async Task SendUserEmailVerificationAsync(ApplicationUser user)
        {
            // Get the user details
            var userIdentity = await mUserManager.FindByNameAsync(user.UserName);

            // Generate an email verification code
            var emailVerificationCode = await mUserManager.GenerateEmailConfirmationTokenAsync(user);

            // TODO: Replace with APIRoutes that will contain the static routes to use
            var confirmationUrl = $"http://{Request.Host.Value}/api/verify/email/{userIdentity.Id}/{emailVerificationCode}";

            // Email the user the verification code
            await FasettoEmailSender.SendUserVerificationEmailAsync(user.UserName, userIdentity.Email, confirmationUrl);

        }
    }
}