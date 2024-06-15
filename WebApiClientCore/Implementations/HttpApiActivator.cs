using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示THttpApi的实例创建器抽象
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    [Obsolete("该类型存在构造器调用虚方法的设计失误，不建议再使用", error: false)]
    public abstract class HttpApiActivator<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] THttpApi>
        : IHttpApiActivator<THttpApi>
    {
        /// <summary>
        /// 接口的所有方法执行器
        /// </summary>
        private readonly ApiActionInvoker[] actionInvokers;

        /// <summary>
        /// 创建工厂
        /// </summary>
        private readonly Func<IHttpApiInterceptor, ApiActionInvoker[], THttpApi> factory;

        /// <summary>
        /// 获取接口的所有方法
        /// </summary>
        protected MethodInfo[] ApiMethods { get; }

        /// <summary>
        /// THttpApi的实例创建器抽象
        /// </summary>
        /// <param name="actionDescriptorProvider"></param>
        /// <param name="actionInvokerProvider"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public HttpApiActivator(IApiActionDescriptorProvider actionDescriptorProvider, IApiActionInvokerProvider actionInvokerProvider)
        {
            this.ApiMethods = this.FindApiMethods();
            this.actionInvokers = this.ApiMethods
                 .Select(item => actionDescriptorProvider.CreateActionDescriptor(item, typeof(THttpApi)))
                 .Select(item => actionInvokerProvider.CreateActionInvoker(item))
                 .ToArray();

            // 最后一步创建工厂
            this.factory = this.CreateFactory();
        }

        /// <summary>
        /// 查找接口的Api方法
        /// 用于创建代理对象的ApiActionInvoker
        /// </summary>
        /// <returns></returns>
        protected virtual MethodInfo[] FindApiMethods()
        {
            return HttpApi.FindApiMethods(typeof(THttpApi));
        }

        /// <summary>
        /// 创建实例工厂
        /// </summary>
        /// <returns></returns>
        protected abstract Func<IHttpApiInterceptor, ApiActionInvoker[], THttpApi> CreateFactory();

        /// <summary>
        /// 创建接口的实例
        /// </summary>
        /// <param name="apiInterceptor">接口拦截器</param>
        /// <returns></returns>
        public THttpApi CreateInstance(IHttpApiInterceptor apiInterceptor)
        {
            return this.factory(apiInterceptor, this.actionInvokers);
        }
    }
}
