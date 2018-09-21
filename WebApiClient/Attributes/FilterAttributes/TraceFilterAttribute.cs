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
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async override Task OnBeginRequestAsync(ApiActionContext context)
        {
            var logger = context.HttpApiConfig.Logger;
            if (logger == null)
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
            if (logger == null)
            {
                return;
            }

            var builder = new StringBuilder();
            const string format = "yyyy-MM-dd HH:mm:ss.fff";

            var request = context.Tags.Get(tagKey).As<Request>();
            builder
                .AppendLine($"[REQUEST] [{request.Time.ToString(format)}]")
                .AppendLine($"{request.Message.TrimEnd()}");

            var response = context.ResponseMessage;
            if (response != null && response.Content != null)
            {
                builder
                    .AppendLine()
                    .AppendLine($"[RESPONSE] [{DateTime.Now.ToString(format)}]")
                    .AppendLine($"{await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
            }


            var message = builder
                .AppendLine()
                .AppendLine($"[TIMESPAN] {DateTime.Now.Subtract(request.Time)}")
                .ToString();

            if (context.Exception == null)
            {
                logger.LogInformation(message);
            }
            else
            {
                var id = response == null ? 0 : (int)response.StatusCode;
                logger.LogError(id, context.Exception, message);
            }
        }

        /// <summary>
        /// 请求信息
        /// </summary>
        private class Request
        {
            public DateTime Time { get; set; }

            public string Message { get; set; }
        }
    }
}
