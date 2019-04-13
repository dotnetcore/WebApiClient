using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示追踪请求响应内容的抽象过滤器
    /// </summary>
    public abstract class TraceFilterBaseAttribute : ApiActionFilterAttribute
    {
        /// <summary>
        /// tag的key
        /// </summary>
        private static readonly string tagKey = "$TraceFilter";

        /// <summary>
        /// 获取或设置是否输出请求内容
        /// </summary>
        public bool TraceRequest { get; set; } = true;

        /// <summary>
        /// 获取或设置是否输出响应内容
        /// </summary>
        public bool TraceResponse { get; set; } = true;

        /// <summary>
        /// 追踪请求响应内容的过滤器
        /// </summary>
        public TraceFilterBaseAttribute()
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
            var message = new TraceMessage
            {
                RequestTime = DateTime.Now,
                HasRequest = this.TraceRequest
            };

            if (this.TraceRequest == true)
            {
                var request = context.RequestMessage;
                message.RequestHeaders = request.GetHeadersString();
                if (request.Content != null)
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
        public async override Task OnEndRequestAsync(ApiActionContext context)
        {
            var response = context.ResponseMessage;
            var message = context.Tags.Take(tagKey).As<TraceMessage>();

            message.ResponseTime = DateTime.Now;
            message.Exception = context.Exception;

            if (this.TraceResponse && response != null)
            {
                message.HasResponse = true;
                message.ResponseHeaders = this.GetResponseHeadersString(response);
                if (response.Content != null)
                {
                    message.ResponseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            await this.LogTraceMessageAsync(context, message).ConfigureAwait(false);
        }

        /// <summary>
        /// 返回响应字符串
        /// </summary>
        /// <param name="response">响应信息</param>
        /// <returns></returns>
        private string GetResponseHeadersString(HttpResponseMessage response)
        {
            var builder = new StringBuilder()
                .AppendLine($"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}");

            foreach (var item in response.Headers)
            {
                builder.AppendLine($"{item.Key}: {string.Join(",", item.Value)}");
            }

            if (response.Content != null)
            {
                foreach (var item in response.Content.Headers)
                {
                    builder.AppendLine($"{item.Key}: {string.Join(",", item.Value)}");
                }
            }

            return builder.AppendLine().ToString();
        }

        /// <summary>
        /// 输出追踪到的消息
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="traceMessage">追踪的消息</param>
        /// <returns></returns>
        protected abstract Task LogTraceMessageAsync(ApiActionContext context, TraceMessage traceMessage);
    }
}
