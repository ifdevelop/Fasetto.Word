using Dna;
using System;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// The View Model for a register screen
    /// </summary>
    public class RegisterViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The username of the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// The email of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// A flag indicatig if register command is runing
        /// </summary>
        public bool RegisterIsRunning { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// The command to login
        /// </summary>
        public ICommand LoginCommand { get; set; }

        /// <summary>
        /// The command to register
        /// </summary>
        public ICommand RegisterCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="window"></param>
        public RegisterViewModel()
        {
            // Create commands
            RegisterCommand = new RelayParameterizedCommand(async(parameter) => await RegisterAsync(parameter));
            LoginCommand = new RelayCommand(async () => await LoginAsync());
        }



        #endregion

        /// <summary>
        /// Attempts to register a user 
        /// </summary>
        /// <param name="parameter"> The <see cref="SecureString"/> passed in from the view for the users password </param>
        /// <returns></returns>
        public async Task RegisterAsync(object parameter)
        {
            await RunCommandAsync(() => RegisterIsRunning, async () =>
            {
                // Call the server and attempt to register with the provided credentials
                var result = await WebRequests.PostAsync<ApiResponse<RegisterResultApiModel>>(
                    // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.Register),
                    // Create API model
                    new RegisterCredentialsApiModel
                    {
                        Username = Username,
                        Email = Email,
                        Password = (parameter as IHavePassword).SecurePassword.Unsecure()
                    });

                // If the response has an error...
                if (await result.DisplayErrorIfFailedAsync("Register Failed"))
                    // We are done
                    return;

                // Ok successfully registered (and logged in)... now get users data
                var loginResult = result.ServerResponse.ResponseGeneric;

                // Let the application view model handle what happens
                // with the successful login
                await IoC.Application.HandleSuccessfulLoginAsync(loginResult);
            });
            
        }

        /// <summary>
        /// Takes the user to the login page
        /// </summary>
        /// <returns></returns>
        public async Task LoginAsync()
        {
            
            // Go to register page?
            IoC.Application.GoToPage(ApplicationPage.Login);

            await Task.Delay(1);
        }

    }
}
