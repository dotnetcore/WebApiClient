using System;
using System.Diagnostics;

namespace WebApiClient
{
    /// <summary>
    /// 提供HttpApi的创建、注册和解析   
    /// </summary>
    [DebuggerTypeProxy(typeof(DebugView))]
    public abstract partial class HttpApi : IHttpApi
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
        private class DebugView : HttpApi
        {
            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="target">查看的对象</param>
            public DebugView(HttpApi target)
                : base(target.ApiInterceptor)
            {
            }
        }
    }
}
