using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 用于记录接口类型与TokenProviderService的映射关系
    /// </summary>
    sealed class TokenProviderFactoryOptions
    {
        /// <summary>
        /// httpApi的服务描述映射
        /// </summary>
        private readonly Dictionary<Type, AliasServiceDescriptor> httpApiServiceDescriptors = [];

        /// <summary>
        /// 登记映射
        /// </summary>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <typeparam name="TTokenPrivder">提供者类型</typeparam>
        /// <param name="alias">TokenProvider的别名</param>
        public void Register<THttpApi, TTokenPrivder>(string alias) where TTokenPrivder : ITokenProvider
        {
            var httpApiType = typeof(THttpApi);
            var serviceType = typeof(TokenProviderService<THttpApi, TTokenPrivder>);
            var serviceDescriptor = new AliasServiceDescriptor(serviceType);

            if (this.httpApiServiceDescriptors.TryGetValue(httpApiType, out var existDescriptor) &&
                existDescriptor.ServiceType == serviceType)
            {
                existDescriptor.AddAlias(alias);
            }
            else
            {
                serviceDescriptor.AddAlias(alias);
                this.httpApiServiceDescriptors[httpApiType] = serviceDescriptor;
            }
        }

        /// <summary>
        /// 尝试获取服务描述值
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <param name="descriptor">TokenProviderService的描述</param>
        /// <returns></returns>
        public bool TryGetValue(Type httpApiType, [MaybeNullWhen(false)] out AliasServiceDescriptor descriptor)
        {
            return this.httpApiServiceDescriptors.TryGetValue(httpApiType, out descriptor);
        }

        /// <summary>
        /// 别名的服务描述
        /// </summary>
        public class AliasServiceDescriptor
        {
            private readonly HashSet<string> aliasSet = [];

            /// <summary>
            /// 获取服务类型
            /// </summary>
            public Type ServiceType { get; }

            /// <summary>
            /// 别名的服务描述
            /// </summary>
            /// <param name="serviceType">服务类型</param>
            public AliasServiceDescriptor(Type serviceType)
            {
                this.ServiceType = serviceType;
            }

            /// <summary>
            /// 添加别名
            /// </summary>
            /// <param name="alias"></param>
            public void AddAlias(string alias)
            {
                this.aliasSet.Add(alias);
            }

            /// <summary>
            /// 是否存在别名
            /// </summary>
            /// <param name="alias"></param>
            /// <returns></returns>
            public bool ContainsAlias(string alias)
            {
                return this.aliasSet.Contains(alias);
            }
        }
    }
}
