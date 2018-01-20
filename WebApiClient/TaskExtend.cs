using System;
using System.Threading.Tasks;
using WebApiClient.Interfaces;

namespace WebApiClient
{
    /// <summary>
    /// 提供ITask的扩展
    /// </summary>
    public static class TaskExtend
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
            return new ApiRetryTask<TResult>(task.InvokeAsync, maxCount, delay);
        }

        /// <summary>
        /// 返回提供异常处理请求任务对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IHandleTask<TResult> Handle<TResult>(this ITask<TResult> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return new ApiHandleTask<TResult>(task.InvokeAsync);
        }

        /// <summary>
        /// 当遇到异常时返回默认值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ITask<TResult> HandleAsDefaultWhenException<TResult>(this ITask<TResult> task)
        {
            return task.HandleAsDefaultWhenException(null);
        }

        /// <summary>
        /// 当遇到异常时返回默认值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="handler">异常处理委托</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ITask<TResult> HandleAsDefaultWhenException<TResult>(this ITask<TResult> task, Action<Exception> handler)
        {
            Func<Exception, TResult> func = ex =>
            {
                if (handler != null)
                {
                    handler(ex);
                }
                return default(TResult);
            };
            return task.Handle().WhenCatch<Exception>(func);
        }


        /// <summary>
        /// 使用工作线程包装请求并同步等待结果
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static TResult Wait<TResult>(this ITask<TResult> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return Task.Factory.StartNew(() => task.InvokeAsync().ConfigureAwait(false).GetAwaiter().GetResult()).Result;
        }
    }
}
