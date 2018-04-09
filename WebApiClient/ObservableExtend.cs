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
        /// 转换为Observable对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IObservable<TResult> ToObservable<TResult>(this ITask<TResult> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return task.InvokeAsync().ToObservable();
        }

        /// <summary>
        /// 转换为Observable对象
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="task"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IObservable<TResult> ToObservable<TResult>(this Task<TResult> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }
            return new TaskObservable<TResult>(task);
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="observable"></param>
        /// <param name="onResult">收到结果委托</param>
        /// <param name="onError">遇到错误委托</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IDisposable Subscribe<TResult>(this IObservable<TResult> observable, Action<TResult> onResult, Action<Exception> onError)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            var observer = new TaskObserver<TResult>(onResult, onError, null);
            return observable.Subscribe(observer);
        }
    }
}
