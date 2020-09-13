using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示token提供者的域
    /// 一个域对应多个接口类型
    /// </summary>
    class TokenProviderDomain : IEquatable<TokenProviderDomain>
    {
        /// <summary>
        /// 提供者实例
        /// </summary>
        private ITokenProvider? tokenProvider;

        /// <summary>
        /// 获取所在域
        /// </summary>
        public string Domain { get; }

        /// <summary>
        /// token提供者类型
        /// </summary>
        public Type TokenProviderType { get; }

        /// <summary>
        /// token提供者的域
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="tokenProviderType"></param>
        public TokenProviderDomain(string domain, Type tokenProviderType)
        {
            this.Domain = domain;
            this.TokenProviderType = tokenProviderType;
        }

        /// <summary>
        /// 获取提供者的实例
        /// </summary>
        /// <param name="serviceProvider">服务提供者</param>
        /// <returns></returns>
        public ITokenProvider CreateTokenProvider(IServiceProvider serviceProvider)
        {
            var instance = Volatile.Read(ref this.tokenProvider);
            if (instance == null)
            {
                var value = (ITokenProvider)serviceProvider.GetRequiredService(this.TokenProviderType);
                value.Domain = this.Domain;
                Interlocked.CompareExchange(ref this.tokenProvider, value, null);
                instance = this.tokenProvider;
            }
            return instance;
        }

        /// <summary>
        /// 是否相等
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(TokenProviderDomain other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Domain == other.Domain && this.TokenProviderType == other.TokenProviderType;
        }
    }
}
