using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 提供ApiTask的创建
    /// </summary>
    abstract class ApiTask
    {
        /// <summary>
        /// 完成的任务
        /// </summary>
        /// <returns></returns>
        public static readonly Task CompletedTask = Task.FromResult<object>(null);

        /// <summary>
        /// 获取泛型构造器
        /// </summary>
        /// <param name="dataType">泛型参数类型</param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(Type dataType)
        {
            return typeof(ApiTaskOf<>)
                .MakeGenericType(dataType)
                .GetConstructor(new[] { typeof(HttpApiConfig), typeof(ApiActionDescriptor) });
        }

        /// <summary>
        /// 创建ApiTaskOf(T)的实例
        /// </summary>
        /// <param name="httpApiConfig">http接口配置</param>
        /// <param name="apiActionDescriptor">api描述</param>
        /// <returns></returns>
        public static ApiTask CreateInstance(HttpApiConfig httpApiConfig, ApiActionDescriptor apiActionDescriptor)
        {
            // var instance = new ApiTask<TResult>(httpApiConfig, apiActionDescriptor);
            var ctor = apiActionDescriptor.Return.ITaskCtor;
            return ctor.Invoke(new object[] { httpApiConfig, apiActionDescriptor }) as ApiTask;
        }

        /// <summary>
        /// 创建请求任务
        /// 返回请求结果
        /// </summary>
        /// <returns></returns>
        public abstract Task InvokeAsync();


        /// <summary>
        /// 表示Api请求的异步任务
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        private class ApiTaskOf<TResult> : ApiTask, ITask<TResult>
        {
            /// <summary>
            /// http接口配置
            /// </summary>
            private readonly HttpApiConfig httpApiConfig;

            /// <summary>
            /// api描述
            /// </summary>
            private readonly ApiActionDescriptor apiActionDescriptor;

            /// <summary>
            /// Api请求的异步任务
            /// </summary>
            /// <param name="httpApiConfig">http接口配置</param>
            /// <param name="apiActionDescriptor">api描述</param>
            public ApiTaskOf(HttpApiConfig httpApiConfig, ApiActionDescriptor apiActionDescriptor)
            {
                this.httpApiConfig = httpApiConfig;
                this.apiActionDescriptor = apiActionDescriptor;
            }

            /// <summary>
            /// 执行InvokeAsync
            /// 并返回其TaskAwaiter对象
            /// </summary>
            /// <returns></returns>
            public TaskAwaiter<TResult> GetAwaiter()
            {
                return this.RequestAsync().GetAwaiter();
            }

            /// <summary>
            /// 创建请求任务
            /// </summary>
            /// <returns></returns>
            public override Task InvokeAsync()
            {
                return this.RequestAsync();
            }

            /// <summary>
            /// 创建请求任务
            /// </summary>
            /// <returns></returns>
            Task<TResult> ITask<TResult>.InvokeAsync()
            {
                return this.RequestAsync();
            }

            /// <summary>
            /// 执行一次请求
            /// </summary>
            /// <returns></returns>
            private async Task<TResult> RequestAsync()
            {
                var context = new ApiActionContext
                {
                    ApiActionDescriptor = this.apiActionDescriptor,
                    HttpApiConfig = this.httpApiConfig,
                    RequestMessage = new HttpApiRequestMessage { RequestUri = this.httpApiConfig.HttpHost },
                    ResponseMessage = null
                };
                var result = await this.RequestAsync(context);
                return (TResult)result;
            }

            /// <summary>
            /// 异步执行http请求
            /// </summary>
            /// <param name="context">上下文</param>
            /// <returns></returns>
            private async Task<object> RequestAsync(ApiActionContext context)
            {
                var apiAction = context.ApiActionDescriptor;
                var globalFilters = context.HttpApiConfig.GlobalFilters;

                foreach (var actionAttribute in apiAction.Attributes)
                {
                    await actionAttribute.BeforeRequestAsync(context);
                }

                foreach (var parameter in apiAction.Parameters)
                {
                    foreach (var parameterAttribute in parameter.Attributes)
                    {
                        await parameterAttribute.BeforeRequestAsync(context, parameter);
                    }
                }

                foreach (var filter in globalFilters)
                {
                    await filter.OnBeginRequestAsync(context);
                }

                foreach (var filter in apiAction.Filters)
                {
                    await filter.OnBeginRequestAsync(context);
                }

                var client = context.HttpApiConfig.HttpClient;
                context.ResponseMessage = await client.SendAsync(context.RequestMessage);

                foreach (var filter in globalFilters)
                {
                    await filter.OnEndRequestAsync(context);
                }

                foreach (var filter in apiAction.Filters)
                {
                    await filter.OnEndRequestAsync(context);
                }

                return await apiAction.Return.Attribute.GetTaskResult(context);
            }
        }
    }
}
