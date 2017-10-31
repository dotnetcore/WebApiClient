using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义返回结果的行为
    /// </summary>
    interface ITask
    {
        /// <summary>
        /// 创建请求任务
        /// </summary>
        /// <returns></returns>
        Task InvokeAsync();
    }

    /// <summary>
    /// 提供ApiTask的创建
    /// </summary>
    static class ApiTask
    {
        /// <summary>
        /// 完成的任务
        /// </summary>
        /// <returns></returns>
        public static readonly Task CompletedTask = Task.FromResult<object>(null);

        /// <summary>
        /// 创建ApiTask的实例
        /// </summary>
        /// <param name="httpApiConfig">http接口配置</param>
        /// <param name="apiActionDescriptor">api描述</param>
        /// <returns></returns>
        public static ITask CreateInstance(HttpApiConfig httpApiConfig, ApiActionDescriptor apiActionDescriptor)
        {
            // var instance = new ApiTask<TResult>(httpApiConfig, apiActionDescriptor);
            var ctor = apiActionDescriptor.Return.ITaskCtor;
            return ctor.Invoke(new object[] { httpApiConfig, apiActionDescriptor }) as ITask;
        }
    }


    /// <summary>
    /// 表示Api请求的异步任务
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    sealed class ApiTask<TResult> : ITask<TResult>, ITask
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
        public ApiTask(HttpApiConfig httpApiConfig, ApiActionDescriptor apiActionDescriptor)
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
            return this.InvokeAsync().GetAwaiter();
        }

        /// <summary>
        /// 创建请求任务
        /// </summary>
        /// <returns></returns>
        public async Task<TResult> InvokeAsync()
        {
            var context = new ApiActionContext
            {
                ApiActionDescriptor = this.apiActionDescriptor,
                HttpApiConfig = this.httpApiConfig,
                RequestMessage = new HttpApiRequestMessage { RequestUri = this.httpApiConfig.HttpHost },
                ResponseMessage = null
            };
            return (TResult)await this.apiActionDescriptor.ExecuteAsync(context);
        }

        /// <summary>
        /// 创建请求任务
        /// </summary>
        /// <returns></returns>
        Task ITask.InvokeAsync()
        {
            return this.InvokeAsync();
        }
    }
}
