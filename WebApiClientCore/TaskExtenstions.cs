using System;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// ITask扩展
    /// </summary>
    public static class TaskExtenstions
    {
        /// <summary>
        /// 返回提供请求重试的请求任务对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="maxCount">最大重试次数</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IRetryTask<TResult> Retry<TResult>(this ITask<TResult> task, int maxCount)
        {
            return task.Retry(maxCount, null);
        }

        /// <summary>
        /// 返回提供请求重试的请求任务对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="maxCount">最大重试次数</param>
        /// <param name="delay">各次重试的延时时间</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IRetryTask<TResult> Retry<TResult>(this ITask<TResult> task, int maxCount, TimeSpan delay)
        {
            return task.Retry(maxCount, (i) => delay);
        }

        /// <summary>
        /// 返回提供请求重试的请求任务对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="maxCount">最大重试次数</param>
        /// <param name="delay">各次重试的延时时间</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IRetryTask<TResult> Retry<TResult>(this ITask<TResult> task, int maxCount, Func<int, TimeSpan> delay)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (maxCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount));
            }
            return new ActionRetryTask<TResult>(task.InvokeAsync, maxCount, delay);
        }

        /// <summary>
        /// 当请求异常时返回默认值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static async Task<T> ExceptionThenDefault<T>(this ITask<T> task)
        {
            try
            {
                return await task.ConfigureAwait(false);
            }
            catch (Exception)
            {
                return default;
            }
        }
    }
}
