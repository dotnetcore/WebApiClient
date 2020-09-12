using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// token提供者创建器接口
    /// </summary>
    public interface ITokenProviderBuilder
    {
        /// <summary>
        /// 获取别名
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 获取接口类型
        /// </summary>
        Type HttpApiType { get; }

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        IServiceCollection Services { get; }
    }
}
