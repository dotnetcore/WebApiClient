using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供接口类型命名记录扩展
    /// </summary>
    public static class NamedHttpApiExtensions
    {
        /// <summary>
        /// 注册http接口类型的别名
        /// </summary>
        /// <param name="services"></param>
        /// <param name="name">接口别名</param>
        /// <param name="httpApiType">接口类型</param>
        internal static void NamedHttpApiType(this IServiceCollection services, string name, Type httpApiType)
        {
            services.TryAddSingleton(new NameTypeRegistration());
            var descriptor = services.Single(item => item.ServiceType == typeof(NameTypeRegistration));

            var registration = (NameTypeRegistration)descriptor.ImplementationInstance;
            registration[name] = httpApiType;
        }

        /// <summary>
        /// 获取builder关联的HttpApi类型
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static Type? GetHttpApiType(this IHttpClientBuilder builder)
        {
            var descriptor = builder.Services.FirstOrDefault(item => item.ServiceType == typeof(NameTypeRegistration));
            if (descriptor == null)
            {
                return null;
            }

            var registration = (NameTypeRegistration)descriptor.ImplementationInstance;
            registration.TryGetValue(builder.Name, out var type);
            return type;
        }

        /// <summary>
        /// 命名记录
        /// </summary>
        private class NameTypeRegistration : Dictionary<string, Type>
        {
        }
    }
}
