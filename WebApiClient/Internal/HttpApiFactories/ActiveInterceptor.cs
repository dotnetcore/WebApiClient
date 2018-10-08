using System;
using System.Threading.Tasks;
using WebApiClient.Defaults;

namespace WebApiClient
{
    /// <summary>
    /// 表示激活状态的拦截器
    /// </summary>
    class ActiveInterceptor : ApiInterceptor
    {
        /// <summary>
        /// 激活状态的拦截器
        /// </summary>
        /// <param name="httpApiConfig">httpApi配置</param>
        /// <param name="lifeTime">生命周期</param>
        /// <param name="deactivateAction">生效委托</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ActiveInterceptor(HttpApiConfig httpApiConfig, TimeSpan lifeTime, Action<ActiveInterceptor> deactivateAction)
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
