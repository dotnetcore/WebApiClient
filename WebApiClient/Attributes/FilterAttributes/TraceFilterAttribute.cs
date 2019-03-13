using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将请求响应内容写入统一日志的过滤器
    /// </summary>
    public class TraceFilterAttribute : TraceFilterBaseAttribute
    {
        /// <summary>
        /// 获取或设置日志的EventId
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async override Task OnBeginRequestAsync(ApiActionContext context)
        {
            if (context.HttpApiConfig.LoggerFactory != null)
            {
                await base.OnBeginRequestAsync(context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public async override Task OnEndRequestAsync(ApiActionContext context)
        {
            if (context.HttpApiConfig.LoggerFactory != null)
            {
                await base.OnEndRequestAsync(context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 输出异常
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常</param>
        /// <param name="message">消息</param>
        protected override void LogError(ApiActionContext context, Exception exception, string message)
        {
            this.GetLogger(context).LogError(this.EventId, exception, message);
        }

        /// <summary>
        /// 输出消息
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="message">消息</param>
        protected override void LogInformation(ApiActionContext context, string message)
        {
            this.GetLogger(context).LogInformation(this.EventId, message);
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
    }
}
