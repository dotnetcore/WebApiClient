using System;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示激活状态的记录
    /// </summary>
    class ActiveEntry
    {
        /// <summary>
        /// 获取或设置用于释放资源的对象
        /// </summary>
        public IDisposable Disposable { get; set; }

        /// <summary>
        /// 获取或设置http接口拦截器
        /// </summary>
        public LifeTimeTrackingInterceptor Interceptor { get; set; }

        /// <summary>
        /// 激活状态的记录
        /// </summary>
        /// <param name="factory">httpApiClient工厂</param>
        public ActiveEntry(IHttpApiFactory factory)
        {
            Task.Delay(factory.Lifetime)
                .ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(() => factory.OnEntryDeactivate(this));
        }
    }
}
