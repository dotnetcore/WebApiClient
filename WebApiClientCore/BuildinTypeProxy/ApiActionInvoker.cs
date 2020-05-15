using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
        private readonly ApiActionExecutionDelegate requestDelegate;

        public ApiActionInvoker(ApiActionDescriptor descriptor)
        {
            this.requestDelegate = CreateRequestDelegate(descriptor);
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
        private static ApiActionExecutionDelegate CreateRequestDelegate(ApiActionDescriptor descriptor)
        {
            var builder = new RequestDelegateBuilder()
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
               });

            foreach (var attr in descriptor.Attributes)
            {
                builder.Use(attr.BeforeRequestAsync);
            }

            foreach (var item in descriptor.Parameters)
            {
                foreach (var attr in item.Attributes)
                {
                    builder.Use(async (context, next) =>
                    {
                        var ctx = new ApiParameterContext(context, item.Index);
                        await attr.BeforeRequestAsync(ctx, next);
                    });
                }
            }

            foreach (var attr in descriptor.ResultAttributes)
            {
                builder.Use(attr.BeforeRequestAsync);
            }

            // 请求前特性的执行
            //builder.Then(async context =>
            //{
            //    var apiAction = context.ApiAction;
            //    //foreach (var actionAttribute in apiAction.Attributes)
            //    //{
            //    //    await actionAttribute.BeforeRequestAsync(context).ConfigureAwait(false);
            //    //}

            //    //foreach (var parameter in apiAction.Parameters)
            //    //{
            //    //    var parameterContext = new ApiParameterContext(context, parameter.Index);
            //    //    foreach (var parameterAttribute in parameter.Attributes)
            //    //    {
            //    //        await parameterAttribute.BeforeRequestAsync(parameterContext).ConfigureAwait(false);
            //    //    }
            //    //}
            //    await apiAction.Return.Attributes.BeforeRequestAsync(context).ConfigureAwait(false);
            //});


            // 请求前过滤器执行
            //.Then(async context =>
            //{
            //    foreach (var filter in context.ApiAction.Filters)
            //    {
            //        await filter.BeforeRequestAsync(context).ConfigureAwait(false);
            //    }
            //})

            foreach (var attr in descriptor.FilterAttributes)
            {
                builder.Use(attr.BeforeRequestAsync);
            }

            // 发起http请求
            builder.Then(async context =>
            {
                await HttpRequestAsync(context);
            });

            foreach (var attr in descriptor.ResultAttributes)
            {
                builder.Use(async (context, next) =>
                {
                    if (context.ResultStatus ==  ResultStatus.NoResult)
                    {
                        await attr.AfterRequestAsync(context, next);
                    }
                    else
                    {
                        await next();
                    }
                });
            }

            // 请求结束后过滤器执行
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
                try
                {
                    using var cancellation = CreateLinkedTokenSource(context);
                    context.HttpContext.ResponseMessage = await context.HttpContext.Client.SendAsync(context.HttpContext.RequestMessage, cancellation.Token).ConfigureAwait(false);

                    ApiValidator.ValidateReturnValue(context.Result, context.HttpContext.Options.UseReturnValuePropertyValidate);
                    await apiCache.SetAsync(cacheResult.CacheKey).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    context.Exception = ex;
                }
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
            private readonly ApiActionExecutionDelegate completedHandler;
            private readonly List<Func<ApiActionExecutionDelegate, ApiActionExecutionDelegate>> middlewares = new List<Func<ApiActionExecutionDelegate, ApiActionExecutionDelegate>>();

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
            public RequestDelegateBuilder(ApiActionExecutionDelegate completedHandler)
            {
                this.completedHandler = completedHandler;
            }

            /// <summary>
            /// 使用中间件
            /// </summary>
            /// <param name="middleware"></param>
            /// <returns></returns>
            public RequestDelegateBuilder Use(Func<ApiActionExecutionDelegate, ApiActionExecutionDelegate> middleware)
            {
                this.middlewares.Add(middleware);
                return this;
            }

            /// <summary>
            /// 使用中间件
            /// </summary>  
            /// <param name="middleware"></param>
            /// <returns></returns>
            public RequestDelegateBuilder Use(Func<ApiActionContext, Func<Task>, Task> middleware)
            {
                return this.Use(next => context => middleware(context, () => next(context)));
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
            public ApiActionExecutionDelegate Build()
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
