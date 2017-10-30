using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义返回任务的行为
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        Task InvokeAsync();
    }

    /// <summary>
    /// 定义返回任务的行为
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ITask<TResult> : ITask
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        Task<TResult> InvokeAsync();

        /// <summary>
        /// 返回TaskAwaiter对象
        /// </summary>
        /// <returns></returns>
        TaskAwaiter<TResult> GetAwaiter();
    }
}
