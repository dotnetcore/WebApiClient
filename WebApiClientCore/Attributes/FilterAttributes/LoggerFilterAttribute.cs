using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将请求响应内容指定目标的过滤器
    /// </summary>
    public class LoggerFilterAttribute : LoggerFilterBaseAttribute
    {
        /// <summary>
        /// 输出追踪到的消息
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="message">追踪的消息</param>
        /// <returns></returns>
        protected override Task WriterLogAsync(ApiActionContext context, LoggerMessage message)
        {
            var method = context.ApiAction.Member;
            var actionName = $"{method.DeclaringType.Namespace}.{method.DeclaringType.Name}.{method.Name}";
            this.WriteLoggerFactory(context, actionName, message);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 写入LoggerFactory
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="categoryName">日志容器名称</param>
        /// <param name="mesage">追踪的消息</param>
        private void WriteLoggerFactory(ApiActionContext context, string categoryName, LoggerMessage mesage)
        {
            var logging = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
            if (logging == null)
            {
                return;
            }

            var logger = logging.CreateLogger(categoryName);
            var message = this.FormatMessage(mesage);
            logger.LogInformation(message);
            if (mesage.Exception != null)
            {
                logger.LogError(mesage.Exception, mesage.Exception.GetType().Name);
            }
        }

        /// <summary>
        /// 格式化追踪到的消息
        /// </summary> 
        /// <param name="message">追踪的消息</param>
        /// <returns></returns>
        protected virtual string FormatMessage(LoggerMessage message)
        {
            return message.ToExcludeException().ToString();
        }
    }
}
