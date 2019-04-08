using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Text;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将请求响应内容指定目标的过滤器
    /// 默认是OutputTarget.LoggerFactory
    /// </summary>
    public class TraceFilterAttribute : TraceFilterBaseAttribute
    {
        /// <summary>
        /// 获取或设置日志工厂的EventId
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// 获取或设置输出目标
        /// </summary>
        public OutputTarget OutputTarget { get; set; } = OutputTarget.LoggerFactory;

        /// <summary>
        /// 输出异常
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常</param>
        /// <param name="message">消息</param>
        protected override void LogError(ApiActionContext context, Exception exception, string message)
        {
            this.Log(context, message, exception);
        }

        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="message">消息</param>
        protected override void LogInformation(ApiActionContext context, string message)
        {
            this.Log(context, message, exception: null);
        }

        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="context"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>       
        private void Log(ApiActionContext context, string message, Exception exception)
        {
            if (this.OutputTarget.HasFlag(OutputTarget.LoggerFactory))
            {
                if (exception == null)
                {
                    this.GetLogger(context)?.LogInformation(message);
                }
                else
                {
                    this.GetLogger(context)?.LogError(this.EventId, exception, message);
                }
            }

            if (this.OutputTarget.HasFlag(OutputTarget.Debug))
            {
                Debug.Write(this.GetLogContent(context, message, exception));
            }

            if (this.OutputTarget.HasFlag(OutputTarget.Console))
            {
                Console.Write(this.GetLogContent(context, message, exception));
            }

#if !NETSTANDARD1_3
            if (this.OutputTarget.HasFlag(OutputTarget.Debugger))
            {
                Debugger.Log(0, null, this.GetLogContent(context, message, exception));
            }
#endif
        }

        /// <summary>
        /// 获取日志组件
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private ILogger GetLogger(ApiActionContext context)
        {
            var logging = context.HttpApiConfig.LoggerFactory;
            var method = context.ApiActionDescriptor.Member;
            var categoryName = $"{method.DeclaringType.Name}.{method.Name}";
            return logging.CreateLogger(categoryName);
        }

        /// <summary>
        /// 获取完整日志内容
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="message">消息</param>
        /// <param name="exception">异常</param>
        /// <returns></returns>
        private string GetLogContent(ApiActionContext context, string message, Exception exception)
        {
            var method = context.ApiActionDescriptor.Member;
            var methodName = $"{method.DeclaringType.Name}.{method.Name}";

            var log = new StringBuilder()
                .AppendLine(methodName)
                .AppendLine(message);

            if (exception != null)
            {
                log.AppendLine(exception.ToString());
            }
            return log.ToString();
        }
    }
}
