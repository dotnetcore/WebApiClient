using System;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 上下文执行器
    /// </summary>
    static class ContextInvoker
    {
        /// <summary>
        /// 上下文处理委托
        /// </summary>
        private static readonly Func<ApiRequestContext, Task<ApiResponseContext>> handler;

        /// <summary>
        /// 上下文执行器
        /// </summary>
        static ContextInvoker()
        {
            var requestHandler = BuildRequestHandler();
            var responseHandler = BuildResponseHandler();

            handler = async request =>
            {
                await requestHandler(request).ConfigureAwait(false);
                var response = await HttpRequest.SendAsync(request).ConfigureAwait(false);
                await responseHandler(response).ConfigureAwait(false);
                return response;
            };
        }


        /// <summary>
        /// 执行上下文
        /// </summary>
        /// <param name="request">请求上下文</param>
        /// <returns></returns>
        public static Task<ApiResponseContext> InvokeAsync(ApiRequestContext request)
        {
            return handler(request);
        }

        /// <summary>
        /// 创建请求上下文处理委托
        /// </summary>
        /// <returns></returns>
        private static InvokeDelegate<ApiRequestContext> BuildRequestHandler()
        {
            var builder = new PipelineBuilder<ApiRequestContext>();

            // 参数验证
            builder.Use(next => context =>
            {
                var validateProperty = context.HttpContext.HttpApiOptions.UseParameterPropertyValidate;
                foreach (var parameter in context.ApiAction.Parameters)
                {
                    var parameterValue = context.Arguments[parameter.Index];
                    DataValidator.ValidateParameter(parameter, parameterValue, validateProperty);
                }
                return next(context);
            });

            // action特性请求前执行
            builder.Use(next => async context =>
            {
                foreach (var attr in context.ApiAction.Attributes)
                {
                    await attr.OnRequestAsync(context).ConfigureAwait(false);
                }
                await next(context).ConfigureAwait(false);
            });

            // 参数特性请求前执行
            builder.Use(next => async context =>
            {
                foreach (var parameter in context.ApiAction.Parameters)
                {
                    var ctx = new ApiParameterContext(context, parameter.Index);
                    foreach (var attr in parameter.Attributes)
                    {
                        await attr.OnRequestAsync(ctx).ConfigureAwait(false);
                    }
                }
                await next(context).ConfigureAwait(false);
            });

            // Return特性请求前执行
            builder.Use(next => async context =>
            {
                foreach (var @return in context.ApiAction.Return.Attributes)
                {
                    await @return.OnRequestAsync(context).ConfigureAwait(false);
                }
                await next(context).ConfigureAwait(false);
            });

            // Filter请求前执行    
            builder.Use(next => async context =>
            {
                foreach (var filter in context.ApiAction.FilterAttributes)
                {
                    await filter.OnRequestAsync(context).ConfigureAwait(false);
                }
                await next(context).ConfigureAwait(false);
            });

            return builder.Build();
        }

        /// <summary>
        /// 创建响应上下文处理委托
        /// </summary>
        /// <returns></returns>
        private static InvokeDelegate<ApiResponseContext> BuildResponseHandler()
        {
            var builder = new PipelineBuilder<ApiResponseContext>();

            // Return特性请求后执行
            builder.Use(next => async context =>
            {
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
                await next(context).ConfigureAwait(false);
            });

            // 验证Result是否ok
            builder.Use(next => context =>
            {
                if (context.ApiAction.Return.DataType.IsRawType == true)
                {
                    return next(context);
                }

                if (context.ResultStatus != ResultStatus.HasResult)
                {
                    return next(context);
                }

                if (context.HttpContext.HttpApiOptions.UseReturnValuePropertyValidate == false)
                {
                    return next(context);
                }

                try
                {
                    DataValidator.ValidateReturnValue(context.Result);
                }
                catch (Exception ex)
                {
                    context.Exception = ex;
                }
                return next(context);
            });

            // Filter请求后执行
            builder.Use(next => async context =>
            {
                foreach (var filter in context.ApiAction.FilterAttributes)
                {
                    await filter.OnResponseAsync(context).ConfigureAwait(false);
                }
                await next(context).ConfigureAwait(false);
            });

            return builder.Build();
        }
    }
}
