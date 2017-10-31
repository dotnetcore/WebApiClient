using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义返回结果的行为
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ITask<TResult>
    {
        /// <summary>
        /// 创建请求任务
        /// </summary>
        /// <returns></returns>
        Task<TResult> InvokeAsync();

        /// <summary>
        /// 调用InvokeAsync
        /// 并返回其TaskAwaiter对象
        /// </summary>
        /// <returns></returns>
        TaskAwaiter<TResult> GetAwaiter();
    }
}
