using System.Windows.Input;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// The settings state as a view model
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {

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
        public TextEntryViewModel Password { get; set; }

        /// <summary>
        /// The currentusers email
        /// </summary>
        public TextEntryViewModel Email { get; set; }

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

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SettingsViewModel()
        {
            // Create commands
            OpenCommand = new RelayCommand(Open);
            CloseCommand = new RelayCommand(Close);

            // TODO: Remove this with real information pulled from our database in future
            Name = new TextEntryViewModel { Label = "Name", OriginalText = "Igor Feoktistov" };
            Username = new TextEntryViewModel { Label = "Username", OriginalText = "Igor" };
            Password = new TextEntryViewModel { Label = "Password", OriginalText = "********" };
            Email = new TextEntryViewModel { Label = "Email", OriginalText = "if.dev402@gmail.com" };
        }

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

        #endregion
    }
}
