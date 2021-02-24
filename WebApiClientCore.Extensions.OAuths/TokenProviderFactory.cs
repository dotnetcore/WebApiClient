using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示默认的token提供者工厂
    /// </summary>
    sealed class TokenProviderFactory : ITokenProviderFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TokenProviderFactoryOptions options;
        private readonly ConcurrentDictionary<CacheKey, ITokenProvider> tokenProviderCache = new ConcurrentDictionary<CacheKey, ITokenProvider>();

        /// <summary>
        /// 默认的token提供者工厂
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="options"></param>
        public TokenProviderFactory(IServiceProvider serviceProvider, IOptions<TokenProviderFactoryOptions> options)
        {
            this.serviceProvider = serviceProvider;
            this.options = options.Value;
        }

        /// <summary>
        /// 通过接口类型获取或创建其对应的token提供者
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <param name="typeMatchMode">类型匹配模式</param>     
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public ITokenProvider Create(Type httpApiType, TypeMatchMode typeMatchMode)
        {
            if (httpApiType == null)
            {
                throw new ArgumentNullException(nameof(httpApiType));
            }

            var cacheKey = new CacheKey(httpApiType, typeMatchMode);
            return this.tokenProviderCache.GetOrAdd(cacheKey, this.GetTokenProvider);
        }

        /// <summary>
        /// 获取或创建其对应的token提供者
        /// </summary>
        /// <param name="cacheKey">缓存的键</param>    
        /// <returns></returns> 
        /// <exception cref="InvalidOperationException"></exception>
        private ITokenProvider GetTokenProvider(CacheKey cacheKey)
        {
            var httpApiType = cacheKey.HttpApiType;
            if (this.options.TryGetValue(httpApiType, out var serviceType))
            {
                var service = this.serviceProvider.GetRequiredService(serviceType);
                return ((ITokenProviderService)service).TokenProvider;
            }

            if (cacheKey.TypeMatchMode == TypeMatchMode.TypeOrBaseTypes)
            {
                return this.GetTokenProviderFromBaseType(httpApiType);
            }

            throw new InvalidOperationException($"尚未注册{httpApiType}的token提供者");
        }

        /// <summary>
        /// 从基础接口获取TokenProvider
        /// </summary>
        /// <param name="httpApiType"></param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        private ITokenProvider GetTokenProviderFromBaseType(Type httpApiType)
        {
            foreach (var baseType in httpApiType.GetInterfaces())
            {
                if (this.options.TryGetValue(baseType, out var serviceType))
                {
                    var service = this.serviceProvider.GetRequiredService(serviceType);
                    return ((ITokenProviderService)service).TokenProvider;
                }
            }
            throw new InvalidOperationException($"尚未注册{httpApiType}或其基础接口的token提供者");
        }

        /// <summary>
        /// 缓存的键
        /// </summary>
        private class CacheKey : IEquatable<CacheKey>
        {
            public Type HttpApiType { get; }

            public TypeMatchMode TypeMatchMode { get; }

            public CacheKey(Type httpApiType, TypeMatchMode typeMatchMode)
            {
                this.HttpApiType = httpApiType;
                this.TypeMatchMode = typeMatchMode;
            }

            public bool Equals(CacheKey other)
            {
                return this.HttpApiType == other.HttpApiType && this.TypeMatchMode == other.TypeMatchMode;
            }

            public override bool Equals(object obj)
            {
                return obj is CacheKey other && this.Equals(other);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(this.HttpApiType, this.TypeMatchMode);
            }
        }
    }
}
