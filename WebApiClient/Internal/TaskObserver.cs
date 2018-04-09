using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示任务的观察者
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    class TaskObserver<TResult> : IObserver<TResult>
    {
        /// <summary>
        /// 有值更新委托
        /// </summary>
        private readonly Action<TResult> onNext;

        /// <summary>
        /// 错误委托
        /// </summary>
        private readonly Action<Exception> onError;

        /// <summary>
        /// 完成委托
        /// </summary>
        private readonly Action onCompleted;

        /// <summary>
        /// 任务的观察者
        /// </summary>
        /// <param name="onNext">有值更新委托</param>
        /// <param name="onError">错误委托</param>
        /// <param name="onCompleted">完成委托</param>
        public TaskObserver(Action<TResult> onNext, Action<Exception> onError, Action onCompleted)
        {
            this.onNext = onNext;
            this.onError = onError;
            this.onCompleted = onCompleted;
        }

        /// <summary>
        /// 完成触发
        /// </summary>
        public void OnCompleted()
        {
            this.onCompleted?.Invoke();
        }

        /// <summary>
        /// 错误触发
        /// </summary>
        /// <param name="error"></param>
        public void OnError(Exception error)
        {
            this.onError?.Invoke(error);
        }

        /// <summary>
        /// 有值更新触发
        /// </summary>
        /// <param name="value"></param>
        public void OnNext(TResult value)
        {
            this.onNext?.Invoke(value);
        }
    }
}
