using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示任务的Rx
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    class TaskObservable<TResult> : IObservable<TResult>
    {
        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// 观察者列表
        /// </summary>
        private readonly List<IObserver<TResult>> observerList = new List<IObserver<TResult>>();

        /// <summary>
        /// 任务的Rx
        /// </summary>
        /// <param name="task">任务</param>
        public TaskObservable(Task<TResult> task)
        {
            task.ContinueWith(t =>
            {
                switch (t.Status)
                {
                    case TaskStatus.RanToCompletion:
                        lock (this.syncRoot)
                        {
                            foreach (var observer in this.observerList)
                            {
                                observer.OnNext(t.Result);
                                observer.OnCompleted();
                            }
                            this.observerList.Clear();
                        }
                        break;

                    case TaskStatus.Faulted:
                        lock (this.syncRoot)
                        {
                            foreach (var observer in this.observerList)
                            {
                                observer.OnError(t.Exception);
                            }
                        }
                        break;

                    case TaskStatus.Canceled:
                        lock (this.syncRoot)
                        {
                            foreach (var observer in this.observerList)
                            {
                                observer.OnError(new TaskCanceledException(t));
                            }
                        }
                        break;
                }
            });
        }

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="observer">观察者</param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<TResult> observer)
        {
            lock (this.syncRoot)
            {
                this.observerList.Add(observer);
            }

            return new Unsubscriber<TResult>(() =>
            {
                lock (this.syncRoot)
                {
                    this.observerList.Remove(observer);
                }
            });
        }

        /// <summary>
        /// 表示订阅取消器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class Unsubscriber<T> : IDisposable
        {
            /// <summary>
            /// 取消委托
            /// </summary>
            private readonly Action onUnsubscribe;

            /// <summary>
            /// 订阅取消器
            /// </summary>
            /// <param name="onUnsubscribe">取消委托</param>
            public Unsubscriber(Action onUnsubscribe)
            {
                this.onUnsubscribe = onUnsubscribe;
            }

            /// <summary>
            /// 取消订阅
            /// </summary>
            public void Dispose()
            {
                this.onUnsubscribe.Invoke();
            }
        }
    }
}
