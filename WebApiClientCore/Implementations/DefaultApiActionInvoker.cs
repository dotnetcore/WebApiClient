using System;
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
    public class DefaultApiActionInvoker<TResult> : ApiActionInvoker, IITaskReturnConvertable
    {
        /// <summary>
        /// 获取Action描述
        /// </summary>
        public override ApiActionDescriptor ActionDescriptor { get; }

        /// <summary>
        /// Action执行器
        /// </summary>
        /// <param name="actionDescriptor"></param>
        public DefaultApiActionInvoker(ApiActionDescriptor actionDescriptor)
        {
            this.ActionDescriptor = actionDescriptor;
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
                var requestContext = new ApiRequestContext(httpContext, this.ActionDescriptor, arguments, new DefaultDataCollection(), context.HttpApiOptions.IsIgnoreAutoUri);
                return await this.InvokeAsync(requestContext).ConfigureAwait(false);
            }
            catch (HttpRequestException)
            {
                throw;
            }
            catch (Exception ex)
            {
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
#nullable disable
            var response = await ApiRequestExecuter.ExecuteAsync(request).ConfigureAwait(false);
            if (response.ResultStatus == ResultStatus.HasResult)
            {
                return (TResult)response.Result;
            }

            if (response.ResultStatus == ResultStatus.HasException)
            {
                ExceptionDispatchInfo.Capture(response.Exception).Throw();
            }

            throw new ApiReturnNotSupportedExteption(response);
#nullable enable
        }


        /// <summary>
        /// 表示ITask返回声明的Action执行器
        /// </summary> 
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
