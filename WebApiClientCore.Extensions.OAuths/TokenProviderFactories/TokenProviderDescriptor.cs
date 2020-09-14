using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示token提供者的描述器
    /// </summary>
    class TokenProviderDescriptor : IEqualityComparer<TokenProviderDescriptor>, IEquatable<TokenProviderDescriptor>
    {
        /// <summary>
        /// 提供者实例
        /// </summary>
        private ITokenProvider? tokenProvider;

        /// <summary>
        /// 获取token提供者名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///  获取token提供者类型
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// token提供者的描述器
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="type">token提供者类型</param>
        public TokenProviderDescriptor(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
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
                var value = (ITokenProvider)serviceProvider.GetRequiredService(this.Type);
                value.Name = this.Name;
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
        public bool Equals(TokenProviderDescriptor other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Name == other.Name && this.Type == other.Type;
        }

        /// <summary>
        /// 比较是否相等
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool Equals(TokenProviderDescriptor x, TokenProviderDescriptor y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int GetHashCode(TokenProviderDescriptor obj)
        {
            return obj.GetHashCode();
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Name, this.Type);
        }

        /// <summary>
        /// 转换为string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
