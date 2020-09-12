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
        /// <param name="httpApiType">接口名称</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        ITokenProvider Create(Type httpApiType);

        /// <summary>
        /// 创建token提供者
        /// </summary>
        /// <param name="name">提供者的别名</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        ITokenProvider Create(string name);
    }
}
