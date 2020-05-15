using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示将请求和响应内容的输出为日志的过滤器
    /// </summary>
    public class LoggingFilterAttribute : ApiFilterAttribute
    {
        /// <summary>
        /// tag的key
        /// </summary>
        private static readonly string tagKey = "$LoggingFilter";

        /// <summary>
        /// 获取或设置是否输出请求内容
        /// </summary>
        public bool LogRequest { get; set; } = true;

        /// <summary>
        /// 获取或设置是否输出响应内容
        /// </summary>
        public bool LogResponse { get; set; } = true;

        /// <summary>
        /// 将请求和响应内容的输出为日志的过滤器
        /// </summary>
        public LoggingFilterAttribute()
        {
            this.OrderIndex = int.MaxValue;
        }

        /// <summary>
        /// 请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed async override Task OnRequestAsync(ApiRequestContext context)
        {
            var logMessage = new LogMessage
            {
                RequestTime = DateTime.Now,
                HasRequest = this.LogRequest
            };

            if (this.LogRequest == true)
            {
                var request = context.HttpContext.RequestMessage;
                logMessage.RequestHeaders = request.GetHeadersString();

                if (request.Content is ICustomTracable httpContent)
                {
                    logMessage.RequestContent = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
                }
                else if (request.Content != null)
                {
                    logMessage.RequestContent = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            context.Tags.Set(tagKey, logMessage);
        }

        /// <summary>
        /// 响应后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed async override Task OnResponseAsync(ApiResponseContext context)
        {
            var response = context.HttpContext.ResponseMessage;
            var logMessage = context.Tags.Take(tagKey).As<LogMessage>();

            logMessage.ResponseTime = DateTime.Now;
            logMessage.Exception = context.Exception;

            if (this.LogResponse && response != null)
            {
                logMessage.HasResponse = true;
                logMessage.ResponseHeaders = this.ReadResponseHeadersString(response);
                if (response.Content != null)
                {
                    logMessage.ResponseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            this.WriterLog(context, logMessage);
        }

        /// <summary>
        /// 读取响应字符串
        /// </summary>
        /// <param name="response">响应信息</param>
        /// <returns></returns>
        private string ReadResponseHeadersString(HttpResponseMessage response)
        {
            var builder = new StringBuilder()
                .AppendLine($"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}")
                .Append(response.Headers.ToString());

            if (response.Content != null)
            {
                builder.Append(response.Content.Headers.ToString());
            }
            return builder.ToString();
        }

        /// <summary>
        /// 写日志到LoggerFactory
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="logMessage">日志消息</param>
        /// <returns></returns>
        protected virtual void WriterLog(ApiRequestContext context, LogMessage logMessage)
        {
            var method = context.ApiAction.Member;
            var categoryName = $"{method.DeclaringType.Namespace}.{method.DeclaringType.Name}.{method.Name}";
            var logger = context.HttpContext.Services.GetRequiredService<ILoggerFactory>().CreateLogger(categoryName);

            var message = this.GetMessage(logMessage);
            logger.LogInformation(message);

            if (logMessage.Exception != null)
            {
                logger.LogError(logMessage.Exception, logMessage.Exception.Message);
            }
        }

        /// <summary>
        /// 获取文本格式的日志消息
        /// </summary> 
        /// <param name="logMessage">日志消息</param>
        /// <returns></returns>
        protected virtual string GetMessage(LogMessage logMessage)
        {
            return logMessage.ToExcludeException().ToString();
        }
    }
}
