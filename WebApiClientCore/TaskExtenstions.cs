using System;

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
        public static IRetryTask<TResult> Retry<TResult>(this ITask<TResult> task, int maxCount, Func<int, TimeSpan>? delay)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            if (maxCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCount));
            }
            return new AcitonRetryTask<TResult>(async () => await task, maxCount, delay);
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
#nullable disable
            return task.Handle().WhenCatch<Exception>(ex => default);
#nullable enable
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
            return new ActionHandleTask<TResult>(async () => await task);
        }
    }
}
