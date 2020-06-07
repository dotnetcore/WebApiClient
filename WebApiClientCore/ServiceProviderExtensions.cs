using System;
using System.Diagnostics.CodeAnalysis;
using WebApiClientCore.Serialization;

namespace WebApiClientCore
{
    /// <summary>
    /// IServiceProvider扩展
    /// </summary>
    public static class ServiceProviderExtensions
    {
        private static readonly XmlSerializer xmlSerializer = new XmlSerializer();
        private static readonly JsonSerializer jsonSerializer = new JsonSerializer();
        private static readonly KeyValueSerializer keyValueSerializer = new KeyValueSerializer();

        /// <summary>
        /// 尝试获取IXmlSerializer
        /// 获取不到则使用默认实例
        /// </summary>
        /// <param name="provider">服务提供者</param>
        /// <returns></returns>
        public static IXmlSerializer GetXmlSerializer(this IServiceProvider provider)
        {
            return provider.GetService<IXmlSerializer>() ?? xmlSerializer;
        }

        /// <summary>
        /// 尝试获取IJsonSerializer
        /// 获取不到则使用默认实例
        /// </summary>
        /// <param name="provider">服务提供者</param>
        /// <returns></returns>
        public static IJsonSerializer GetJsonSerializer(this IServiceProvider provider)
        {
            return provider.GetService<IJsonSerializer>() ?? jsonSerializer;
        }

        /// <summary>
        /// 尝试获取IKeyValueSerializer
        /// 获取不到则使用默认实例
        /// </summary>
        /// <param name="provider">服务提供者</param>
        /// <returns></returns>
        public static IKeyValueSerializer GetKeyValueSerializer(this IServiceProvider provider)
        {
            return provider.GetService<IKeyValueSerializer>() ?? keyValueSerializer;
        }

        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider">服务提供者</param>
        /// <returns></returns>
        [return: MaybeNull]
        internal static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }
    }
}
