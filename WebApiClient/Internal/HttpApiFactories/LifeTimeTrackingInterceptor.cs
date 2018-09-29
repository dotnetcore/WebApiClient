using System;
using WebApiClient.Defaults;

namespace WebApiClient
{
    /// <summary>
    /// 表示具有生命周期自动监视的http接口调用的拦截器
    /// </summary>
    class LifeTimeTrackingInterceptor : ApiInterceptor
    {
        /// <summary>
        /// http接口调用的拦截器
        /// </summary>
        /// <param name="httpApiConfig">httpApi配置</param>
        /// <exception cref="ArgumentNullException"></exception>
        public LifeTimeTrackingInterceptor(HttpApiConfig httpApiConfig)
            : base(httpApiConfig)
        {
        }

        /// <summary>
        /// 这里不释放资源
        /// </summary>
        public sealed override void Dispose()
        {
        }
    }
}
