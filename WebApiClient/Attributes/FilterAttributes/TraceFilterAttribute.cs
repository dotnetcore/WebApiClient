using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将请求响应内容写入HttpApiConfig.Logger的过滤器
    /// </summary>
    public class TraceFilterAttribute : ApiActionFilterAttribute
    {
        /// <summary>
        /// tag的key
        /// </summary>
        private static readonly string tagKey = "TraceFilter";

        /// <summary>
        /// 获取日志的EventId
        /// </summary>
        public int EventId { get; private set; }

        /// <summary>
        /// 获取或设置是否启用
        /// </summary>
        public bool Enable { get; set; } = true;

        /// <summary>
        /// 将请求响应内容写入HttpApiConfig.Logger的过滤器
        /// </summary>
        /// <param name="eventId">日志的EventId</param>
        public TraceFilterAttribute(int eventId = 1)
        {
            this.EventId = eventId;
        }

        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async override Task OnBeginRequestAsync(ApiActionContext context)
        {
            var logger = context.HttpApiConfig.Logger;
            if (logger == null || this.Enable == false)
            {
                return;
            }

            var request = new Request
            {
                Time = DateTime.Now,
                Message = await context.RequestMessage.ToStringAsync().ConfigureAwait(false)
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
            var logger = context.HttpApiConfig.Logger;
            if (logger == null || this.Enable == false)
            {
                return;
            }

            const string format = "yyyy-MM-dd HH:mm:ss.fff";
            var request = context.Tags.Get(tagKey).As<Request>();
            var method = context.ApiActionDescriptor.Member;

            var builder = new StringBuilder()
                .AppendLine($"{method.DeclaringType.Name}.{method.Name}()")
                .AppendLine()

                .AppendLine($"[REQUEST] {request.Time.ToString(format)}")
                .AppendLine($"{request.Message.TrimEnd()}");

            var response = context.ResponseMessage;
            if (response != null && response.Content != null)
            {
                builder
                    .AppendLine()
                    .AppendLine($"[RESPONSE] {DateTime.Now.ToString(format)}")
                    .AppendLine($"{await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
            }


            var message = builder
                .AppendLine()
                .AppendLine($"[TIMESPAN] {DateTime.Now.Subtract(request.Time)}")
                .ToString();

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
