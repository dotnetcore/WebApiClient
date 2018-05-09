using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供HttpApiClient代理类生成
    /// 不支持泛型方法的接口
    /// 不支持ref/out参数的接口
    /// </summary>
    static class HttpApiClientProxy
    {
        /// <summary>
        /// 代理类型的构造器的参数类型
        /// </summary>
        private static readonly Type[] proxyTypeCtorArgTypes = new Type[] { typeof(IApiInterceptor), typeof(MethodInfo[]) };

        /// <summary>
        /// 创建静态代理实例
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public static object CreateProxyInstance(Type interfaceType, IApiInterceptor interceptor)
        {
            var proxyTypeName = interfaceType.Name.Length > 1 && interfaceType.Name.StartsWith("I") ? interfaceType.Name.Substring(1) : interfaceType.Name;
            var typeName = $"System.StaticProxy.{interfaceType.Namespace}.{proxyTypeName}";

            var proxyType = interfaceType.Assembly.GetType(typeName, false);
            var apiMethods = interfaceType.GetAllApiMethods();

            var proxyTypeCtor = proxyType.GetConstructor(proxyTypeCtorArgTypes);
            return proxyTypeCtor.Invoke(new object[] { interceptor, apiMethods });
        }
    }
}
