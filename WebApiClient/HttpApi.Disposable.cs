using System.Diagnostics;

namespace WebApiClient
{
    /// <summary>
    /// 提供HttpApi的创建、注册和解析   
    /// </summary>
    [DebuggerTypeProxy(typeof(DebugView))]
    public partial class HttpApi : Disposable, IHttpApi
    {
        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing">是否也释放托管资源</param>
        protected override void Dispose(bool disposing)
        {
            this.ApiInterceptor.Dispose();
        }

        /// <summary>
        /// 调试视图
        /// </summary>
        private partial class DebugView
        {
            /// <summary>
            /// 获取对象是否已释放
            /// </summary>
            public bool IsDisposed { get; }

            /// <summary>
            /// 获取拦截器
            /// </summary>
            public IApiInterceptor ApiInterceptor { get; }

            /// <summary>
            /// 调试视图
            /// </summary>
            /// <param name="httpApi">查看的对象</param>
            public DebugView(HttpApi httpApi)
            {
                this.IsDisposed = httpApi.IsDisposed;
                this.ApiInterceptor = httpApi.ApiInterceptor;
            }
        }
    }
}
