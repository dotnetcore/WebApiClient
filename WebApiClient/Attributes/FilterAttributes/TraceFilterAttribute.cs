using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
        /// 获取或设置事件Id
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// 获取或设置输出目标
        /// </summary>
        public OutputTarget OutputTarget { get; set; } = OutputTarget.LoggerFactory;

        /// <summary>
        /// 准备请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed async override Task OnBeginRequestAsync(ApiActionContext context)
        {
            if (this.IsNeedToTrace(context) == true)
            {
                await base.OnBeginRequestAsync(context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 请求完成之后
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public sealed async override Task OnEndRequestAsync(ApiActionContext context)
        {
            if (this.IsNeedToTrace(context) == true)
            {
                await base.OnEndRequestAsync(context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 返回是否需要追踪
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        private bool IsNeedToTrace(ApiActionContext context)
        {
            if (this.OutputTarget == OutputTarget.LoggerFactory)
            {
                if (context.HttpApiConfig.LoggerFactory == null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 输出追踪到的消息
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="traceMessage">追踪的消息</param>
        /// <returns></returns>
        protected override Task LogTraceMessageAsync(ApiActionContext context, TraceMessage traceMessage)
        {
            var method = context.ApiActionDescriptor.Member;
            var actionName = $"{method.DeclaringType.Name}.{method.Name}";

            if (this.OutputTarget.HasFlag(OutputTarget.LoggerFactory))
            {
                this.WriteLoggerFactory(context, actionName, traceMessage);
            }

            if (this.OutputTarget.HasFlag(OutputTarget.Console))
            {
                var message = this.FormatTraceMessage(OutputTarget.Console, traceMessage);
                Console.Write($"{actionName}[{this.EventId}]{Environment.NewLine}{message}");
            }

#if !NETSTANDARD1_3
            if (this.OutputTarget.HasFlag(OutputTarget.Debug))
            {
                var message = this.FormatTraceMessage(OutputTarget.Debug, traceMessage);
                Trace.Write($"{actionName}[{this.EventId}]{Environment.NewLine}{message}");
            }

            if (this.OutputTarget.HasFlag(OutputTarget.Debugger))
            {
                var message = this.FormatTraceMessage(OutputTarget.Debugger, traceMessage);
                Debugger.Log(0, null, $"{actionName}[{this.EventId}]{Environment.NewLine}{message}");
            }
#endif
            return ApiTask.CompletedTask;
        }


        /// <summary>
        /// 写入LoggerFactory
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="categoryName">日志容器名称</param>
        /// <param name="traceMessage">追踪的消息</param>
        private void WriteLoggerFactory(ApiActionContext context, string categoryName, TraceMessage traceMessage)
        {
            var logging = context.HttpApiConfig.LoggerFactory;
            if (logging == null)
            {
                return;
            }

            var logger = logging.CreateLogger(categoryName);
            var message = this.FormatTraceMessage(OutputTarget.LoggerFactory, traceMessage);

            if (traceMessage.Exception == null)
            {
                logger.LogInformation(this.EventId, message);
            }
            else
            {
                logger.LogError(this.EventId, traceMessage.Exception, message);
            }
        }

        /// <summary>
        /// 格式化追踪到的消息
        /// </summary>
        /// <param name="outputTarget">输出目标</param>
        /// <param name="traceMessage">追踪的消息</param>
        /// <returns></returns>
        protected virtual string FormatTraceMessage(OutputTarget outputTarget, TraceMessage traceMessage)
        {
            if (outputTarget == OutputTarget.LoggerFactory)
            {
                return traceMessage.ToString();
            }
            else
            {
                return traceMessage.ToIndentedString(spaceCount: 4);
            }
        }
    }
}
