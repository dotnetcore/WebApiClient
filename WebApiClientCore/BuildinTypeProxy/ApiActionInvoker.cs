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
        /// 请求委托
        /// </summary>
        private readonly ExecutionDelegate handler;

        /// <summary>
        /// 上下文执行器
        /// </summary>
        /// <param name="descriptor"></param>
        public ApiActionInvoker(ApiActionDescriptor descriptor)
        {
            this.handler = CreateHandler(descriptor);
        }

        /// <summary>
        /// 执行Api方法
        /// </summary>
        /// <returns></returns>
        Task IApiActionInvoker.InvokeAsync(ApiActionContext context)
        {
            return this.InvokeAsync(context);
        }

        /// <summary>
        /// 执行Api方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<TResult> InvokeAsync(ApiActionContext context)
        {
            await handler(context);

            if (context.ResultStatus == ResultStatus.HasResult)
            {
                return (TResult)context.Result;
            }
            else if (context.ResultStatus == ResultStatus.HasException)
            {
                throw context.Exception;
            }

            throw new ApiResultNotSupportedExteption(context.HttpContext.ResponseMessage, context.ApiAction.Return.DataType.Type);
        }

        /// <summary>
        /// 创建请求委托
        /// </summary>
        /// <returns></returns>
        private static ExecutionDelegate CreateHandler(ApiActionDescriptor descriptor)
        {
            var builder = new PipelineBuilder();

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

            // 发起http请求
            builder.Use(next => async context =>
            {
                try
                {
                    await HttpRequestAsync(context);
                }
                catch (Exception ex)
                {
                    context.Exception = ex;
                }
                finally
                {
                    await next(context);
                }
            });

            // 结果特性请求后执行
            foreach (var attr in descriptor.ResultAttributes)
            {
                builder.Use(async (context, next) =>
                {
                    if (context.ResultStatus == ResultStatus.NoResult)
                    {
                        await attr.AfterRequestAsync(context, next);
                    }
                    else
                    {
                        await next();
                    }
                });
            }

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
        private static async Task HttpRequestAsync(ApiActionContext context)
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

                ApiValidator.ValidateReturnValue(context.Result, context.HttpContext.Options.UseReturnValuePropertyValidate);
                await apiCache.SetAsync(cacheResult.CacheKey).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 创建取消令牌源
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static CancellationTokenSource CreateLinkedTokenSource(ApiActionContext context)
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
