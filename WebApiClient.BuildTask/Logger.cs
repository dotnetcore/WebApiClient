using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WebApiClient.BuildTask
{
    /// <summary>
    /// 表示日志
    /// </summary>
    class Logger
    {
        /// <summary>
        /// 日志标签
        /// </summary>
        private const string tagName = "WebApiClient";

        /// <summary>
        /// 包装的日志类
        /// </summary>
        private readonly TaskLoggingHelper logger;

        /// <summary>
        /// 表示日志
        /// </summary>
        /// <param name="logger"></param>
        public Logger(TaskLoggingHelper logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="action">行为</param>
        /// <param name="value">值</param>
        public void Message(string action, string value)
        {
            var message = $"{action}: {value}";
            this.Message(message);
        }

        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="message">消息</param>
        public void Message(string message)
        {
            var log = $"{tagName} -> {message}";
            this.logger.LogMessage(MessageImportance.High, log);
        }

        /// <summary>
        /// 输出异常
        /// </summary>
        /// <param name="message">异常</param>
        public void Error(string message)
        {
            var log = $"{tagName} -> {message}";
            this.logger.LogError(log);
        }
    }
}
