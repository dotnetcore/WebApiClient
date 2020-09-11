using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// 提供http接口类型的扩展
    /// </summary>
    public static class HttpApiTypeExtensions
    {
        /// <summary>
        /// 获取builder关联的HttpApi类型
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static Type? GetHttpApiType(this IHttpClientBuilder builder)
        {
            var descriptor = builder.Services.FirstOrDefault(item => item.ServiceType == typeof(HttpApiMappingRegistry));
            if (descriptor == null)
            {
                return null;
            }

            var registry = (HttpApiMappingRegistry)descriptor.ImplementationInstance;
            registry.NamedHttpApiRegistrations.TryGetValue(builder.Name, out var type);
            return type;
        }
    }
}
