using System;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 请求上下文执行器
    /// </summary>
    static class ApiRequestContextExecuter
    {
        /// <summary>
        /// 执行上下文
        /// </summary>
        /// <param name="request">请求上下文</param>
        /// <returns></returns>
        public static async Task<ApiResponseContext> ExecuteAsync(ApiRequestContext request)
        {
            await HandleRequestAsync(request).ConfigureAwait(false);
            var response = await HttpRequest.SendAsync(request).ConfigureAwait(false);
            await HandleResponseAsync(response).ConfigureAwait(false);
            return response;
        }

        /// <summary>
        /// 处理请求上下文
        /// </summary>
        /// <returns></returns>
        private static async Task HandleRequestAsync(ApiRequestContext context)
        {
            // 参数验证
            var validateProperty = context.HttpContext.HttpApiOptions.UseParameterPropertyValidate;
            foreach (var parameter in context.ApiAction.Parameters)
            {
                var parameterValue = context.Arguments[parameter.Index];
                DataValidator.ValidateParameter(parameter, parameterValue, validateProperty);
            }

            // action特性请求前执行
            foreach (var attr in context.ApiAction.Attributes)
            {
                await attr.OnRequestAsync(context).ConfigureAwait(false);
            }

            // 参数特性请求前执行
            foreach (var parameter in context.ApiAction.Parameters)
            {
                var ctx = new ApiParameterContext(context, parameter);
                foreach (var attr in parameter.Attributes)
                {
                    await attr.OnRequestAsync(ctx).ConfigureAwait(false);
                }
            }

            // Return特性请求前执行
            foreach (var @return in context.ApiAction.Return.Attributes)
            {
                await @return.OnRequestAsync(context).ConfigureAwait(false);
            }

            // GlobalFilter请求前执行 
            foreach (var filter in context.HttpContext.HttpApiOptions.GlobalFilters)
            {
                await filter.OnRequestAsync(context).ConfigureAwait(false);
            }

            // Filter请求前执行 
            foreach (var filter in context.ApiAction.FilterAttributes)
            {
                await filter.OnRequestAsync(context).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 处理响应上下文
        /// </summary>
        /// <returns></returns>
        private static async Task HandleResponseAsync(ApiResponseContext context)
        {
            // Return特性请求后执行
            foreach (var @return in context.ApiAction.Return.Attributes)
            {
                try
                {
                    await @return.OnResponseAsync(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    context.Exception = ex;
                }

                if (context.ResultStatus != ResultStatus.None)
                {
                    break;
                }
            }

            // 结果验证
            DataValidator.ValidateReturnValue(context);

            // GlobalFilter请求后执行 
            foreach (var filter in context.HttpContext.HttpApiOptions.GlobalFilters)
            {
                await filter.OnResponseAsync(context).ConfigureAwait(false);
            }

            // Filter请求后执行
            foreach (var filter in context.ApiAction.FilterAttributes)
            {
                await filter.OnResponseAsync(context).ConfigureAwait(false);
            }
        }
    }
}
