using System;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// Token提供者工厂接口
    /// </summary>
    public interface ITokenProviderFactory
    {
        /// <summary>
        /// 创建token提供者
        /// </summary>
        /// <param name="httpApiType">接口</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        ITokenProvider Create(Type httpApiType);
    }
}
