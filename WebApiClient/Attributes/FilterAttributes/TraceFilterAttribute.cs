using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将请求响应内容写入统一日志的过滤器
    /// </summary>
    public class TraceFilterAttribute : ApiActionFilterAttribute
    {
        /// <summary>
        /// tag的key
        /// </summary>
        private static readonly string tagKey = "$TraceFilter";

        /// <summary>
        /// 获取或设置日志的EventId
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// 获取是否输出请求内容
        /// </summary>
        public bool TraceRequest { get; set; } = true;

        /// <summary>
        /// 获取是否输出响应内容
        /// </summary>
        public bool TraceResponse { get; set; } = true;

        /// <summary>
        /// 将请求响应内容写入统一日志的过滤器
        /// </summary>
        public TraceFilterAttribute()
        {
            this.OrderIndex = int.MaxValue;
        }

        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async override Task OnBeginRequestAsync(ApiActionContext context)
        {
            if (context.HttpApiConfig.LoggerFactory == null)
            {
                return;
            }

            var request = new Request
            {
                Time = DateTime.Now,
                Message = await context.RequestMessage.GetRequestStringAsync().ConfigureAwait(false)
            };
            context.Tags.Set(tagKey, request);
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async override Task OnEndRequestAsync(ApiActionContext context)
        {
            var logging = context.HttpApiConfig.LoggerFactory;
            if (logging == null)
            {
                return;
            }

            var builder = new StringBuilder();
            const string format = "yyyy-MM-dd HH:mm:ss.fff";
            var request = context.Tags.Get(tagKey).As<Request>();

            if (this.TraceRequest == true)
            {
                builder.AppendLine($"[REQUEST] {request.Time.ToString(format)}")
                    .AppendLine($"{request.Message.TrimEnd()}");
            }

            var response = context.ResponseMessage;
            if (this.TraceResponse && response != null && response.Content != null)
            {
                if (this.TraceRequest == true)
                {
                    builder.AppendLine();
                }

                builder.AppendLine($"[RESPONSE] {DateTime.Now.ToString(format)}")
                    .AppendLine($"{await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
            }

            var message = builder
                .AppendLine()
                .Append($"[TIMESPAN] {DateTime.Now.Subtract(request.Time)}")
                .ToString();

            var method = context.ApiActionDescriptor.Member;
            var categoryName = $"{method.DeclaringType.Name}.{method.Name}";
            var logger = logging.CreateLogger(categoryName);

            if (context.Exception == null)
            {
                logger.LogInformation(this.EventId, message);
            }
            else
            {
                logger.LogError(this.EventId, context.Exception, message);
            }
        }

        /// <summary>
        /// 请求信息
        /// </summary>
        private class Request
        {
            /// <summary>
            /// 请求时间
            /// </summary>
            public DateTime Time { get; set; }

            /// <summary>
            /// 请求消息
            /// </summary>
            public string Message { get; set; }
        }
    }
}
