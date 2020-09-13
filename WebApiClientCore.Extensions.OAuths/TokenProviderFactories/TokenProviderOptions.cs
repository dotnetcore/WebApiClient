using System;
using System.Collections.Generic;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 用于记录接口类型与接口提供者域的映射
    /// </summary>
    class TokenProviderOptions : Dictionary<Type, TokenProviderDescriptor>
    {
        /// <summary>
        /// 登录映射
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenPrivder">提供者类型</typeparam>
        /// <param name="tokenProviderName">提供者的名称</param>
        public void Register<THttpApi, TTokenPrivder>(string tokenProviderName) where TTokenPrivder : ITokenProvider
        {
            var httpApiType = typeof(THttpApi);
            var tokenProviderType = typeof(TTokenPrivder);
            var descriptor = new TokenProviderDescriptor(tokenProviderName, tokenProviderType);

            if (this.TryGetValue(httpApiType, out var instance) == true)
            {
                if (instance.Equals(descriptor) == false)
                {
                    throw new InvalidOperationException($"不能添加多种tokenProvider：{tokenProviderName}");
                }
                this[httpApiType] = instance;
            }
            else
            {
                this[httpApiType] = descriptor;
            }
        }
    }
}
