﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// The application state as a view model
    /// </summary>
    public class ApplicationViewModel : BaseViewModel
    {
        #region Private Members


        /// <summary>
        /// True if the settings menu should be shown
        /// </summary>
        private bool mSettingsMenuVisible;

        #endregion

        #region Public Properties

        /// <summary>
        /// The current page of the application
        /// </summary>
        public ApplicationPage CurrentPage { get; private set; } = ApplicationPage.Chat;

        /// <summary>
        /// The view model to use for the current page when the CurrentPage changes
        /// NOTE: This is not a live up-to-date view model of the current page
        ///       it is simply used to set the view model of the current page
        ///       at the time it changes
        /// </summary>
        public BaseViewModel CurrentPageViewModel { get; set; }

        /// <summary>
        /// True if the side menu should be shown
        /// </summary>
        public bool SideMenuVisible { get; set; } = false;

        /// <summary>
        /// True if the settings menu should be shown
        /// </summary>
        public bool SettingsMenuVisible
        {
            get => mSettingsMenuVisible;
            set
            {
                // If property has not changed...
                if (mSettingsMenuVisible == value)
                    // Ignore
                    return;

                // Set the backing field
                mSettingsMenuVisible = value;

                // If the settings menu is now isible...
                if (value)
                    // Reload settings
                    IoC.Task.RunAndForget(IoC.Settings.LoadAsync);
            }
        }

        #endregion
        /// <summary>
        /// Navigates to the specified page
        /// </summary>
        /// <param name="page"> The page to go to </param>
        /// <param name="viewModel"> The view model, if any, to set explicity to the new page </param>
        public void GoToPage(ApplicationPage page, BaseViewModel viewModel = null)
        {
            // Always hide settings page if we are chnging pages
            SettingsMenuVisible = false;

            // Set the view model
            CurrentPageViewModel = viewModel;

            // Set the current page
            CurrentPage = page;

            // Fire off a CurrentPage changed event
            OnPropertyChanged(nameof(CurrentPage));

            // Show side menu or not
            SideMenuVisible = page == ApplicationPage.Chat;

        }

        /// <summary>
        /// Handles what happenswhen we have successfully logged in
        /// </summary>
        /// <param name="loginResult"> The results from the successful login</param>
        /// <returns></returns>
        public async Task HandleSuccessfulLoginAsync(UserProfileDetailsApiModel loginResult)
        {
            // Store this in the client data store
            await IoC.ClientDataStore.SaveLoginCredentialsAsync(loginResult.ToLoginCredentialsDataModel());

            // Load new settings
            await IoC.Settings.LoadAsync();

            // Go to chat page
            IoC.Application.GoToPage(ApplicationPage.Chat);
        }
    }
}
