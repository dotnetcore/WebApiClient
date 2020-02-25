using System;

namespace WebApiClient
{
    /// <summary>
    /// Define behaviors that support observation tasks
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ITaskObservable<TResult> : IObservable<TResult>
    {
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="onResult">Receive results commission</param>
        /// <param name="onError">Encountered error delegation</param>
        /// <returns></returns>
        IDisposable Subscribe(Action<TResult> onResult, Action<Exception> onError);
    }
}
