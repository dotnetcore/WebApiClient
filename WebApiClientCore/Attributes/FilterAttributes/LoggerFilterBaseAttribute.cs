using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示追踪请求响应内容的抽象过滤器
    /// </summary>
    public abstract class LoggerFilterBaseAttribute : ApiActionFilterAttribute
    {
        /// <summary>
        /// tag的key
        /// </summary>
        private static readonly string tagKey = "$LoggerFilter";

        /// <summary>
        /// 获取或设置是否输出请求内容
        /// </summary>
        public bool LogRequest { get; set; } = true;

        /// <summary>
        /// 获取或设置是否输出响应内容
        /// </summary>
        public bool LogResponse { get; set; } = true;

        /// <summary>
        /// 追踪请求响应内容的过滤器
        /// </summary>
        public LoggerFilterBaseAttribute()
        {
            this.OrderIndex = int.MaxValue;
        }

        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async override Task BeforeRequestAsync(ApiActionContext context)
        {
            var message = new LoggerMessage
            {
                RequestTime = DateTime.Now,
                HasRequest = this.LogRequest
            };

            if (this.LogRequest == true)
            {
                var request = context.HttpContext.RequestMessage;
                message.RequestHeaders = request.GetHeadersString();

                if (request.Content is ICustomTracable httpContent)
                {
                    message.RequestContent = await httpContent.ReadAsStringAsync().ConfigureAwait(false);
                }
                else if (request.Content != null)
                {
                    message.RequestContent = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            context.Tags.Set(tagKey, message);
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async override Task AfterRequestAsync(ApiActionContext context)
        {
            var response = context.HttpContext.ResponseMessage;
            var message = context.Tags.Take(tagKey).As<LoggerMessage>();

            message.ResponseTime = DateTime.Now;
            message.Exception = context.Exception;

            if (this.LogResponse && response != null)
            {
                message.HasResponse = true;
                message.ResponseHeaders = this.GetResponseHeadersString(response);
                if (response.Content != null)
                {
                    message.ResponseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            await this.WriterLogAsync(context, message).ConfigureAwait(false);
        }

        /// <summary>
        /// 返回响应字符串
        /// </summary>
        /// <param name="response">响应信息</param>
        /// <returns></returns>
        private string GetResponseHeadersString(HttpResponseMessage response)
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
        /// 输出追踪到的消息
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="message">追踪的消息</param>
        /// <returns></returns>
        protected abstract Task WriterLogAsync(ApiActionContext context, LoggerMessage message);
    }
}
