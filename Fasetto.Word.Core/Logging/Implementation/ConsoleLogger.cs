using System;

namespace Fasetto.Word.Core
{
    /// <summary>
    /// Logs the given message to the system Console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Logs the given message to the system console
        /// </summary>
        /// <param name="message"> The message to log </param>
        /// <param name="level"> The level of the message </param>
        public void Log(string message, LogLevel level)
        {
            // Save old color
            var consoleOldColor = Console.ForegroundColor;

            // Default log color value
            var consoleColor = ConsoleColor.White;

            // Color console based on level
            switch(level)
            {// Debug is blue
                case LogLevel.Debug:
                    consoleColor = ConsoleColor.Blue;
                    break;
                // Verbose is gray
                case LogLevel.Verbose:
                    consoleColor = ConsoleColor.Gray;
                    break;
                // Warning is yellow
                case LogLevel.Warning:
                    consoleColor = ConsoleColor.DarkYellow;
                    break;
                // Error is red
                case LogLevel.Error:
                    consoleColor = ConsoleColor.Red;
                    break;
                // Success is green
                case LogLevel.Success:
                    consoleColor = ConsoleColor.Green;
                    break;

            }

            // Write message to the Console
            Console.WriteLine(message);

            // Reset color
            Console.ResetColor();
        }
    }
}
