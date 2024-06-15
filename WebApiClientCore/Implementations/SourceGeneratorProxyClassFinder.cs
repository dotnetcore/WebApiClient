using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 提供获取SourceGenerator生成的代理类型
    /// </summary>
    static class SourceGeneratorProxyClassFinder
    {
        private static readonly object syncRoot = new();
        private static readonly HashSet<Assembly> assemblies = [];
        private static readonly Dictionary<Type, Type> httpApiProxyClassTable = [];
        private const string HttpApiProxyClassTypeName = "WebApiClientCore.HttpApiProxyClass";

        /// <summary>
        /// 查找指定接口类型的代理类类型
        /// </summary>
        /// <param name="httpApiType">接口类型</param> 
        /// <returns></returns>
        public static Type? Find(Type httpApiType)
        {
            lock (syncRoot)
            {
                if (assemblies.Add(httpApiType.Assembly))
                {
                    AnalyzeAssembly(httpApiType.Assembly);
                }
            }

            if (httpApiProxyClassTable.TryGetValue(httpApiType, out var proxyClassType))
            {
                return proxyClassType;
            }
            return null;
        }

        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026", Justification = "类型已使用ModuleInitializer和DynamicDependency来阻止被裁剪")]
        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2075", Justification = "类型已使用ModuleInitializer和DynamicDependency来阻止被裁剪")]
        private static void AnalyzeAssembly(Assembly assembly)
        {
            var httpApiProxyClass = assembly.GetType(HttpApiProxyClassTypeName);
            if (httpApiProxyClass != null)
            {
                foreach (var classType in httpApiProxyClass.GetNestedTypes(BindingFlags.NonPublic))
                {
                    var proxyClassAttr = classType.GetCustomAttribute<HttpApiProxyClassAttribute>();
                    if (proxyClassAttr != null && proxyClassAttr.HttpApiType.IsAssignableFrom(classType))
                    {
                        httpApiProxyClassTable.TryAdd(proxyClassAttr.HttpApiType, classType);
                    }
                }
            }
        }
    }
}
