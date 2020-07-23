using System;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// This view model for a password entry to edit a password
    /// </summary>
    public class PasswordEntryViewModel : BaseViewModel
    {

        #region Public Properties

        /// <summary>
        /// The label to indentify what this value for
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// The label to display fake password
        /// </summary>
        public string FakePassword { get; set; }

        /// <summary>
        /// The current password hint text
        /// </summary>
        public string CurrentPasswordHintText { get; set; }

        /// <summary>
        /// The new password hint text
        /// </summary>
        public string NewPasswordHintText { get; set; }

        /// <summary>
        /// The confirm password hint text
        /// </summary>
        public string ConfirmPasswordHintText { get; set; }

        /// <summary>
        /// The current saved password
        /// </summary>
        public SecureString CurrentPassword { get; set; }

        /// <summary>
        /// The current non-commit edited password
        /// </summary>
        public SecureString NewPassword { get; set; }

        /// <summary>
        /// The current non-commit edited confirmed password
        /// </summary>
        public SecureString ConfirmPassword { get; set; }

        /// <summary>
        /// Indicates if the current text is in edit mode
        /// </summary>
        public bool Editing { get; set; }

        /// <summary>
        /// Indicates if the current control is pending an update (in progress)
        /// </summary>
        public bool Working { get; set; }

        /// <summary>
        /// The action to run when saving the text.
        /// Returns true if the commit was successful, or false otherwise
        /// </summary>
        public Func<Task<bool>> CommitAction { get; set; }

        #endregion

        #region Public Commands

        /// <summary>
        /// Puts the control intoedit mode
        /// </summary>
        public ICommand EditCommand { get; set; }

        /// <summary>
        /// Cancels out of edit mode
        /// </summary>
        public ICommand CancelCommand { get; set; }

        /// <summary>
        /// Commits the edits and saves the value
        /// as well as goes back to non-edit mode
        /// </summary>
        public ICommand SaveCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public PasswordEntryViewModel()
        {
            // Create commands
            EditCommand = new RelayCommand(Edit);
            CancelCommand = new RelayCommand(Cancel);
            SaveCommand = new RelayCommand(Save);

            // Set defaults hints
            // TODO: Replace with localization text
            CurrentPasswordHintText = "Current Password";
            NewPasswordHintText = "New Password";
            ConfirmPasswordHintText = "Confirm Password";
        }

        #endregion

        #region Commant Methods

        /// <summary>
        /// Puts the control into edit mode
        /// </summary>
        public void Edit()
        {
            // Clear all password
            NewPassword = new SecureString();
            ConfirmPassword = new SecureString();

            // Go into edit mode
            Editing = true;
        }

        /// <summary>
        /// Cancels out of edit mode
        /// </summary>
        public void Cancel()
        {
            Editing = false;
        }

        /// <summary>
        /// Commits the content and exits out of edit mode
        /// </summary>
        public void Save()
        {
            // Store the result of a commit call
            var result = default(bool);

            RunCommandAsync(() => Working, async () =>
            {
                // While working, come out of edit mode
                Editing = false;

                // Try and do the work
                result = CommitAction == null ? true : await CommitAction();

            }).ContinueWith(t =>
            {
                // If we succeeded...
                // Nothing to do
                // If we fail...
                if (!result)
                {
                    // Go back into edit mode
                    Editing = true;
                }
            });

            //// Make sure current password is correct
            //// TODO: This will come from the real back-end store of this users password
            ////       or via asking the web server to confirm it
            //var storedPassword = "Testing";

            //// Confirm current password is a match
            //// NOTE: Typically this isn't done here, it's done on the server
            //if (storedPassword != CurrentPassword.Unsecure())
            //{
            //    // Let user know
            //    IoC.UI.ShowMessage(new MessageBoxDialogViewModel
            //    {
            //        Title = "Wrong password",
            //        Message = "The current password is invalid"
            //    });
            //}

            //// Now check that the new and confirm password match
            //if (NewPassword.Unsecure() != ConfirmPassword.Unsecure())
            //{
            //    // Let user know
            //    IoC.UI.ShowMessage(new MessageBoxDialogViewModel
            //    {
            //        Title = "Password mismatch",
            //        Message = "The new and confirm password do not match"
            //    });
            //}

            //// Check we actualy have a password
            //if (NewPassword.Unsecure().Length == 0)
            //{
            //    // Let user know
            //    IoC.UI.ShowMessage(new MessageBoxDialogViewModel
            //    {
            //        Title = "Password too short",
            //        Message = "You must enter a password!"
            //    });
            //}

            //// Set the edited password to the current value
            //CurrentPassword = new SecureString();
            //foreach (var c in NewPassword.Unsecure().ToCharArray())
            //    CurrentPassword.AppendChar(c);

            //Editing = false;
        }

        #endregion
    }
}
