using System;
using System.Diagnostics.CodeAnalysis;

namespace WebApiClientCore
{
    /// <summary>
    /// IServiceProvider扩展
    /// </summary>
    static class ServiceProviderExtensions
    {
        /// <summary>
        /// 获取服务
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider">服务提供者</param>
        /// <returns></returns>
        [return: MaybeNull]
        public static T GetService<T>(this IServiceProvider provider)
        {
            return (T)provider.GetService(typeof(T));
        }
    }
}
