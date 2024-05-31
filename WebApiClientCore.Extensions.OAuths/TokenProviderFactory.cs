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
        private readonly ConcurrentDictionary<ServiceKey, ITokenProvider> tokenProviderCache = new();

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
            return this.Create(httpApiType, typeMatchMode, name: string.Empty);
        }

        /// <summary>
        /// 通过接口类型获取或创建其对应的token提供者
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <param name="typeMatchMode">类型匹配模式</param>
        /// <param name="name">TokenProvider的别名</param>     
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ITokenProvider Create(Type httpApiType, TypeMatchMode typeMatchMode, string name)
        {
            if (httpApiType == null)
            {
                throw new ArgumentNullException(nameof(httpApiType));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var serviceKey = new ServiceKey(httpApiType, typeMatchMode, name);
            return this.tokenProviderCache.GetOrAdd(serviceKey, this.CreateTokenProvider);
        }

        /// <summary>
        /// 创建其对应的token提供者
        /// </summary>
        /// <param name="serviceKey">缓存的键</param>    
        /// <returns></returns> 
        /// <exception cref="InvalidOperationException"></exception>
        private ITokenProvider CreateTokenProvider(ServiceKey serviceKey)
        {
            var name = serviceKey.Name;
            var httpApiType = serviceKey.HttpApiType;
            if (this.options.TryGetValue(httpApiType, out var descriptor) && descriptor.ContainsName(name))
            {
                var service = (ITokenProviderService)this.serviceProvider.GetRequiredService(descriptor.ServiceType);
                service.SetProviderName(name);
                return service.TokenProvider;
            }

            if (serviceKey.TypeMatchMode == TypeMatchMode.TypeOrBaseTypes)
            {
                var tokenProvider = this.CreateTokenProviderFromBaseType(httpApiType, name);
                if (tokenProvider != null)
                {
                    return tokenProvider;
                }
            }


            var message = string.IsNullOrEmpty(name)
                ? $"尚未注册{httpApiType}无别名的token提供者"
                : $"尚未注册{httpApiType}别名为{name}的token提供者";
            throw new InvalidOperationException(message);
        }

        /// <summary>
        /// 从基础接口创建TokenProvider
        /// </summary>
        /// <param name="httpApiType"></param>
        /// <param name="name">别名</param>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        private ITokenProvider? CreateTokenProviderFromBaseType(Type httpApiType, string name)
        {
            foreach (var baseType in httpApiType.GetInterfaces())
            {
                if (this.options.TryGetValue(baseType, out var descriptor) && descriptor.ContainsName(name))
                {
                    var service = (ITokenProviderService)this.serviceProvider.GetRequiredService(descriptor.ServiceType);
                    service.SetProviderName(name);
                    return service.TokenProvider;
                }
            }
            return null;
        }

        /// <summary>
        /// 服务缓存的键
        /// </summary>
        private sealed class ServiceKey : IEquatable<ServiceKey>
        {
            private int? hashCode;

            public Type HttpApiType { get; }

            public TypeMatchMode TypeMatchMode { get; }

            public string Name { get; }

            public ServiceKey(Type httpApiType, TypeMatchMode typeMatchMode, string name)
            {
                this.HttpApiType = httpApiType;
                this.TypeMatchMode = typeMatchMode;
                this.Name = name;
            }

            public bool Equals(ServiceKey? other)
            {
                return other != null &&
                    this.HttpApiType == other.HttpApiType &&
                    this.TypeMatchMode == other.TypeMatchMode &&
                    this.Name == other.Name;
            }

            public override bool Equals(object? obj)
            {
                return obj is ServiceKey other && this.Equals(other);
            }

            public override int GetHashCode()
            {
                this.hashCode ??= HashCode.Combine(this.HttpApiType, this.TypeMatchMode, this.Name);
                return this.hashCode.Value;
            }
        }
    }
}
