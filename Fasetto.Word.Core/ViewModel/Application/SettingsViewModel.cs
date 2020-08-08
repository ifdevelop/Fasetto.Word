using Dna;
using System;
using System.Linq.Expressions;
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
        /// The currentusers first name
        /// </summary>
        public TextEntryViewModel FirstName { get; set; }

        /// <summary>
        /// The currentusers last name
        /// </summary>
        public TextEntryViewModel LastName { get; set; }

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

        #region Transactional Properties

        /// <summary>
        /// Indicates if the first name is current being saved
        /// </summary>
        public bool FirstNameIsSaving { get; set; }

        /// <summary>
        /// Indicates if the last name is current being saved
        /// </summary>
        public bool LastNameIsSaving { get; set; }

        /// <summary>
        /// Indicates if the username is current being saved
        /// </summary>
        public bool UsernameIsSaving { get; set; }

        /// <summary>
        /// Indicates if the email is current being saved
        /// </summary>
        public bool EmailIsSaving { get; set; }

        /// <summary>
        /// Indicates if the password is current being changed
        /// </summary>
        public bool PasswordIsChanging { get; set; }

        #endregion

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
        /// Saves the current first name to the server
        /// </summary>
        public ICommand SaveFirstNameCommand { get; set; }

        /// <summary>
        /// Saves the current last name to the server
        /// </summary>
        public ICommand SaveLastNameCommand { get; set; }

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
            // Create FirstName
            FirstName = new TextEntryViewModel
            {
                Label = "First Name",
                OriginalText = mLoadingText,
                CommitAction = SaveFirstNameAsync
            };

            // Create LastName
            LastName = new TextEntryViewModel
            {
                Label = "Last Name",
                OriginalText = mLoadingText,
                CommitAction = SaveLastNameAsync
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
            SaveFirstNameCommand = new RelayCommand(async () => await SaveFirstNameAsync());
            SaveLastNameCommand = new RelayCommand(async () => await SaveLastNameAsync());
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
            FirstName.OriginalText = mLoadingText;
            LastName.OriginalText = mLoadingText;
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
            var result = await WebRequests.PostAsync<ApiResponse<UserProfileDetailsApiModel>>(
                // Set URL
                RouteHelpers.GetAbsoluteRoute(ApiRoutes.GetUserProfile),
                // Pass in user Token
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
        /// Saves the FirstName to the server
        /// </summary>
        /// <param name="self"> The details of the view model </param>
        /// <returns> Returns true if successful, false otherwise </returns>
        public async Task<bool> SaveFirstNameAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => FirstNameIsSaving, async () =>
            {
                // Update the First Name value on the server...
                return await UpdateUserCredentialsValueAsync(
                    // Display name
                    "First Name",
                    // Update the first name
                    (credentials) => credentials.FirstName,
                    // To new value
                    FirstName.OriginalText,
                    // Set api model value
                    (apiModel, value) => apiModel.FirstName = value
                    ); 
            });
        }

        /// <summary>
        /// Saves the LastName to the server
        /// </summary>
        /// <param name="self"> The details of the view model </param>
        /// <returns> Returns true if successful, false otherwise </returns>
        public async Task<bool> SaveLastNameAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => LastNameIsSaving, async () =>
            {
                // Update the Last Name value on the server...
                return await UpdateUserCredentialsValueAsync(
                    // Display name
                    "Last Name",
                    // Update the last name
                    (credentials) => credentials.LastName,
                    // To new value
                    LastName.OriginalText,
                    // Set api model value
                    (apiModel, value) => apiModel.LastName = value
                    );
            });
        }

        /// <summary>
        /// Saves the Username to the server
        /// </summary>
        /// <param name="self"> The details of the view model </param>
        /// <returns> Returns true if successful, false otherwise </returns>
        public async Task<bool> SaveUsernameAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => UsernameIsSaving, async () =>
            {
                // Update the Username value on the server...
                return await UpdateUserCredentialsValueAsync(
                    // Display name
                    "Username",
                    // Update the username
                    (credentials) => credentials.Username,
                    // To new value
                    Username.OriginalText,
                    // Set api model value
                    (apiModel, value) => apiModel.Username = value
                    );
            });
        }

        /// <summary>
        /// Saves the Email to the server
        /// </summary>
        /// <param name="self"> The details of the view model </param>
        /// <returns> Returns true if successful, false otherwise </returns>
        public async Task<bool> SaveEmailAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => EmailIsSaving, async () =>
            {
                // Update the email value on the server...
                return await UpdateUserCredentialsValueAsync(
                    // Display name
                    "Email",
                    // Update the email
                    (credentials) => credentials.Email,
                    // To new value
                    Email.OriginalText,
                    // Set api model value
                    (apiModel, value) => apiModel.Email = value
                    );
            });
        }

        /// <summary>
        /// Saves the Password to the server
        /// </summary>
        /// <param name="self"> The details of the view model </param>
        /// <returns> Returns true if successful, false otherwise </returns>
        public async Task<bool> SavePasswordAsync()
        {
            // Lock this command to ignore any other requests while processing
            return await RunCommandAsync(() => PasswordIsChanging, async () =>
            {
                // Log it
                IoC.Logger.Log($"Changing password...", LogLevel.Debug);

                // Get the current known credentials
                var credentials = await IoC.ClientDataStore.GetLoginCredentialsAsync();

                // Make sure the user has entered the same password
                if(Password.NewPassword.Unsecure() != Password.ConfirmPassword.Unsecure())
                {
                    // Display error
                    await IoC.UI.ShowMessage(new MessageBoxDialogViewModel
                    {
                        // TODO: Localize
                        Title = "Password mismatch",
                        Message = "New password and confirm password must match"
                    });

                    // Return fail
                    return false;
                }

                // Update the server with the new password
                var result = await WebRequests.PostAsync<ApiResponse>(
                    // Set URL
                    RouteHelpers.GetAbsoluteRoute(ApiRoutes.UpdateUserPassword),
                    // Create API model
                    new UpdateUserPasswordApiModel
                    {
                        CurrentPassword = Password.CurrentPassword.Unsecure(),
                        NewPassword = Password.NewPassword.Unsecure()
                    },
                    // Pass in user token
                    bearerToken: credentials.Token);

                // If the response has an error...
                if (await result.DisplayErrorIfFailedAsync($"Change Password"))
                {
                    // Log it
                    IoC.Logger.Log($"Failed to change password. {result.ErrorMessage}", LogLevel.Debug);

                    return false;
                }

                // Log it
                IoC.Logger.Log($"Successfully changed password", LogLevel.Debug);

                // Return successful
                return true;
            });
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

            // Set first name
            FirstName.OriginalText = storedCredentials?.FirstName;

            // Set last name
            LastName.OriginalText = storedCredentials?.LastName;

            // Set username
            Username.OriginalText = storedCredentials?.Username;

            // Set email
            Email.OriginalText = storedCredentials?.Email;
        }

        /// <summary>
        /// Updates a specific value from the client data store for the user profile details
        /// and attempts to update the server tomatch those details.
        /// for example, updating the first name of the user.
        /// </summary>
        /// <param name="displayName"> The display name for logging and display purposes of the property we are updating </param>
        /// <param name="propertyToUpdate"> The property from the <see cref="LoginCredentialsDataModel" to be updated /></param>
        /// <param name="newValue"> The new value to update the property to </param>
        /// <param name="setApiModel"> setes the correct property in the <see cref="UpdateUserProfileApiModel"/> that this property maps to </param>
        /// <returns></returns>
        private async Task<bool> UpdateUserCredentialsValueAsync(string displayName, Expression<Func<LoginCredentialsDataModel, string>> propertyToUpdate, string newValue, Action<UpdateUserProfileApiModel, string> setApiModel)
        {
            // Log it
            IoC.Logger.Log($"Saving {displayName}...", LogLevel.Debug);

            // Get the current known credentials
            var credentials = await IoC.ClientDataStore.GetLoginCredentialsAsync();

            // Get the property to update from the credentials
            var toUpdate = propertyToUpdate.GetPropertyValue(credentials);

            // Log it
            IoC.Logger.Log($"{displayName} currently {toUpdate}, updating to {newValue}", LogLevel.Debug);

            // Check if the value is the same...
            if (toUpdate == newValue)
            {
                // Log it
                IoC.Logger.Log($"{displayName} is the same, ignoring...", LogLevel.Debug);

                return true;
            }

            // Set the property
            propertyToUpdate.SetPropertyValue(newValue, credentials);

            // Create update details
            var updateApiModel = new UpdateUserProfileApiModel();

            // Ask caller to set appropriate value
            setApiModel(updateApiModel, newValue);

            // Update the server awith the details
            var result = await WebRequests.PostAsync<ApiResponse>(
                // Set URL
                RouteHelpers.GetAbsoluteRoute(ApiRoutes.UpdateUserProfile),
                // Pass the Api model
                updateApiModel,
                // Pass in user token
                bearerToken: credentials.Token);

            // If the response has an error...
            if (await result.DisplayErrorIfFailedAsync($"Update {displayName}"))
            {
                // Log it
                IoC.Logger.Log($"Failed to update {displayName}. {result.ErrorMessage}", LogLevel.Debug);

                return false;
            }

            // Log it
            IoC.Logger.Log($"Successfully updated {displayName}. Saving to local database cache...", LogLevel.Debug);

            // Store the new user credentials to the data store
            await IoC.ClientDataStore.SaveLoginCredentialsAsync(credentials);

            // Return successful
            return true;
        }

        #endregion
    }
}
