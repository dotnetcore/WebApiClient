using System;
using WebApiClientCore.Extensions.OAuths.TokenClients;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供token客户端的扩展
    /// </summary>
    public static class TokenClientExtensions
    {
        /// <summary>
        /// 添加自定义token客户端处理指定接口
        /// </summary>
        /// <typeparam name="TCustomTokenClient">自定义token客户端</typeparam>
        /// <typeparam name="THttpApi">接口类型</typeparam>
        /// <param name="services"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static IServiceCollection AddCustomTokenClient<TCustomTokenClient, THttpApi>(this IServiceCollection services)
        {
            return services.AddCustomTokenClient<TCustomTokenClient>(typeof(THttpApi));
        }

        /// <summary>
        /// 添加自定义token客户端处理指定接口
        /// </summary>
        /// <typeparam name="TCustomTokenClient">自定义token客户端</typeparam>    
        /// <param name="services"></param>
        /// <param name="httpApiType">接口类型</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public static IServiceCollection AddCustomTokenClient<TCustomTokenClient>(this IServiceCollection services, Type httpApiType)
        {
            var serviceType = typeof(ICustomTokenClient<>).MakeGenericType(httpApiType);
            var implementationType = typeof(TCustomTokenClient);

            if (serviceType.IsAssignableFrom(implementationType) == false)
            {
                var type = typeof(CustomTokenClient<>).MakeGenericType(httpApiType);
                throw new ArgumentException($"{typeof(TCustomTokenClient).Name}的类型必须为{type}");
            }

            return services.AddTransient(serviceType, implementationType);
        }
    }
}
