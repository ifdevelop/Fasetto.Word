using Dna;
using System;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Helper methods for getting and working with web requests
    /// </summary>
    public static class RouteHelpers
    {
        /// <summary>
        /// Converts a relative Url int an absolute Url
        /// </summary>
        /// <param name="relativeUrl"> The relative Url </param>
        /// <returns> Returns the absolute URL including the Host Url </returns>
        public static string GetAbsoluteRoute(string relativeUrl)
        {
            // Get the host
            var host = Framework.Construction.Configuration.GetSection("FasettoWordServer:HostUrl");

            // If they are not passing any Url...
            if (string.IsNullOrEmpty(relativeUrl))
                // Return the host
                return host.Value;

            // If the relative Url does not start with / ...
            if (!relativeUrl.StartsWith("/"))
                // Add the /
                relativeUrl = $"/{relativeUrl}";

            // Return combined Url
            return host.Value + relativeUrl;
        }
    }
}
