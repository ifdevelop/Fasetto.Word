﻿using System;
using System.Security;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// The View Model for a login screen
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The email of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// A flag indicatig if login command is runing
        /// </summary>
        public bool LoginIsRunning { get; set; }

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
        public LoginViewModel()
        {
            // Create commands
            LoginCommand = new RelayParameterizedCommand(async(parameter) => await LoginAsync(parameter));
            RegisterCommand = new RelayCommand(async () => await RegisterAsync());
        }



        #endregion

        /// <summary>
        /// Attempts to log the user in
        /// </summary>
        /// <param name="parameter"> The <see cref="SecureString"/> passed in from the view for the users password </param>
        /// <returns></returns>
        public async Task LoginAsync(object parameter)
        {
            await RunCommandAsync(() => LoginIsRunning, async () =>
            {
                // TODO: Fake a login...
                await Task.Delay(1000);

                // Ok successfully logged in... now get users data
                // TODO: Ask servernfor users info

                // TODO: Remove this with real information pulled from our database in future
                IoC.Settings.Name = new TextEntryViewModel { Label = "Name", OriginalText = $"Igor Feoktistov {DateTime.Now.ToLocalTime()}" };
                IoC.Settings.Username = new TextEntryViewModel { Label = "Username", OriginalText = "Igor" };
                IoC.Settings.Password = new PasswordEntryViewModel { Label = "Password", FakePassword = "********" };
                IoC.Settings.Email = new TextEntryViewModel { Label = "Email", OriginalText = "if.dev402@gmail.com" };

                // Go to chat page
                IoC.Application.GoToPage(ApplicationPage.Chat);

                //var email = Email;

                ////IMPORTANT: Never store unsecure password in variable like this
                //var pass = (parameter as IHavePassword).SecurePassword.Unsecure();

                //LoginIsRunning = false;
            });
            
        }

        /// <summary>
        /// Takes the user to the register page
        /// </summary>
        /// <returns></returns>
        public async Task RegisterAsync()
        {
            
            // Go to register page?
            IoC.Application.GoToPage(ApplicationPage.Register);

            await Task.Delay(1);
        }

    }
}
