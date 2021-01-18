using System;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示ApiAction执行器
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    class ActionInvoker<TResult> : IActionInvoker
    {
        /// <summary>
        /// 获取Action描述
        /// </summary>
        public ApiActionDescriptor ApiAction { get; }

        /// <summary>
        /// 上下文执行器
        /// </summary>
        /// <param name="apiAction"></param>
        public ActionInvoker(ApiActionDescriptor apiAction)
        {
            this.ApiAction = apiAction;
        }

        /// <summary>
        /// 执行Action
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        public object Invoke(HttpClientContext context, object?[] arguments)
        {
            return this.InvokeAsync(context, arguments);
        }

        /// <summary>
        /// 执行Action
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        public async Task<TResult> InvokeAsync(HttpClientContext context, object?[] arguments)
        {
            try
            {
                using var httpContext = new HttpContext(context);
                var requestContext = new ApiRequestContext(httpContext, this.ApiAction, arguments);
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
            var response = await ContextInvoker.InvokeAsync(request).ConfigureAwait(false);
            if (response.ResultStatus == ResultStatus.HasResult)
            {
                return (TResult)response.Result;
            }

            if (response.ResultStatus == ResultStatus.HasException)
            {
                var inner = response.Exception;
                throw new HttpRequestException(inner.Message, inner);
            }

            throw new ApiReturnNotSupportedExteption(response);
#nullable enable
        }
    }
}
