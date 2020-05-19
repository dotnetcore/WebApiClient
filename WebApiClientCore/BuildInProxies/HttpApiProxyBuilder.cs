using System;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示IHttpApi的代理类的实例创建者
    /// </summary>
    class HttpApiProxyBuilder
    {
        /// <summary>
        /// 接口类型
        /// </summary>
        private readonly Type interfaceType;

        /// <summary>
        /// 接口声明的Api方法
        /// </summary>
        private readonly MethodInfo[] apiMetods;

        /// <summary>
        /// 代理类的构造器
        /// </summary>
        private readonly Func<IActionInterceptor, Type, MethodInfo[], object> proxyTypeCtor;


        /// <summary>
        /// IHttpApi的代理类的实例创建者
        /// </summary>
        /// <param name="interfaceType">IHttpApi接口类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public HttpApiProxyBuilder(Type interfaceType)
        {
            this.interfaceType = interfaceType ?? throw new ArgumentNullException(nameof(interfaceType));
            this.apiMetods = interfaceType.GetAllApiMethods();
            var proxyType = HttpApiProxyTypeBuilder.Build(interfaceType, this.apiMetods);
            this.proxyTypeCtor = Lambda.CreateCtorFunc<IActionInterceptor, Type, MethodInfo[], object>(proxyType);
        }

        /// <summary>
        /// 创建IHttpApi的代理类的实例
        /// </summary>
        /// <param name="interceptor">拦截器</param>
        /// <returns></returns>
        public object Build(IActionInterceptor interceptor)
        {
            return this.proxyTypeCtor.Invoke(interceptor, this.interfaceType, this.apiMetods);
        }
    }
}
