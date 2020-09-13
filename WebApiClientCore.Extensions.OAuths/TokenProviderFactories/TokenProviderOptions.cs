using System;
using System.Collections.Generic;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 用于记录接口类型与接口提供者域的映射
    /// </summary>
    class TokenProviderOptions : Dictionary<Type, TokenProviderDomain>
    {
        /// <summary>
        /// 登录映射
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenPrivder">提供者类型</typeparam>
        /// <param name="domain">接口所在的域</param>
        public void Register<THttpApi, TTokenPrivder>(string domain) where TTokenPrivder : ITokenProvider
        {
            var httpApiType = typeof(THttpApi);
            var tokenProviderType = typeof(TTokenPrivder);
            var tokenProviderDomain = new TokenProviderDomain(domain, tokenProviderType);

            if (this.TryGetValue(httpApiType, out var buildinDomain) == true)
            {
                if (buildinDomain.Equals(tokenProviderDomain) == false)
                {
                    throw new InvalidOperationException($"每个接口或domain只能使用一种tokenProvider：{domain}");
                }
            }

            this[httpApiType] = tokenProviderDomain;
        }
    }
}
