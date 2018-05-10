using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        private static readonly ConcurrentCache<Type, ConstructorInfo> proxyCtorCache = new ConcurrentCache<Type, ConstructorInfo>();

        /// <summary>
        /// 接口的方法缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, MethodInfo[]> interfaceMethodsCache = new ConcurrentCache<Type, MethodInfo[]>();


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
                var fullTypeName = type.GetProxyTypeFullName();
                var proxyType = interfaceType.Assembly.GetType(fullTypeName, false);
                return proxyType?.GetConstructor(proxyTypeCtorArgTypes);
            });

            if (proxyTypeCtor == null)
            {
                throw new TypeLoadException($"找不到类型{interfaceType}的代理类，请确保接口继承于IHttpApi接口");
            }

            var apiMethods = interfaceType.GetInterfaceAllApis();
            return proxyTypeCtor.Invoke(new object[] { interceptor, apiMethods });
        }

        /// <summary>
        /// 返回接口的代理类型全名
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        private static string GetProxyTypeFullName(this Type interfaceType)
        {
            var @namespace = GetProxyTypeNamespace(interfaceType.Namespace);
            var typeName = GetProxyTypeName(interfaceType.Name);
            return $"{@namespace}.{typeName}";
        }

        /// <summary>
        /// 返回接口类型的代理类型的命名空间
        /// </summary>
        /// <param name="interfaceNamespace">接口命名空间</param>
        /// <returns></returns>
        public static string GetProxyTypeNamespace(string interfaceNamespace)
        {
            return $"{interfaceNamespace}.Proxy";
        }

        /// <summary>
        /// 返回接口类型的代理类型的名称
        /// </summary>
        /// <param name="interfaceTypeName">接口类型名称</param>
        /// <returns></returns>
        public static string GetProxyTypeName(string interfaceTypeName)
        {
            if (interfaceTypeName.Length <= 1 || interfaceTypeName.StartsWith("I") == false)
            {
                return interfaceTypeName;
            }
            return interfaceTypeName.Substring(1);
        }

        /// <summary>
        /// 获取接口类型及其继承的接口的所有方法
        /// 忽略HttpApiClient类型的所有接口的方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param>     
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        private static MethodInfo[] GetInterfaceAllApis(this Type interfaceType)
        {
            return interfaceMethodsCache.GetOrAdd(interfaceType, type =>
            {
                // 排除HttpApiClient已实现的接口
                var excepts = typeof(HttpApiClient).GetInterfaces();
                var exceptHashSet = new HashSet<Type>(excepts);
                var methodHashSet = new HashSet<MethodInfo>();

                type.GetInterfaceMethods(ref exceptHashSet, ref methodHashSet);
                return methodHashSet.ToArray();
            });
        }

        /// <summary>
        /// 递归查找接口的方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="exceptHashSet">排除的接口类型</param>
        /// <param name="methodHashSet">收集到的方法</param>
        /// <exception cref="NotSupportedException"></exception>
        private static void GetInterfaceMethods(this Type interfaceType, ref HashSet<Type> exceptHashSet, ref HashSet<MethodInfo> methodHashSet)
        {
            if (exceptHashSet.Add(interfaceType) == false)
            {
                return;
            }

            var methods = interfaceType.GetMethods();
            foreach (var item in methods)
            {
                methodHashSet.Add(item);
            }

            foreach (var item in interfaceType.GetInterfaces())
            {
                item.GetInterfaceMethods(ref exceptHashSet, ref methodHashSet);
            }
        }
    }
}
