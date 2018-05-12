#if AOT
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace WebApiClient
{
    /// <summary>
    /// 提供HttpApiClient代理搜索和实例化
    /// </summary>
    static class HttpApiClientProxy
    {
        /// <summary>
        /// 代理类型的构造器的参数类型
        /// </summary>
        private static readonly Type[] proxyTypeCtorArgTypes = new Type[] { typeof(IApiInterceptor), typeof(MethodInfo[]) };

        /// <summary>
        /// 接口对应的代理类型的构造器缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, ConstructorInfo> proxyCtorCache = new ConcurrentDictionary<Type, ConstructorInfo>();

        /// <summary>
        /// 接口的方法缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, MethodInfo[]> interfaceMethodsCache = new ConcurrentDictionary<Type, MethodInfo[]>();


        /// <summary>
        /// 搜索接口的代理类型并实例化
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="interceptor">拦截器</param>
        /// <exception cref="TypeLoadException"></exception>
        /// <returns></returns>
        public static object CreateInstance(Type interfaceType, IApiInterceptor interceptor)
        {
            var proxyTypeCtor = proxyCtorCache.GetOrAdd(interfaceType, type =>
            {
                var @namespace = GetProxyTypeNamespace(interfaceType.Namespace);
                var typeName = GetProxyTypeName(interfaceType.Name);
                var fullTypeName = $"{@namespace}.{typeName}";

                var proxyType = interfaceType.Assembly.GetType(fullTypeName, false);
                return proxyType?.GetConstructor(proxyTypeCtorArgTypes);
            });

            if (proxyTypeCtor == null)
            {
                throw new TypeLoadException($"找不到类型{interfaceType}的代理类，请确保接口继承于IHttpApi接口");
            }

            var apiMethods = interfaceType.GetAllApiMethods();
            return proxyTypeCtor.Invoke(new object[] { interceptor, apiMethods });
        }
         
        /// <summary>
        /// 返回接口类型的代理类型的命名空间
        /// </summary>
        /// <param name="interfaceNamespace">接口命名空间</param>
        /// <returns></returns>
        private static string GetProxyTypeNamespace(string interfaceNamespace)
        {
            return $"{interfaceNamespace}.Proxy";
        }

        /// <summary>
        /// 返回接口类型的代理类型的名称
        /// </summary>
        /// <param name="interfaceTypeName">接口类型名称</param>
        /// <returns></returns>
        private static string GetProxyTypeName(string interfaceTypeName)
        {
            if (interfaceTypeName.Length <= 1 || interfaceTypeName.StartsWith("I") == false)
            {
                return interfaceTypeName;
            }
            return interfaceTypeName.Substring(1);
        }
    }
}
#endif