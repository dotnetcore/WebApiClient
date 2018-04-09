using System;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// Rx扩展
    /// </summary>
    public static class ObservableExtend
    {
        /// <summary>
        /// 转换为ITaskObservable对象
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

        /// <summary>
        /// 转换为ITaskObservable对象
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
    }
}
