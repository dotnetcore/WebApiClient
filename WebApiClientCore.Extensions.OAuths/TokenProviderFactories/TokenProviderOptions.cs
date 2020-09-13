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
        /// 实例化过的所有描述器
        /// </summary>
        private readonly HashSet<TokenProviderDescriptor> descriptorHashSet = new HashSet<TokenProviderDescriptor>();

        /// <summary>
        /// 登录映射
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenPrivder">提供者类型</typeparam>
        /// <param name="tokenProviderName">提供者的名称</param>
        public void Register<THttpApi, TTokenPrivder>(string tokenProviderName) where TTokenPrivder : ITokenProvider
        {
            var httpApiType = typeof(THttpApi);
            var descriptor = this.CreateDescriptor(tokenProviderName, typeof(TTokenPrivder), out var old);
            this[httpApiType] = descriptor;

            if (old != null)
            {
                foreach (var key in this.Keys)
                {
                    if (ReferenceEquals(this[key], old))
                    {
                        this[key] = descriptor;
                    }
                }
            }
        }

        /// <summary>
        /// 创建描述器
        /// </summary>
        /// <param name="tokenProviderName"></param>
        /// <param name="tokenProviderType"></param>
        /// <param name="old"></param>
        /// <returns></returns>
        private TokenProviderDescriptor CreateDescriptor(string tokenProviderName, Type tokenProviderType, out TokenProviderDescriptor? old)
        {
            var descriptor = new TokenProviderDescriptor(tokenProviderName, tokenProviderType);
            if (this.descriptorHashSet.TryGetValue(descriptor, out old) == true)
            {
                this.descriptorHashSet.Remove(descriptor);
            }

            this.descriptorHashSet.Add(descriptor);
            return descriptor;
        }
    }
}
