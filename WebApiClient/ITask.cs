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
    /// <typeparam name="TResult"></typeparam>
    public interface ITask<TResult>
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        Task<object> InvokeAsync();

        /// <summary>
        /// 返回TaskAwaiter对象
        /// </summary>
        /// <returns></returns>
        TaskAwaiter<TResult> GetAwaiter();
    }
}
