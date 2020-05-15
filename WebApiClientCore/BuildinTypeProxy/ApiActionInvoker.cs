using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 上下文执行器
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    class ApiActionInvoker<TResult> : IApiActionInvoker
    {
        /// <summary>
        /// 执行委托
        /// </summary>
        private readonly Func<ApiRequestContext, Task<ApiResponseContext>> handler;

        /// <summary>
        /// 上下文执行器
        /// </summary>
        /// <param name="descriptor"></param>
        public ApiActionInvoker(ApiActionDescriptor descriptor)
        {
            this.handler = CreateExecutionHandler(descriptor);
        }

        /// <summary>
        /// 执行Api方法
        /// </summary>
        /// <returns></returns>
        Task IApiActionInvoker.InvokeAsync(ApiRequestContext context)
        {
            return this.InvokeAsync(context);
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


        /// <summary>
        /// 创建执行委托
        /// </summary>
        /// <returns></returns>
        private static Func<ApiRequestContext, Task<ApiResponseContext>> CreateExecutionHandler(ApiActionDescriptor descriptor)
        {
            var requestHandler = BuildRequestHandler(descriptor);
            var responseHandler = BuildResponseHandler(descriptor);

            return async context =>
            {
                await requestHandler(context);
                var response = await ExecuteApiAsync(context);
                await responseHandler(response);
                return response;
            };
        }


        /// <summary>
        /// 创建请求委托
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        private static InvokeDelegate<ApiRequestContext> BuildRequestHandler(ApiActionDescriptor descriptor)
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
            foreach (var attr in descriptor.Attributes)
            {
                builder.Use(attr.BeforeRequestAsync);
            }

            // 参数特性请求前执行
            foreach (var parameter in descriptor.Parameters)
            {
                foreach (var attr in parameter.Attributes)
                {
                    builder.Use(async (context, next) =>
                    {
                        var ctx = new ApiParameterContext(context, parameter.Index);
                        await attr.BeforeRequestAsync(ctx, next);
                    });
                }
            }

            // 结果特性请求前执行
            foreach (var attr in descriptor.ResultAttributes)
            {
                builder.Use(attr.BeforeRequestAsync);
            }

            // 过滤器请求前执行            
            foreach (var attr in descriptor.FilterAttributes)
            {
                builder.Use(attr.BeforeRequestAsync);
            }

            return builder.Build();
        }

        /// <summary>
        /// 创建响应委托
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        private static InvokeDelegate<ApiResponseContext> BuildResponseHandler(ApiActionDescriptor descriptor)
        {
            var builder = new PipelineBuilder<ApiResponseContext>();

            // 结果特性请求后执行
            foreach (var attr in descriptor.ResultAttributes)
            {
                builder.Use(async (context, next) =>
                {
                    // 有结果值 本中间件就不再处理
                    if (context.ResultStatus != ResultStatus.NoResult)
                    {
                        await next();
                        return;
                    }

                    try
                    {
                        await attr.AfterRequestAsync(context, next);
                    }
                    catch (Exception ex)
                    {
                        context.Exception = ex;
                        await next();
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

            // 过滤器请求后执行
            foreach (var attr in descriptor.FilterAttributes)
            {
                builder.Use(attr.AfterRequestAsync);
            }

            return builder.Build();
        }


        /// <summary>
        /// 执行http请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task<ApiResponseContext> ExecuteApiAsync(ApiRequestContext context)
        {
            var response = new ApiResponseContext(context);
            try
            {
                await SendRequestAsync(context);
            }
            catch (Exception ex)
            {
                response.Exception = ex;
            }
            return response;
        }

        /// <summary>
        /// 发送http请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task SendRequestAsync(ApiRequestContext context)
        {
            var apiCache = new ApiCache(context);
            var cacheResult = await apiCache.GetAsync().ConfigureAwait(false);

            if (cacheResult.ResponseMessage != null)
            {
                context.HttpContext.ResponseMessage = cacheResult.ResponseMessage;
            }
            else
            {
                using var cancellation = CreateLinkedTokenSource(context);
                context.HttpContext.ResponseMessage = await context.HttpContext.Client.SendAsync(context.HttpContext.RequestMessage, cancellation.Token).ConfigureAwait(false);
                await apiCache.SetAsync(cacheResult.CacheKey).ConfigureAwait(false);
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
