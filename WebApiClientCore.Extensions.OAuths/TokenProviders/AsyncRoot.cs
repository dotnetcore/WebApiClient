using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore.Extensions.OAuths.TokenProviders
{
    /// <summary>
    /// 提供异步锁
    /// </summary>
    class AsyncRoot : Disposable
    {
        /// <summary>
        /// 信号量
        /// </summary>
        private readonly SemaphoreSlim semaphoreSlim;

        /// <summary>
        /// 异步锁
        /// </summary>
        public AsyncRoot()
            : this(1)
        {
        }

        /// <summary>
        /// 异步锁
        /// </summary>
        /// <param name="concurrent">允许并行的线程数</param>
        public AsyncRoot(int concurrent)
        {
            this.semaphoreSlim = new SemaphoreSlim(concurrent, concurrent);
        }

        /// <summary>
        /// 锁住代码块
        /// using( asyncRoot.Lock() ){ }
        /// </summary>
        /// <returns></returns>
        public IDisposable Lock()
        {
            this.semaphoreSlim.Wait();
            return new UnLocker(this);
        }

        /// <summary>
        /// 锁住代码块
        /// using( await asyncRoot.LockAsync() ){ }
        /// </summary>
        /// <returns></returns>
        public async Task<IDisposable> LockAsync()
        {
            await this.semaphoreSlim.WaitAsync().ConfigureAwait(false);
            return new UnLocker(this);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.semaphoreSlim.Dispose();
        }

        /// <summary>
        /// 提供解锁
        /// </summary>
        private class UnLocker : Disposable
        {
            /// <summary>
            /// 信号量
            /// </summary>
            private readonly AsyncRoot root;

            /// <summary>
            /// 解锁
            /// </summary>
            /// <param name="root"></param>
            public UnLocker(AsyncRoot root)
            {
                this.root = root;
            }

            /// <summary>
            /// 释放锁
            /// </summary>
            /// <param name="disposing"></param>
            protected override void Dispose(bool disposing)
            {
                this.root.semaphoreSlim.Release();
            }
        }
    }
}
