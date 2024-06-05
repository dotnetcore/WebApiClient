using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 提供获取SourceGenerator生成的代理类型
    /// </summary>
    static class SourceGeneratorProxyClassType
    {
        private static readonly object syncRoot = new();
        private static readonly HashSet<Assembly> assemblies = [];
        private static readonly ConcurrentDictionary<Type, Type> httpApiProxyClassTable = [];

        /// <summary>
        /// 查找指定接口类型的代理类类型
        /// </summary>
        /// <param name="httpApiType">接口类型</param> 
        /// <returns></returns>
        public static Type? Find(Type httpApiType)
        {
            AnalyzeAssembly(httpApiType.Assembly);

            if (httpApiProxyClassTable.TryGetValue(httpApiType, out var proxyClassType))
            {
                return proxyClassType;
            }
            return null;
        }


        private static void AnalyzeAssembly(Assembly assembly)
        {
            if (AddAssembly(assembly))
            {
                foreach (var classType in assembly.GetTypes())
                {
                    if (classType.IsClass)
                    {
                        var proxyClassAttr = classType.GetCustomAttribute<HttpApiProxyClassAttribute>();
                        if (proxyClassAttr != null)
                        {
                            httpApiProxyClassTable.TryAdd(proxyClassAttr.HttpApiType, classType);
                        }
                    }
                }
            }
        }

        private static bool AddAssembly(Assembly assembly)
        {
            lock (syncRoot)
            {
                return assemblies.Add(assembly);
            }
        }
    }
}
