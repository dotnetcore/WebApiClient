using System;
using System.Collections.Generic;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// token提供者工厂选项
    /// </summary>
    class TokenProviderFactoryOptions
    {
        /// <summary>
        /// 获取token提供者注册信息
        /// </summary>
        public Dictionary<string, Type> Registrations { get; } = new Dictionary<string, Type>();

        /// <summary>
        /// 添加token提供者
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenProvider">token提供者类型</typeparam>
        public void AddTokenProvider<THttpApi, TTokenProvider>() where TTokenProvider : ITokenProvider
        {
            var name = HttpApi.GetName<THttpApi>();
            this.Registrations[name] = typeof(TTokenProvider);
        }
    }
}
