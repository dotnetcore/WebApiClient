using System;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示激活状态的Handler记录
    /// </summary>
    class ActiveHandlerEntry
    {
        /// <summary>
        /// 获取或设置关联的http接口类型
        /// </summary>
        public Type ApiType { get; set; }

        /// <summary>
        /// 获取或设置http接口配置
        /// </summary>
        public HttpApiConfig HttpApiConfig { get; set; }

        /// <summary>
        /// 获取或设置用于释放资源的对象
        /// </summary>
        public IDisposable Disposable { get; set; }

        /// <summary>
        /// 激活状态的Handler记录
        /// </summary>
        /// <param name="factory">httpApiClient工厂</param>
        public ActiveHandlerEntry(IHttpApiClientFactory factory)
        {
            Task.Delay(factory.Lifetime)
                .ConfigureAwait(false)
                .GetAwaiter()
                .OnCompleted(() => factory.OnEntryDeactivate(this));
        }
    }
}
