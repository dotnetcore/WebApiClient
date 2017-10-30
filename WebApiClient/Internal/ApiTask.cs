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
    /// 提供ApiTask的创建
    /// </summary>
    static class ApiTask
    {
        /// <summary>
        /// 创建ApiTask实例
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public static ITask CreateInstance(ApiActionContext context)
        {
            var ctor = context.ApiActionDescriptor.Return.ITaskCtor;
            return ctor.Invoke(new object[] { context }) as ITask;
        }
    }


    /// <summary>
    /// 表示Api请求的异步任务
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    class ApiTask<TResult> : ITask<TResult>, ITask
    {
        /// <summary>
        /// 上下文
        /// </summary>
        private readonly ApiActionContext context;

        /// <summary>
        /// Api请求的异步任务
        /// </summary>
        /// <param name="context">上下文</param>
        public ApiTask(ApiActionContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 返回TaskAwaiter对象
        /// </summary>
        /// <returns></returns>
        public TaskAwaiter<TResult> GetAwaiter()
        {
            return this.InvokeAsync().GetAwaiter();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public async Task<TResult> InvokeAsync()
        {
            return (TResult)await this.context.ExecuteAsync();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        Task ITask.InvokeAsync()
        {
            return this.InvokeAsync();
        }
    }
}
