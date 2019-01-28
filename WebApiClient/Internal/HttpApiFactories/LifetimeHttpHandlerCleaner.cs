using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示LifetimeHttpHandler清理器
    /// </summary>
    class LifetimeHttpHandlerCleaner
    {
        /// <summary>
        /// 当前监视生命周期的记录的数量
        /// </summary>
        private int trackingEntryCount = 0;

        /// <summary>
        /// 监视生命周期的记录队列
        /// </summary>
        private readonly ConcurrentQueue<TrackingEntry> trackingEntries = new ConcurrentQueue<TrackingEntry>();

        /// <summary>
        /// 获取或设置清理的时间间隔
        /// 默认10s
        /// </summary>
        public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromSeconds(10d);

        /// <summary>
        /// 添加要清除的httpHandler
        /// </summary>
        /// <param name="handler">httpHandler</param>
        public void Add(LifetimeHttpHandler handler)
        {
            var entry = new TrackingEntry(handler);
            this.trackingEntries.Enqueue(entry);

            // 从0变为1，要启动清理作业
            if (Interlocked.Increment(ref this.trackingEntryCount) == 1)
            {
                this.StartCleanup();
            }
        }

        /// <summary>
        /// 启动清理作业
        /// </summary>
        private async void StartCleanup()
        {
            do
            {
                await Task
                    .Delay(this.CleanupInterval)
                    .ConfigureAwait(false);
            }
            while (this.Cleanup() == false);
        }

        /// <summary>
        /// 清理失效的拦截器
        /// 返回是否完全清理
        /// </summary>
        /// <returns></returns>
        private bool Cleanup()
        {
            var cleanCount = this.trackingEntries.Count;
            for (var i = 0; i < cleanCount; i++)
            {
                this.trackingEntries.TryDequeue(out var entry);
                if (entry.CanDispose == false)
                {
                    this.trackingEntries.Enqueue(entry);
                    continue;
                }

                entry.Dispose();
                if (Interlocked.Decrement(ref this.trackingEntryCount) == 0)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 表示监视生命周期的记录
        /// </summary>
        private class TrackingEntry : IDisposable
        {
            /// <summary>
            /// 用于释放资源的对象
            /// </summary>
            private readonly IDisposable disposable;

            /// <summary>
            /// 监视对象的弱引用
            /// </summary>
            private readonly WeakReference weakReference;

            /// <summary>
            /// 获取是否可以释放资源
            /// </summary>
            /// <returns></returns>
            public bool CanDispose
            {
                get => this.weakReference.IsAlive == false;
            }

            /// <summary>
            /// 监视生命周期的记录
            /// </summary>
            /// <param name="handler">激活状态的httpHandler</param>
            public TrackingEntry(LifetimeHttpHandler handler)
            {
                this.disposable = handler.InnerHandler;
                this.weakReference = new WeakReference(handler);
            }

            /// <summary>
            /// 释放资源
            /// </summary>
            public void Dispose()
            {
                if (this.CanDispose == true)
                {
                    this.disposable.Dispose();
                }
            }
        }
    }
}
