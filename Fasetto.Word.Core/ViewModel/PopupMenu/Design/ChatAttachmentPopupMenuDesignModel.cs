﻿namespace Fasetto.Word.Core
{
    /// <summary>
    /// The design-time data for a <see cref="ChatAttachmentPopupMenuViewModel"/>
    /// </summary>
    public class ChatAttachmentPopupMenuDesignModel : ChatAttachmentPopupMenuViewModel
    {
            #region Singleton

            /// <summary>
            /// A single instance of the design model
            /// </summary>
            public static ChatAttachmentPopupMenuDesignModel Instance => new ChatAttachmentPopupMenuDesignModel();

            #endregion

            #region Constructor
            /// <summary>
            /// Default cinstructor
            /// </summary>
            public ChatAttachmentPopupMenuDesignModel()
            {
                BubbleBackground = "ffffff";
            }

            #endregion
        }
}
