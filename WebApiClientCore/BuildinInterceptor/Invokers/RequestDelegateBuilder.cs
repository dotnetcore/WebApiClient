using System;
using System.Linq;
using System.Threading;
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
                var response = await SendHttpRequestAsync(request).ConfigureAwait(false);
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
                    ApiValidator.ValidateParameter(parameter, parameterValue, validateProperty);
                }
                return next(context);
            });

            // action特性请求前执行
            foreach (var attr in apiAction.Attributes)
            {
                builder.Use(attr.OnRequestAsync);
            }

            // 参数特性请求前执行
            foreach (var parameter in apiAction.Parameters)
            {
                var index = parameter.Index;
                foreach (var attr in parameter.Attributes)
                {
                    builder.Use(async (context, next) =>
                    {
                        var ctx = new ApiParameterContext(context, index);
                        await attr.OnRequestAsync(ctx, next).ConfigureAwait(false);
                    });
                }
            }

            // Return特性请求前执行
            foreach (var @return in apiAction.Return.Attributes)
            {
                if (@return.Enable == true)
                {
                    builder.Use(@return.OnRequestAsync);
                }
            }

            // Filter请求前执行            
            foreach (var filter in apiAction.FilterAttributes)
            {
                if (filter.Enable == true)
                {
                    builder.Use(filter.OnRequestAsync);
                }
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
                if (@return.Enable == false)
                {
                    continue;
                }

                builder.Use(async (context, next) =>
                {
                    if (context.ResultStatus == ResultStatus.None)
                    {
                        await @return.OnResponseAsync(context, next).ConfigureAwait(false);
                    }
                    else
                    {
                        await next().ConfigureAwait(false);
                    }
                });
            }

            // 验证Result是否ok
            builder.Use(next => context =>
            {
                try
                {
                    ApiValidator.ValidateReturnValue(context.Result, context.HttpContext.Options.UseReturnValuePropertyValidate);
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
                if (filter.Enable == true)
                {
                    builder.Use(filter.OnResponseAsync);
                }
            }

            return builder.Build();
        }


        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task<ApiResponseContext> SendHttpRequestAsync(ApiRequestContext context)
        {
            try
            {
                var apiCache = new ApiCache(context);
                var cacheValue = await apiCache.GetAsync().ConfigureAwait(false);

                if (cacheValue != null && cacheValue.Value != null)
                {
                    context.HttpContext.ResponseMessage = cacheValue.Value;
                }
                else
                {
                    using var cancellation = CreateLinkedTokenSource(context);
                    var response = await context.HttpContext.Client.SendAsync(context.HttpContext.RequestMessage, cancellation.Token).ConfigureAwait(false);

                    context.HttpContext.ResponseMessage = response;
                    await apiCache.SetAsync(cacheValue?.Key, response).ConfigureAwait(false);
                }
                return new ApiResponseContext(context);
            }
            catch (Exception ex)
            {
                return new ApiResponseContext(context) { Exception = ex };
            }
        }

        /// <summary>
        /// 创建取消令牌源
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static CancellationTokenSource CreateLinkedTokenSource(ApiRequestContext context)
        {
            if (context.CancellationTokens.Count == 0)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);
            }
            else
            {
                var tokens = context.CancellationTokens.ToArray();
                return CancellationTokenSource.CreateLinkedTokenSource(tokens);
            }
        }
    }
}
