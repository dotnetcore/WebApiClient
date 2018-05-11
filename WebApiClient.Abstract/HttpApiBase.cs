using System;

namespace WebApiClient
{
    /// <summary>
    /// 表示IHttpApi实现类的抽象类
    /// </summary>
    public abstract class HttpApiBase : IHttpApi
    {
        /// <summary>
        /// Api拦截器
        /// </summary>
        private readonly IApiInterceptor interceptor;

        /// <summary>
        /// IHttpApi实现类的抽象类
        /// </summary>
        /// <param name="interceptor">Api拦截器 </param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpApiBase(IApiInterceptor interceptor)
        {
            this.interceptor = interceptor ?? throw new ArgumentNullException(nameof(interceptor));
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.interceptor.Dispose();
        }
    }
}
