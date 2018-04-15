using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供异步锁
    /// </summary>
    class AsyncRoot : IDisposable
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
            return new UnLocker(this.semaphoreSlim);
        }

        /// <summary>
        /// 锁住代码块
        /// using( await asyncRoot.LockAsync() ){ }
        /// </summary>
        /// <returns></returns>
        public async Task<IDisposable> LockAsync()
        {
            await this.semaphoreSlim.WaitAsync();
            return new UnLocker(this.semaphoreSlim);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.semaphoreSlim.Dispose();
        }

        /// <summary>
        /// 提供解锁
        /// </summary>
        class UnLocker : IDisposable
        {
            /// <summary>
            /// 信号量
            /// </summary>
            private readonly SemaphoreSlim semaphoreSlim;

            /// <summary>
            /// 解锁
            /// </summary>
            /// <param name="semaphoreSlim">信号量</param>
            public UnLocker(SemaphoreSlim semaphoreSlim)
            {
                this.semaphoreSlim = semaphoreSlim;
            }

            /// <summary>
            /// 释放锁
            /// </summary>
            public void Dispose()
            {
                this.semaphoreSlim.Release();
            }
        }
    }
}
