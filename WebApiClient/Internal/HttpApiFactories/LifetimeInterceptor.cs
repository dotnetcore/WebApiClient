using System;
using System.Threading;
using WebApiClient.Defaults;

namespace WebApiClient
{
    /// <summary>
    /// 表示具有生命周期的拦截器
    /// </summary>
    class LifetimeInterceptor : ApiInterceptor
    {
        /// <summary>
        /// Token取消源
        /// </summary>
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();

        /// <summary>
        /// 具有生命周期的拦截器
        /// </summary>
        /// <param name="httpApiConfig">httpApi配置</param>
        /// <param name="lifeTime">拦截器的生命周期</param>
        /// <param name="deactivateAction">失效回调</param>
        /// <exception cref="ArgumentNullException"></exception>
        public LifetimeInterceptor(HttpApiConfig httpApiConfig, TimeSpan lifeTime, Action<LifetimeInterceptor> deactivateAction)
            : base(httpApiConfig)
        {
            if (deactivateAction == null)
            {
                throw new ArgumentNullException(nameof(deactivateAction));
            }

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
        public sealed override void Dispose()
        {
        }
    }
}
