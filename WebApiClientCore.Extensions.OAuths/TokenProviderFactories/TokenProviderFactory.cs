using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 表示默认的token提供者工厂
    /// </summary>
    class TokenProviderFactory : ITokenProviderFactory
    {
        private readonly IServiceProvider serviceProvider;
        private readonly TokenProviderFactoryOptions options;

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
        /// 创建token提供者
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

            if (typeMatchMode == TypeMatchMode.TypeOnly)
            {
                return this.Create(httpApiType);
            }

            if (this.CanCreate(httpApiType) == false)
            {
                var baseType = httpApiType.GetInterfaces().FirstOrDefault(i => this.CanCreate(i));
                if (baseType != null)
                {
                    httpApiType = baseType;
                }
            }

            return this.Create(httpApiType);
        }


        /// <summary>
        /// 返回是否可以创建token提供者
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        private bool CanCreate(Type httpApiType)
        {
            return this.options.ContainsKey(httpApiType);
        }


        /// <summary>
        /// 创建token提供者
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private ITokenProvider Create(Type httpApiType)
        {
            if (this.options.TryGetValue(httpApiType, out var descriptor))
            {
                return descriptor.CreateTokenProvider(this.serviceProvider);
            }
            throw new InvalidOperationException($"尚未注册{httpApiType}的token提供者");
        }
    }
}
