using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Implementations.Tasks;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示Action执行器
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    [DebuggerDisplay("Member = {ActionDescriptor.Member}")]
    public class DefaultApiActionInvoker<TResult> : ApiActionInvoker, IITaskReturnConvertable
    {
        /// <summary>
        /// 请求委托
        /// </summary>
        private RequestDelegate? requestDelegate;

        /// <summary>
        /// 获取Action描述
        /// </summary>
        public override ApiActionDescriptor ActionDescriptor { get; }

        /// <summary>
        /// Action执行器
        /// </summary>
        /// <param name="actionDescriptor"></param>
        /// <param name="httpApiOptionsMonitor"></param>
        public DefaultApiActionInvoker(ApiActionDescriptor actionDescriptor, IOptionsMonitor<HttpApiOptions> httpApiOptionsMonitor)
        {
            this.ActionDescriptor = actionDescriptor;
            httpApiOptionsMonitor.OnChange(options => this.requestDelegate = null);
        }

        /// <summary>
        /// 转换为ITask返回声明包装的Action执行器
        /// </summary>
        /// <returns></returns>
        public ApiActionInvoker ToITaskReturnActionInvoker()
        {
            return new ITaskReturnActionInvoker(this);
        }

        /// <summary>
        /// 执行Action
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        public sealed override object Invoke(HttpClientContext context, object?[] arguments)
        {
            return this.InvokeAsync(context, arguments);
        }

        /// <summary>
        /// 执行Action
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        public virtual async Task<TResult> InvokeAsync(HttpClientContext context, object?[] arguments)
        {
            try
            {
                var requiredUri = context.HttpApiOptions.HttpHost ?? context.HttpClient.BaseAddress;
                var useDefaultUserAgent = context.HttpApiOptions.UseDefaultUserAgent;
                using var requestMessage = new HttpApiRequestMessageImpl(requiredUri, useDefaultUserAgent);

                var httpContext = new HttpContext(context, requestMessage);
                var requestContext = new ApiRequestContext(httpContext, this.ActionDescriptor, arguments, new DefaultDataCollection());
                return await this.InvokeAsync(requestContext).ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
#if NET5_0_OR_GREATER
                if (ex is IStatusCodeException exception)
                {
                    throw new HttpRequestException(ex.Message, ex, exception.GetStatusCode());
                }
#endif
                throw new HttpRequestException(ex.Message, ex);
            }
        }


        /// <summary>
        /// 执行Api方法
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<TResult> InvokeAsync(ApiRequestContext request)
        {
            this.requestDelegate ??= ApiRequestExecutor.Build(request); // 不需要 lock，并发 build 也不影响结果
            var response = await this.requestDelegate(request).ConfigureAwait(false);

#nullable disable           
            if (response.ResultStatus == ResultStatus.HasResult)
            {
                return (TResult)response.Result;
            }

            if (response.ResultStatus == ResultStatus.HasException)
            {
                ExceptionDispatchInfo.Capture(response.Exception).Throw();
            }

            throw new ApiReturnNotSupportedException(response);
#nullable enable
        }


        /// <summary>
        /// 表示ITask返回声明的Action执行器
        /// </summary> 
        [DebuggerDisplay("Member = {ActionDescriptor.Member}")]
        private class ITaskReturnActionInvoker : ApiActionInvoker
        {
            /// <summary>
            /// Api执行器
            /// </summary>
            private readonly DefaultApiActionInvoker<TResult> actionInvoker;

            /// <summary>
            /// 获取Action描述
            /// </summary>
            public override ApiActionDescriptor ActionDescriptor => this.actionInvoker.ActionDescriptor;

            /// <summary>
            /// ITask返回声明的Action执行器
            /// </summary>
            /// <param name="actionInvoker">Action执行器 </param>
            public ITaskReturnActionInvoker(DefaultApiActionInvoker<TResult> actionInvoker)
            {
                this.actionInvoker = actionInvoker;
            }

            /// <summary>
            /// 执行Action
            /// </summary>
            /// <param name="context">上下文</param>
            /// <param name="arguments">参数值</param>
            /// <returns></returns>
            public override object Invoke(HttpClientContext context, object?[] arguments)
            {
                return new ActionTask<TResult>(this.actionInvoker, context, arguments);
            }
        }
    }
}
