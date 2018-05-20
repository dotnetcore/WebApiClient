using System;
using System.Diagnostics;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi客户端
    /// 提供创建HttpApiClient实例的方法
    /// </summary>
    [DebuggerTypeProxy(typeof(DebugView))]
    public abstract partial class HttpApiClient : IHttpApiClient, IHttpApi, IDisposable
    {
        /// <summary>
        /// 获取Api拦截器
        /// </summary>
        public IApiInterceptor ApiInterceptor { get; private set; }

        /// <summary>
        /// http客户端的基类
        /// </summary>
        /// <param name="apiInterceptor">拦截器</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpApiClient(IApiInterceptor apiInterceptor)
        {
            this.ApiInterceptor = apiInterceptor ?? throw new ArgumentNullException(nameof(apiInterceptor));
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.ApiInterceptor.Dispose();
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        private class DebugView : HttpApiClient
        {
            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="target">查看的对象</param>
            public DebugView(HttpApiClient target)
                : base(target.ApiInterceptor)
            {
            }
        }
    }
}
