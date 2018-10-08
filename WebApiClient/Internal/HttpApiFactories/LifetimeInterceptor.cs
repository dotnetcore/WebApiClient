using System;
using System.Threading.Tasks;
using WebApiClient.Defaults;

namespace WebApiClient
{
    /// <summary>
    /// 表示具有生命周期的拦截器
    /// </summary>
    class LifetimeInterceptor : ApiInterceptor
    {
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

            Task.Delay(lifeTime)
                .ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(() => deactivateAction(this));
        }

        /// <summary>
        /// 这里不释放资源
        /// </summary>
        public sealed override void Dispose()
        {
        }
    }
}
