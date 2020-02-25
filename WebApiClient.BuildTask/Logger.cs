using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WebApiClient.BuildTask
{
    /// <summary>
    /// Means log
    /// </summary>
    class Logger
    {
        /// <summary>
        /// Log label
        /// </summary>
        private const string tagName = "WebApiClient";

        /// <summary>
        /// Wrapped log class
        /// </summary>
        private readonly TaskLoggingHelper logger;

        /// <summary>
        /// Means log
        /// </summary>
        /// <param name="logger"></param>
        public Logger(TaskLoggingHelper logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Output message
        /// </summary>
        /// <param name="action">behavior</param>
        /// <param name="value">value</param>
        public void Message(string action, string value)
        {
            var message = $"{action}: {value}";
            this.Message(message);
        }

        /// <summary>
        /// Output message
        /// </summary>
        /// <param name="message">message</param>
        public void Message(string message)
        {
            var log = $"{tagName} -> {message}";
            this.logger.LogMessage(MessageImportance.High, log);
        }

        /// <summary>
        /// Error message
        /// </summary>
        /// <param name="message">message</param>
        public void Error(string message)
        {
            var log = $"{tagName} -> {message}";
            this.logger.LogError(log);
        }
    }
}
