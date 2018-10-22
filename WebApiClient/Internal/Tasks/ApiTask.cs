using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 提供ApiTask的创建
    /// </summary>
    abstract class ApiTask : ITask
    {
#if NET45
        /// <summary>
        /// 完成的任务
        /// </summary>
        /// <returns></returns>
        public static readonly Task CompletedTask = Task.FromResult<object>(null);
#else
        /// <summary>
        /// 完成的任务
        /// </summary>
        /// <returns></returns>
        public static readonly Task CompletedTask = Task.CompletedTask;
#endif

        /// <summary>
        /// 获取ITaskOf(dataType)的构造器
        /// </summary>
        /// <param name="dataType">泛型参数类型</param>
        /// <returns></returns>
        public static ConstructorInfo GetITaskConstructor(Type dataType)
        {
            return typeof(ApiTaskOf<>)
                .MakeGenericType(dataType)
                .GetConstructor(new[] { typeof(IHttpApi), typeof(HttpApiConfig), typeof(ApiActionDescriptor) });
        }

        /// <summary>
        /// 创建ApiTaskOf(T)的实例
        /// </summary>
        /// <param name="httpApi">httpApi代理类实例</param>
        /// <param name="httpApiConfig">http接口配置</param>
        /// <param name="apiActionDescriptor">api描述</param>
        /// <returns></returns>
        public static ApiTask CreateInstance(IHttpApi httpApi, HttpApiConfig httpApiConfig, ApiActionDescriptor apiActionDescriptor)
        {
            // var instance = new ApiTask<TResult>(httpApi, httpApiConfig, apiActionDescriptor);
            var ctor = apiActionDescriptor.Return.DataType.ITaskConstructor;
            return ctor.Invoke(new object[] { httpApi, httpApiConfig, apiActionDescriptor }) as ApiTask;
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
            /// httpApi代理类实例
            /// </summary>
            private readonly IHttpApi httpApi;

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
            /// <param name="httpApi">httpApi代理类实例</param>
            /// <param name="httpApiConfig">http接口配置</param>
            /// <param name="apiActionDescriptor">api描述</param>
            public ApiTaskOf(IHttpApi httpApi, HttpApiConfig httpApiConfig, ApiActionDescriptor apiActionDescriptor)
            {
                this.httpApi = httpApi;
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
            /// 配置用于等待的等待者
            /// </summary>
            /// <param name="continueOnCapturedContext">试图继续回夺取的原始上下文，则为 true；否则为 false</param>
            /// <returns></returns>
            public ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
            {
                return this.RequestAsync().ConfigureAwait(continueOnCapturedContext);
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
                    HttpApi = this.httpApi,
                    HttpApiConfig = this.httpApiConfig,
                    ApiActionDescriptor = this.apiActionDescriptor,
                    RequestMessage = new HttpApiRequestMessage { RequestUri = this.httpApiConfig.HttpHost }
                };

                return await context.ExecuteActionAsync<TResult>().ConfigureAwait(false);
            }
        }
    }
}
