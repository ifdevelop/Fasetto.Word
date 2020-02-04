namespace Fasetto.Word.Core
{
    /// <summary>
    /// The design-time data for a <see cref="ChatListItemViewModel"/>
    /// </summary>
    public class ChatListItemDesignModel : ChatListItemViewModel
    {
        #region Singleton

        /// <summary>
        /// A single instance of the design model
        /// </summary>
        public static ChatListItemDesignModel Instance => new ChatListItemDesignModel();

        #endregion

        #region Constructor
        /// <summary>
        /// Default cinstructor
        /// </summary>
        public ChatListItemDesignModel()
        {
            Name = "Luke";
            Initials = "LM";
            Message = "This app is awesome! I bet it will be fast too";
            ProfilePictureRGB = "ff0000";
        }

        #endregion
    }
}
