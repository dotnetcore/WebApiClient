using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 默认的THttpApi的实例创建器
    /// 优先使用SourceGeneratorHttpApiActivator
    /// 不支持则回退使用EmitHttpApiActivator
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class DefaultHttpApiActivator<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] THttpApi>
        : IHttpApiActivator<THttpApi>
    {
        private readonly IHttpApiActivator<THttpApi> httpApiActivator;

        /// <summary>
        /// 默认的THttpApi的实例创建器
        /// </summary>
        /// <param name="apiActionDescriptorProvider"></param>
        /// <param name="actionInvokerProvider"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "ILEmitHttpApiActivator使用之前已经使用RuntimeFeature.IsDynamicCodeCompiled来判断")]
        public DefaultHttpApiActivator(IApiActionDescriptorProvider apiActionDescriptorProvider, IApiActionInvokerProvider actionInvokerProvider)
        {
            if (SourceGeneratorHttpApiActivator<THttpApi>.IsSupported)
            {
                this.httpApiActivator = new SourceGeneratorHttpApiActivator<THttpApi>(apiActionDescriptorProvider, actionInvokerProvider);
            }
            else if (RuntimeFeature.IsDynamicCodeCompiled)
            {
                this.httpApiActivator = new ILEmitHttpApiActivator<THttpApi>(apiActionDescriptorProvider, actionInvokerProvider);
            }
            else
            {
                throw new ProxyTypeCreateException(typeof(HttpApi));
            }
        }

        /// <summary>
        /// 创建接口的实例
        /// </summary>
        /// <param name="apiInterceptor">接口拦截器</param>
        /// <returns></returns>
        public THttpApi CreateInstance(IHttpApiInterceptor apiInterceptor)
        {
            return this.httpApiActivator.CreateInstance(apiInterceptor);
        }
    }
}