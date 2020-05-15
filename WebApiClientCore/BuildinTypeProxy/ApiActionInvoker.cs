using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示ApiAction执行器
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    class ApiActionInvoker<TResult> : IApiActionInvoker
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
        public ApiActionInvoker(ApiActionDescriptor apiAction)
        {
            this.apiAction = apiAction;
            this.handler = CreateExecutionHandler(apiAction);
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        object IApiActionInvoker.Invoke(ServiceContext context, object[] arguments)
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


        /// <summary>
        /// 创建执行委托
        /// </summary>
        /// <returns></returns>
        private static Func<ApiRequestContext, Task<ApiResponseContext>> CreateExecutionHandler(ApiActionDescriptor apiAction)
        {
            var requestHandler = BuildRequestHandler(apiAction);
            var responseHandler = BuildResponseHandler(apiAction);

            return async request =>
            {
                await requestHandler(request);
                var response = await SendRequestAsync(request);
                await responseHandler(response);
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
                foreach (var attr in parameter.Attributes)
                {
                    builder.Use(async (context, next) =>
                    {
                        var ctx = new ApiParameterContext(context, parameter.Index);
                        await attr.OnRequestAsync(ctx, next);
                    });
                }
            }

            // 结果特性请求前执行
            foreach (var result in apiAction.ResultAttributes)
            {
                builder.Use(result.OnRequestAsync);
            }

            // 过滤器请求前执行            
            foreach (var filter in apiAction.FilterAttributes)
            {
                builder.Use(filter.OnRequestAsync);
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

            // 结果特性请求后执行
            foreach (var result in apiAction.ResultAttributes)
            {
                builder.Use(result.OnResponseAsync);
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
            foreach (var filter in apiAction.FilterAttributes)
            {
                builder.Use(filter.OnResponseAsync);
            }

            return builder.Build();
        }


        /// <summary>
        /// 执行http请求
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task<ApiResponseContext> SendRequestAsync(ApiRequestContext context)
        {
            var response = new ApiResponseContext(context);
            try
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
            catch (Exception ex)
            {
                response.Exception = ex;
            }
            return response;
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
