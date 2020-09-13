using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 用于记录接口类型与接口提供者域的映射
    /// </summary>
    class TokenProviderFactoryOptions : Dictionary<Type, TokenProviderDescriptor>
    {
        /// <summary>
        /// 登录映射
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenPrivder">提供者类型</typeparam>
        /// <param name="tokenProviderName">提供者的名称</param>
        public void Register<THttpApi, TTokenPrivder>(string tokenProviderName) where TTokenPrivder : ITokenProvider
        {
            // 更新已有的descriptor为最新实例
            var descriptor = new TokenProviderDescriptor(tokenProviderName, typeof(TTokenPrivder));
            foreach (var key in this.Keys.ToArray())
            {
                if (this[key].Equals(descriptor) == true)
                {
                    this[key] = descriptor;
                }
            }

            this[typeof(THttpApi)] = descriptor;
        }
    }
}
