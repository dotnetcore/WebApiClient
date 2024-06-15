using System.Diagnostics.CodeAnalysis;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义THttpApi的实例创建器的接口
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public interface IHttpApiActivator<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
#endif
    THttpApi>
    {
        /// <summary>
        /// 创建THttpApi的代理实例
        /// </summary>
        /// <param name="apiInterceptor">接口拦截器</param>
        /// <returns></returns>
        THttpApi CreateInstance(IHttpApiInterceptor apiInterceptor);
    }
}
