namespace Fasetto.Word.Core
{
    /// <summary>
    /// The response for all Web API cals made
    /// </summary>
    public class ApiResponse<T>
    {
        #region Public Properties

        /// <summary>
        /// Indicates if The API call was successful
        /// </summary>
        public bool Successful => ErrorMessage == null;

        /// <summary>
        /// The error message for a faied API call
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// The API response object
        /// </summary>
        public T Response { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApiResponse()
        {

        }

        #endregion
    }
}
