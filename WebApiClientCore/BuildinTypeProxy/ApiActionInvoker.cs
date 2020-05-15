using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示所有中间件执行委托
    /// </summary> 
    /// <param name="context">中间件上下文</param>
    /// <returns></returns>
    delegate Task RequestDelegate(ApiActionContext context);

    /// <summary>
    /// 上下文执行器
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    class ApiActionInvoker<TResult> : IApiActionInvoker
    {
        /// <summary>
        /// 请求委托
        /// </summary>
        private static RequestDelegate requestDelegate = CreateRequestDelegate();

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
            await requestDelegate(context);
            if (context.Exception != null)
            {
                throw context.Exception;
            }
            return (TResult)context.Result;
        }

        /// <summary>
        /// 创建请求委托
        /// </summary>
        /// <returns></returns>
        private static RequestDelegate CreateRequestDelegate()
        {
            return new RequestDelegateBuilder()
               // 参数验证特性验证和参数模型属性特性验证
               .Then(context =>
               {
                   var validateProperty = context.HttpContext.Options.UseParameterPropertyValidate;
                   foreach (var parameter in context.ApiAction.Parameters)
                   {
                       var parameterValue = context.Arguments[parameter.Index];
                       ApiValidator.ValidateParameter(parameter, parameterValue, validateProperty);
                   }
                   return Task.CompletedTask;
               })
               // 请求前特性的执行
               .Then(async context =>
               {
                   var apiAction = context.ApiAction;
                   foreach (var actionAttribute in apiAction.Attributes)
                   {
                       await actionAttribute.BeforeRequestAsync(context).ConfigureAwait(false);
                   }

                   foreach (var parameter in apiAction.Parameters)
                   {
                       var parameterContext = new ApiParameterContext(context, parameter.Index);
                       foreach (var parameterAttribute in parameter.Attributes)
                       {
                           await parameterAttribute.BeforeRequestAsync(parameterContext).ConfigureAwait(false);
                       }
                   }
                   await apiAction.Return.Attribute.BeforeRequestAsync(context).ConfigureAwait(false);
               })
               // 请求前过滤器执行
               .Then(async context =>
               {
                   foreach (var filter in context.ApiAction.Filters)
                   {
                       await filter.BeforeRequestAsync(context).ConfigureAwait(false);
                   }
               })
               // 发起http请求
               .Then(async context =>
               {
                   try
                   {
                       await HttpRequestAsync(context);
                   }
                   catch (Exception ex)
                   {
                       context.Exception = ex;
                   }
               })
               // 请求结束后过滤器执行
               .Then(async context =>
               {
                   foreach (var filter in context.ApiAction.Filters)
                   {
                       await filter.AfterRequestAsync(context).ConfigureAwait(false);
                   }
               })
               .Build();
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
                context.Result = await context.ApiAction.Return.Attribute.GetResultAsync(context).ConfigureAwait(false);
            }
            else
            {
                using var cancellation = CreateLinkedTokenSource(context);
                context.HttpContext.ResponseMessage = await context.HttpContext.Client.SendAsync(context.HttpContext.RequestMessage, cancellation.Token).ConfigureAwait(false);
                context.Result = await context.ApiAction.Return.Attribute.GetResultAsync(context).ConfigureAwait(false);

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

        /// <summary>
        /// 表示中间件创建者
        /// </summary>
        private class RequestDelegateBuilder
        {
            private readonly RequestDelegate completedHandler;
            private readonly List<Func<RequestDelegate, RequestDelegate>> middlewares = new List<Func<RequestDelegate, RequestDelegate>>();

            /// <summary>
            /// 中间件创建者
            /// </summary> 
            public RequestDelegateBuilder()
                : this(context => Task.CompletedTask)
            {
            }

            /// <summary>
            /// 中间件创建者
            /// </summary> 
            /// <param name="completedHandler">完成执行内容处理者</param>
            public RequestDelegateBuilder(RequestDelegate completedHandler)
            {
                this.completedHandler = completedHandler;
            }

            /// <summary>
            /// 使用中间件
            /// </summary>
            /// <param name="middleware"></param>
            /// <returns></returns>
            public RequestDelegateBuilder Use(Func<RequestDelegate, RequestDelegate> middleware)
            {
                this.middlewares.Add(middleware);
                return this;
            }

            /// <summary>
            /// 使用中间件
            /// </summary>
            /// <param name="handler"></param>
            /// <returns></returns>
            public RequestDelegateBuilder Then(Func<ApiActionContext, Task> handler)
            {
                return this.Use(next => async context =>
                {
                    await handler(context);
                    await next(context);
                });
            }


            /// <summary>
            /// 创建所有中间件执行处理者
            /// </summary>
            /// <returns></returns>
            public RequestDelegate Build()
            {
                var handler = this.completedHandler;
                for (var i = this.middlewares.Count - 1; i >= 0; i--)
                {
                    handler = this.middlewares[i](handler);
                }
                return handler;
            }
        }
    }
}
