using Fasetto.Word.Core;

namespace Fasetto.Word
{
    /// <summary>
    /// A view model for any popup menus
    /// </summary>
    public class BasePopupMenuViewModel : BaseViewModel
    {
        #region Public Properties

        /// <summary>
        /// The background color of the bubble in ARGB value
        /// </summary>
        public string BubbleBackground { get; set; }


        /// <summary>
        /// The aligment of the bubble arrow
        /// </summary>
        public ElementHorizontalAligment ArrowAligment { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public BasePopupMenuViewModel()
        {
            // Set default values
            //TODO: Move colors into Core and make use of it here
            BubbleBackground = "ffffff";
            ArrowAligment = ElementHorizontalAligment.Left;
        }

        #endregion

    }
}
