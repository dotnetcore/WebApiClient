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
    /// 表示Api请求的异步任务
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    class ApiTask<TResult> : ITask<TResult>
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
            return this.InvokeAsync().Cast<TResult>().GetAwaiter();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public Task<object> InvokeAsync()
        {
            return this.context.ExecuteAsync();
        }
    }
}
