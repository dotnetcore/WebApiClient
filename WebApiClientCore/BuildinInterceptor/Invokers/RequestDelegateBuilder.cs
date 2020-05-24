using System;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供Action的调用链委托创建
    /// </summary>
    static class RequestDelegateBuilder
    {
        /// <summary>
        /// 创建执行委托
        /// </summary>
        /// <param name="apiAction">action描述器</param>
        /// <returns></returns>
        public static Func<ApiRequestContext, Task<ApiResponseContext>> Build(ApiActionDescriptor apiAction)
        {
            var requestHandler = BuildRequestHandler(apiAction);
            var responseHandler = BuildResponseHandler(apiAction);

            return async request =>
            {
                await requestHandler(request).ConfigureAwait(false);
                var response = await HttpRequest.SendAsync(request).ConfigureAwait(false);
                await responseHandler(response).ConfigureAwait(false);
                return response;
            };
        }

        /// <summary>
        /// 创建请求委托
        /// </summary>
        /// <param name="apiAction"></param>
        /// <returns></returns>
        private static InvokeDelegate<ApiRequestContext> BuildRequestHandler(ApiActionDescriptor apiAction)
        {
            var builder = new PipelineBuilder<ApiRequestContext>();

            // 参数验证特性验证和参数模型属性特性验证
            builder.Use(next => context =>
            {
                var validateProperty = context.HttpContext.Options.UseParameterPropertyValidate;
                foreach (var parameter in context.ApiAction.Parameters)
                {
                    var parameterValue = context.Arguments[parameter.Index];
                    DataValidator.ValidateParameter(parameter, parameterValue, validateProperty);
                }
                return next(context);
            });

            // action特性请求前执行
            foreach (var attr in apiAction.Attributes)
            {
                builder.Use(next => async context =>
                {
                    await attr.OnRequestAsync(context).ConfigureAwait(false);
                    await next(context).ConfigureAwait(false);
                });
            }

            // 参数特性请求前执行
            foreach (var parameter in apiAction.Parameters)
            {
                var index = parameter.Index;
                foreach (var attr in parameter.Attributes)
                {
                    builder.Use(next => async context =>
                    {
                        var ctx = new ApiParameterContext(context, index);
                        await attr.OnRequestAsync(ctx).ConfigureAwait(false);
                        await next(context).ConfigureAwait(false);
                    });
                }
            }

            // Return特性请求前执行
            foreach (var @return in apiAction.Return.Attributes)
            {
                builder.Use(next => async context =>
                {
                    await @return.OnRequestAsync(context).ConfigureAwait(false);
                    await next(context).ConfigureAwait(false);
                });
            }

            // Filter请求前执行            
            foreach (var filter in apiAction.FilterAttributes)
            {
                builder.Use(next => async context =>
                {
                    await filter.OnRequestAsync(context).ConfigureAwait(false);
                    await next(context).ConfigureAwait(false);
                });
            }

            return builder.Build();
        }

        /// <summary>
        /// 创建响应委托
        /// </summary>
        /// <param name="apiAction"></param>
        /// <returns></returns>
        private static InvokeDelegate<ApiResponseContext> BuildResponseHandler(ApiActionDescriptor apiAction)
        {
            var builder = new PipelineBuilder<ApiResponseContext>();

            // Return特性请求后执行
            foreach (var @return in apiAction.Return.Attributes)
            {
                builder.Use(next => async context =>
                {
                    if (context.ResultStatus == ResultStatus.None)
                    {
                        try
                        {
                            await @return.OnResponseAsync(context).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            context.Exception = ex;
                        }
                    }
                    await next(context).ConfigureAwait(false);
                });
            }

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

                if (context.HttpContext.Options.UseReturnValuePropertyValidate == false)
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
            foreach (var filter in apiAction.FilterAttributes)
            {
                builder.Use(next => async context =>
                {
                    await filter.OnResponseAsync(context).ConfigureAwait(false);
                    await next(context).ConfigureAwait(false);
                });
            }

            return builder.Build();
        }
    }
}
