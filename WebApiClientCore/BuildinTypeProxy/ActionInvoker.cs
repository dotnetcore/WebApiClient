using System;
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
        /// api描述
        /// </summary>
        private readonly ApiActionDescriptor apiAction;

        /// <summary>
        /// ApiAction执行器
        /// </summary>
        private readonly Func<ApiRequestContext, Task<ApiResponseContext>> handler;


        /// <summary>
        /// 上下文执行器
        /// </summary>
        /// <param name="apiAction"></param>
        public ActionInvoker(ApiActionDescriptor apiAction)
        {
            this.apiAction = apiAction;
            this.handler = ActionHandlerBuilder.Build(apiAction);
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        object IActionInvoker.Invoke(ServiceContext context, object[] arguments)
        {
            return this.InvokeAsync(context, arguments);
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        public Task<TResult> InvokeAsync(ServiceContext context, object[] arguments)
        {
            using var httpContext = new HttpContext(context.Client, context.Services, context.Options);
            var requestContext = new ApiRequestContext(httpContext, this.apiAction, arguments);
            return this.InvokeAsync(requestContext);
        }

        /// <summary>
        /// 执行Api方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<TResult> InvokeAsync(ApiRequestContext context)
        {
            var response = await this.handler(context);

            if (response.ResultStatus == ResultStatus.HasResult)
            {
                return (TResult)response.Result;
            }
            else if (response.ResultStatus == ResultStatus.HasException)
            {
                throw response.Exception;
            }

            throw new ApiResultNotSupportedExteption(context.HttpContext.ResponseMessage, context.ApiAction.Return.DataType.Type);
        }
    }
}
