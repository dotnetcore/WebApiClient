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
        /// 时间格式
        /// </summary>
        private static readonly string format = "yyyy-MM-dd HH:mm:ss.fff";

        /// <summary>
        /// 获取是否输出请求内容
        /// </summary>
        public bool TraceRequest { get; set; } = true;

        /// <summary>
        /// 获取是否输出响应内容
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
            var builder = new StringBuilder();
            if (this.TraceRequest == true)
            {
                var requestString = await context.RequestMessage.GetRequestStringAsync().ConfigureAwait(false);
                builder.AppendLine($"[REQUEST] {DateTime.Now.ToString(format)}").Append(requestString);
            }

            var request = new Request { Builder = builder, DateTime = DateTime.Now };
            context.Tags.Set(tagKey, request);
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async override Task OnEndRequestAsync(ApiActionContext context)
        {
            var response = context.ResponseMessage;
            var request = context.Tags.Get(tagKey).As<Request>();
            var builder = request.Builder;

            if (this.TraceResponse && response != null && response.Content != null)
            {
                if (builder.Length > 0)
                {
                    builder.AppendLine();
                }

                builder
                    .AppendLine($"[RESPONSE] {DateTime.Now.ToString(format)}")
                    .Append(await this.GetResponseStringAsync(response).ConfigureAwait(false));
            }

            if (builder.Length > 0)
            {
                builder.AppendLine();
            }

            var message = builder
                .AppendLine($"[TIMESPAN] {DateTime.Now.Subtract(request.DateTime)}")
                .ToString();

            if (context.Exception == null)
            {
                this.LogInformation(context, message);
            }
            else
            {
                this.LogError(context, context.Exception, message);
            }
        }

        /// <summary>
        /// 返回响应字符串
        /// </summary>
        /// <param name="response">响应信息</param>
        /// <returns></returns>
        private async Task<string> GetResponseStringAsync(HttpResponseMessage response)
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

            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return builder.AppendLine().Append(content).ToString();
        }

        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="message">消息</param>
        protected abstract void LogInformation(ApiActionContext context, string message);

        /// <summary>
        /// 输出异常
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常</param>
        /// <param name="message">消息</param>
        protected abstract void LogError(ApiActionContext context, Exception exception, string message);

        /// <summary>
        /// 表示请求记录
        /// </summary>
        private class Request
        {
            /// <summary>
            /// 内容
            /// </summary>
            public StringBuilder Builder { get; set; }

            /// <summary>
            /// 时间
            /// </summary>
            public DateTime DateTime { get; set; }
        }
    }
}
