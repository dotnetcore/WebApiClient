using System;

namespace WebApiClient
{
    /// <summary>
    /// 提供HttpApi的创建、注册和解析   
    /// </summary> 
    public abstract partial class HttpApi
    {
        /// <summary>
        /// 获取拦截器
        /// </summary>
        public IApiInterceptor ApiInterceptor { get; }

        /// <summary>
        /// http接口代理类的基类
        /// </summary>
        /// <param name="apiInterceptor">拦截器</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpApi(IApiInterceptor apiInterceptor)
        {
            this.ApiInterceptor = apiInterceptor ?? throw new ArgumentNullException(nameof(apiInterceptor));
        }
    }
}
