using Castle.DynamicProxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi客户端
    /// 提供获取Http接口的实例
    /// </summary>
    public static class HttpApiClient
    {
        /// <summary>
        /// void类型
        /// </summary>
        private static readonly Type voidType = typeof(void);

        /// <summary>
        /// 代理生成器
        /// </summary>
        private static readonly ProxyGenerator generator = new ProxyGenerator();

        /// <summary>
        /// dispose方法
        /// </summary>
        private static readonly MethodInfo disposeMethod = typeof(IDisposable).GetMethods().FirstOrDefault();

        /// <summary>
        /// 创建请求接口的实例
        /// 关联新建的HttpApiConfig对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static TInterface Create<TInterface>() where TInterface : class
        {
            return Create<TInterface>(null);
        }

        /// <summary>
        /// 创建请求接口的实例
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <param name="httpApiConfig">接口配置</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static TInterface Create<TInterface>(HttpApiConfig httpApiConfig) where TInterface : class
        {
            CheckApiInterface<TInterface>();

            if (httpApiConfig == null)
            {
                httpApiConfig = new HttpApiConfig();
            }
            var interceptor = new DefaultInterceptor(httpApiConfig);
            return generator.CreateInterfaceProxyWithoutTarget<TInterface>(interceptor);
        }

        /// <summary>
        /// 检测接口的正确性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private static void CheckApiInterface<T>()
        {
            var type = typeof(T);
            if (type.IsInterface == false)
            {
                throw new ArgumentException(typeof(T).Name + "不是接口类型");
            }

            foreach (var m in type.GetMethods())
            {
                if (m.ReturnType == voidType && m.Equals(disposeMethod) == false)
                {
                    throw new NotSupportedException("不支持的void返回方法：" + m);
                }
            }
        }
    }
}
