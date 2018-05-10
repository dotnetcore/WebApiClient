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
        private static readonly ConcurrentCache<Type, ConstructorInfo> ctorCache = new ConcurrentCache<Type, ConstructorInfo>();

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
            var proxyTypeCtor = ctorCache.GetOrAdd(interfaceType, type =>
            {
                var @namespace = type.GetProxyTypeNamespace();
                var typeName = type.GetProxyTypeName();
                var fullTypeName = $"{@namespace}.{type.GetProxyTypeName()}";

                var proxyType = interfaceType.Assembly.GetType(fullTypeName, false);
                return proxyType?.GetConstructor(proxyTypeCtorArgTypes);
            });

            if (proxyTypeCtor == null)
            {
                throw new TypeLoadException($"找不到类型{interfaceType}的代理类");
            }

            var apiMethods = interfaceType.GetInterfaceAllApis();
            return proxyTypeCtor.Invoke(new object[] { interceptor, apiMethods });
        }

        /// <summary>
        /// 返回接口类型的代理类型的命名空间
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        public static string GetProxyTypeNamespace(this Type interfaceType)
        {
            return $"{interfaceType.Namespace}.Proxy";
        }

        /// <summary>
        /// 返回接口类型的代理类型的名称
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        public static string GetProxyTypeName(this Type interfaceType)
        {
            if (interfaceType.Name.Length <= 1 || interfaceType.Name.StartsWith("I") == false)
            {
                return interfaceType.Name;
            }
            return interfaceType.Name.Substring(1);
        }

        /// <summary>
        /// 获取接口类型及其继承的接口的所有方法
        /// 忽略HttpApiClient类型的所有接口的方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static MethodInfo[] GetInterfaceAllApis(this Type interfaceType)
        {
            return interfaceMethodsCache.GetOrAdd(
                interfaceType,
                type => type.GetInterfaceAllApisNoCache());
        }

        /// <summary>
        /// 获取接口类型及其继承的接口的所有方法
        /// 忽略HttpApiClient类型的所有接口的方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param> 
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        private static MethodInfo[] GetInterfaceAllApisNoCache(this Type interfaceType)
        {
            if (interfaceType.IsInterface == false)
            {
                throw new ArgumentException("类型必须为接口类型");
            }

            // 接口的实现在动态程序集里，所以接口必须为public修饰才可以创建代理类并实现此接口            
            if (interfaceType.IsVisible == false)
            {
                throw new NotSupportedException(interfaceType.Name + "必须为public修饰且对外可见");
            }

            // 排除HttpApiClient已实现的接口
            var excepts = typeof(HttpApiClient).GetInterfaces();
            var exceptHashSet = new HashSet<Type>(excepts);
            var methodHashSet = new HashSet<MethodInfo>();

            interfaceType.GetInterfaceMethods(ref exceptHashSet, ref methodHashSet);
            return methodHashSet.ToArray();
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
                item.EnsureApiMethod();
                methodHashSet.Add(item);
            }

            foreach (var item in interfaceType.GetInterfaces())
            {
                item.GetInterfaceMethods(ref exceptHashSet, ref methodHashSet);
            }
        }

        /// <summary>
        /// 确保方法是支持的Api接口
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        private static void EnsureApiMethod(this MethodInfo method)
        {
            if (method.IsGenericMethod == true)
            {
                throw new NotSupportedException("不支持泛型方法：" + method);
            }

            if (method.IsSpecialName == true)
            {
                throw new NotSupportedException("不支持属性访问器：" + method);
            }

            var genericType = method.ReturnType;
            if (genericType.IsGenericType == true)
            {
                genericType = genericType.GetGenericTypeDefinition();
            }

            var isTaskType = genericType == typeof(Task<>) || genericType == typeof(ITask<>);
            if (isTaskType == false)
            {
                var message = $"返回类型必须为Task<>或ITask<>：{method}";
                throw new NotSupportedException(message);
            }

            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType.IsByRef == true)
                {
                    var message = $"接口参数不支持ref/out修饰：{parameter}";
                    throw new NotSupportedException(message);
                }
            }
        }
    }
}
