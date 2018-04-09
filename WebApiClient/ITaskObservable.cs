using System;

namespace WebApiClient
{
    /// <summary>
    /// 定义支持观察的任务的行为
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ITaskObservable<TResult> : IObservable<TResult>
    {
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="onResult">收到结果委托</param>
        /// <param name="onError">遇到错误委托</param>
        /// <returns></returns>
        IDisposable Subscribe(Action<TResult> onResult, Action<Exception> onError);
    }
}
