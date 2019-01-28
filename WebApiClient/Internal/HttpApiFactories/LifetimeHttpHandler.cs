using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;

namespace WebApiClient
{
    /// <summary>
    /// 表示具有生命周期的HttpHandler
    /// </summary>
    [DebuggerDisplay("LifeTime = {lifeTime}")]
    class LifetimeHttpHandler : DelegatingHandler
    {
        /// <summary>
        /// 生命周期
        /// </summary>
        private readonly TimeSpan lifeTime;

        /// <summary>
        /// Token取消源
        /// </summary>
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        /// <summary>
        /// 具有生命周期的HttpHandler
        /// </summary>
        /// <param name="handler">HttpHandler</param>
        /// <param name="lifeTime">拦截器的生命周期</param>
        /// <param name="deactivateAction">失效回调</param>
        /// <exception cref="ArgumentNullException"></exception>
        public LifetimeHttpHandler(HttpMessageHandler handler, TimeSpan lifeTime, Action<LifetimeHttpHandler> deactivateAction)
            : base(handler)
        {
            if (deactivateAction == null)
            {
                throw new ArgumentNullException(nameof(deactivateAction));
            }

            this.lifeTime = lifeTime;

            this.tokenSource.Token.Register(() =>
            {
                this.tokenSource.Dispose();
                deactivateAction.Invoke(this);
            }, useSynchronizationContext: false);

            this.tokenSource.CancelAfter(lifeTime);
        }

        /// <summary>
        /// 这里不释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
        }
    }
}
