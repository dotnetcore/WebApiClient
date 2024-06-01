using System;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 默认的THttpApi的实例创建器
    /// 优先使用SourceGeneratorHttpApiActivator
    /// 不支持则回退使用EmitHttpApiActivator
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class DefaultHttpApiActivator<
#if NET5_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    THttpApi> : IHttpApiActivator<THttpApi>
    {
        private readonly Lazy<EmitHttpApiActivator<THttpApi>> emitHttpApiActivatorLazy;
        private readonly SourceGeneratorHttpApiActivator<THttpApi>? sourceGeneratorHttpApiActivator;

        /// <summary>
        /// 默认的THttpApi的实例创建器
        /// </summary>
        /// <param name="apiActionDescriptorProvider"></param>
        /// <param name="actionInvokerProvider"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public DefaultHttpApiActivator(IApiActionDescriptorProvider apiActionDescriptorProvider, IApiActionInvokerProvider actionInvokerProvider)
        {
            if (SourceGeneratorHttpApiActivator<THttpApi>.IsSupported)
            {
                this.sourceGeneratorHttpApiActivator = new SourceGeneratorHttpApiActivator<THttpApi>(apiActionDescriptorProvider, actionInvokerProvider);
            }
            this.emitHttpApiActivatorLazy = new Lazy<EmitHttpApiActivator<THttpApi>>(() => new EmitHttpApiActivator<THttpApi>(apiActionDescriptorProvider, actionInvokerProvider), isThreadSafe: true);
        }

        /// <summary>
        /// 创建接口的实例
        /// </summary>
        /// <param name="apiInterceptor">接口拦截器</param>
        /// <returns></returns>
        public THttpApi CreateInstance(IHttpApiInterceptor apiInterceptor)
        {
            if (this.sourceGeneratorHttpApiActivator != null)
            {
                return this.sourceGeneratorHttpApiActivator.CreateInstance(apiInterceptor);
            }
            return this.emitHttpApiActivatorLazy.Value.CreateInstance(apiInterceptor);
        }
    }
}