using System.Diagnostics;

namespace WebApiClient
{
    /// <summary>
    /// Provides creation, registration, and parsing of HttpApi
    /// </summary>
    [DebuggerTypeProxy(typeof(DebugView))]
    public partial class HttpApi : Disposable, IHttpApi
    {
        /// <summary>
        /// Release resources
        /// </summary>
        /// <param name="disposing">Whether to release managed resources</param>
        protected override void Dispose(bool disposing)
        {
            this.ApiInterceptor.Dispose();
        }

        /// <summary>
        /// Debug view
        /// </summary>
        private partial class DebugView
        {
            /// <summary>
            /// Gets whether the object is released
            /// </summary>
            public bool IsDisposed { get; }

            /// <summary>
            /// Get the interceptor
            /// </summary>
            public IApiInterceptor ApiInterceptor { get; }

            /// <summary>
            /// Debug view
            /// </summary>
            /// <param name="httpApi">Viewed</param>
            public DebugView(HttpApi httpApi)
            {
                this.IsDisposed = httpApi.IsDisposed;
                this.ApiInterceptor = httpApi.ApiInterceptor;
            }
        }
    }
}
