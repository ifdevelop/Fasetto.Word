using System.Collections.Generic;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// The design-time data for a <see cref="ChatListViewModel"/>
    /// </summary>
    public class SettingsDesignModel : SettingsViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static SettingsDesignModel Instance => new SettingsDesignModel();

        #endregion

        #region Constructor
        /// <summary>
        /// Default cinstructor
        /// </summary>
        public SettingsDesignModel()
        {
            FirstName = new TextEntryViewModel { Label = "First Name", OriginalText = "Igor" };
            LastName = new TextEntryViewModel { Label = "Last Name", OriginalText = "Feoktistov" };
            Username = new TextEntryViewModel { Label = "Username", OriginalText = "ifdev" };
            Password = new PasswordEntryViewModel { Label = "Password", FakePassword = "********" };
            Email = new TextEntryViewModel { Label = "Email", OriginalText = "if.dev402@gmail.com" };
        }

        #endregion
    }
}
