using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// ITask的扩展
    /// </summary>
    public static class ITaskExtend
    {
        /// <summary>
        /// 尝试重新请求
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="maxCount">最大重试次数</param>
        /// <returns></returns>
        public static IRetryTask<TResult> Retry<TResult>(this ITask<TResult> task, int maxCount)
        {
            return task.Retry(maxCount, null);
        }

        /// <summary>
        /// 尝试重新请求
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="maxCount">最大重试次数</param>
        /// <param name="delay">各次重试的延时时间</param>
        /// <returns></returns>
        public static IRetryTask<TResult> Retry<TResult>(this ITask<TResult> task, int maxCount, TimeSpan delay)
        {
            return task.Retry(maxCount, (i) => delay);
        }

        /// <summary>
        /// 尝试重新请求
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="maxCount">最大重试次数</param>
        /// <param name="delay">各次重试的延时时间</param>
        /// <returns></returns>
        public static IRetryTask<TResult> Retry<TResult>(this ITask<TResult> task, int maxCount, Func<int, TimeSpan> delay)
        {
            return new ApiRetryTask<TResult>(task.GetInvoker(), maxCount, delay);
        }
    }
}
