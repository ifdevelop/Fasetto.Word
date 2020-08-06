using Dna;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// The settings state as a view model
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        #region Private Members

        /// <summary>
        /// The text to show while loading
        /// </summary>
        private string mLoadingText = "...";

        #endregion

        #region Public Properties

        /// <summary>
        /// The currentusers name
        /// </summary>
        public TextEntryViewModel Name { get; set; }

        /// <summary>
        /// The currentusers username
        /// </summary>
        public TextEntryViewModel Username { get; set; }

        /// <summary>
        /// The currentusers password
        /// </summary>
        public PasswordEntryViewModel Password { get; set; }

        /// <summary>
        /// The currentusers email
        /// </summary>
        public TextEntryViewModel Email { get; set; }

        /// <summary>
        /// The text for the logout button
        /// </summary>
        public string LogoutButtonText { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to close the settings menu
        /// </summary>
        public ICommand CloseCommand { get; set; }

        /// <summary>
        /// The command to open the settings menu
        /// </summary>
        public ICommand OpenCommand { get; set; }

        /// <summary>
        /// The command to logout of the application
        /// </summary>
        public ICommand LogoutCommand { get; set; }

        /// <summary>
        /// The command to clear the users data from the view model
        /// </summary>
        public ICommand ClearUserDataCommand { get; set; }

        /// <summary>
        /// Loads the settings data from the client data store
        /// </summary>
        public ICommand LoadCommand { get; set; }

        /// <summary>
        /// Saves the current name to the server
        /// </summary>
        public ICommand SaveNameCommand { get; set; }

        /// <summary>
        /// Saves the current username to the server
        /// </summary>
        public ICommand SaveUsernameCommand { get; set; }
        /// <summary>
        /// Saves the current email to the server
        /// </summary>
        public ICommand SaveEmailCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsViewModel()
        {
            // Create Name
            Name = new TextEntryViewModel
            {
                Label = "Name",
                OriginalText = mLoadingText,
                CommitAction = SaveNameAsync
            };

            // Create Username
            Username = new TextEntryViewModel
            {
                Label = "Username",
                OriginalText = mLoadingText,
                CommitAction = SaveUsernameAsync
            };

            // Create Password
            Password = new PasswordEntryViewModel
            {
                Label = "Password",
                FakePassword = "********",
                CommitAction = SavePasswordAsync
            };

            // Create Email
            Email = new TextEntryViewModel
            {
                Label = "Email",
                OriginalText = mLoadingText,
                CommitAction = SaveEmailAsync
            };

            // Create commands
            OpenCommand = new RelayCommand(Open);
            CloseCommand = new RelayCommand(Close);
            LogoutCommand = new RelayCommand(async () => await Logout());
            ClearUserDataCommand = new RelayCommand(ClearUserData);
            LoadCommand = new RelayCommand(async () => await LoadAsync());
            SaveNameCommand = new RelayCommand(async () => await SaveNameAsync());
            SaveUsernameCommand = new RelayCommand(async () => await SaveUsernameAsync());
            SaveEmailCommand = new RelayCommand(async () => await SaveEmailAsync());

            //Name = new TextEntryViewModel { Label = "Name", OriginalText = $"Igor Feoktistov" };
            //Username = new TextEntryViewModel { Label = "Username", OriginalText = "Igor" };
            //Password = new PasswordEntryViewModel { Label = "Password", FakePassword = "********" };
            //Email = new TextEntryViewModel { Label = "Email", OriginalText = "if.dev402@gmail.com" };

            // TODO: Get from localization
            LogoutButtonText = "Logout";
        }

        #endregion

        #region Commands Methods

        /// <summary>
        /// Opens the settings menu
        /// </summary>
        public void Open()
        {
            // Close settings menu
            IoC.Application.SettingsMenuVisible = true;
        }

        /// <summary>
        /// Closes the settings menu
        /// </summary>
        public void Close()
        {
            // Close settings menu
            IoC.Application.SettingsMenuVisible = false;
        }

        /// <summary>
        /// Logs the user out
        /// </summary>
        public async Task Logout()
        {
            // TODO: Confirm the user wants to logout

            // Clear any user data/cache
            await IoC.ClientDataStore.ClearAllLoginCredentialsAsync();

            // Clean all aplication level view models that contain
            // any information about the current user
            ClearUserData();

            // Go to login page
            IoC.Application.GoToPage(ApplicationPage.Login);
        }

        /// <summary>
        /// Clears any data specific to current user
        /// </summary>
        public void ClearUserData()
        {
            // Clear all view models containing the users info
            Name.OriginalText = mLoadingText;
            Username.OriginalText = mLoadingText;
            Email.OriginalText = mLoadingText;
        }

        /// <summary>
        /// Sets the settings view model properties based on the data in the client data store
        /// </summary>
        public async Task LoadAsync()
        {
            // Update values from local cache
            await UpdateValuesFromLocalStoreAsync();

            // Get the user token
            var token = (await IoC.ClientDataStore.GetLoginCredentialsAsync()).Token;

            // If we don't have a token (so we are not logged in)...
            if (string.IsNullOrEmpty(token))
                // Then do nothing
                return;

            // Load user profile details from server
            // TODO: Move all URLs and API routes to static class in core
            var result = await WebRequests.PostAsync<ApiResponse<UserProfileDetailsApiModel>>(
                "https://localhost:5001/api/user/profile",
                bearerToken: token);

            // If it was successful...
            if(result.Successful)
            {
                // TODO: Should we check if the values are different before saving
                await Task.Delay(2000);

                // Create data model from the response
                var dataModel = result.ServerResponse.ResponseGeneric.ToLoginCredentialsDataModel();

                // Re-add our known token
                dataModel.Token = token;

                // Store this in the client data store
                await IoC.ClientDataStore.SaveLoginCredentialsAsync(dataModel);

                // Update values from local cache
                await UpdateValuesFromLocalStoreAsync();
            }
        }

        /// <summary>
        /// Saves the Name to the server
        /// </summary>
        /// <param name="self"> The details of the view model </param>
        /// <returns> Returns true if successful, false otherwise </returns>
        public async Task<bool> SaveNameAsync()
        {
            // TODO: Update with server
            await Task.Delay(3000);

            // Return success
            return true;
        }

        /// <summary>
        /// Saves the Username to the server
        /// </summary>
        /// <param name="self"> The details of the view model </param>
        /// <returns> Returns true if successful, false otherwise </returns>
        public async Task<bool> SaveUsernameAsync()
        {
            // TODO: Update with server
            await Task.Delay(3000);

            // Return success
            return true;
        }

        /// <summary>
        /// Saves the Email to the server
        /// </summary>
        /// <param name="self"> The details of the view model </param>
        /// <returns> Returns true if successful, false otherwise </returns>
        public async Task<bool> SaveEmailAsync()
        {
            // TODO: Update with server
            await Task.Delay(3000);

            // Return success
            return true;
        }

        /// <summary>
        /// Saves the Password to the server
        /// </summary>
        /// <param name="self"> The details of the view model </param>
        /// <returns> Returns true if successful, false otherwise </returns>
        public async Task<bool> SavePasswordAsync()
        {
            // TODO: Update with server
            await Task.Delay(3000);

            // Return fail
            return false;
        } 

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Loads the settings from the local data store and binds them
        /// to this view model
        /// </summary>
        /// <returns></returns>
        private async Task UpdateValuesFromLocalStoreAsync()
        {
            //  Get the stored credentials
            var storedCredentials = await IoC.ClientDataStore.GetLoginCredentialsAsync();

            //Name = new TextEntryViewModel { Label = "Name", OriginalText = $"Igor Feoktistov" };
            //Username = new TextEntryViewModel { Label = "Username", OriginalText = "Igor" };
            //Password = new PasswordEntryViewModel { Label = "Password", FakePassword = "********" };
            //Email = new TextEntryViewModel { Label = "Email", OriginalText = "if.dev402@gmail.com" };

            // Set name
            Name.OriginalText = $"{storedCredentials?.FirstName ?? ""} {storedCredentials?.LastName ?? ""}";

            // Set username
            Username.OriginalText = storedCredentials?.Username;

            // Set email
            Email.OriginalText = storedCredentials?.Email;
        }

        #endregion
    }
}
