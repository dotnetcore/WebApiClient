using System;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// Provide project related extensions
    /// </summary>
    public static partial class Extension
    {
        /// <summary>
        /// Returns the request task object providing the request retry
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="maxCount">Maximum number of retries</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IRetryTask<TResult> Retry<TResult>(this ITask<TResult> task, int maxCount)
        {
            return task.Retry(maxCount, null);
        }

        /// <summary>
        /// Returns the request task object providing the request retry
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="maxCount">Maximum number of retries</param>
        /// <param name="delay">Delay time for each retry</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IRetryTask<TResult> Retry<TResult>(this ITask<TResult> task, int maxCount, TimeSpan delay)
        {
            return task.Retry(maxCount, (i) => delay);
        }

        /// <summary>
        /// Returns the request task object providing the request retry
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="maxCount">Maximum number of retries</param>
        /// <param name="delay">Delay time for each retry</param>
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
        /// Returns a task object that provides exception handling requests
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
        /// Returns the default value when an exception is encountered
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
        /// Returns the default value when an exception is encountered
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <param name="handler">Exception Handling Delegate</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ITask<TResult> HandleAsDefaultWhenException<TResult>(this ITask<TResult> task, Action<Exception> handler)
        {
            TResult func(Exception ex)
            {
                handler?.Invoke(ex);
                return default(TResult);
            }
            return task.Handle().WhenCatch<Exception>(func);
        }

        /// <summary>
        /// Converted to ITaskObservable object
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ITaskObservable<TResult> ToObservable<TResult>(this Task<TResult> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return new TaskObservable<TResult>(task);
        }

        /// <summary>
        /// Converted to ITaskObservable object
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ITaskObservable<TResult> ToObservable<TResult>(this ITask<TResult> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.InvokeAsync().ToObservable();
        }
    }
}
