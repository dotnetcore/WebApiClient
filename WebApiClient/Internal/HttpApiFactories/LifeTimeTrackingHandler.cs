using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 表示具有生命周期自动监视的Handler
    /// </summary>
    class LifeTimeTrackingHandler : DelegatingHandler
    {
        /// <summary>
        /// 具有生命周期监视的Handler
        /// </summary>
        /// <param name="innerHandler">内部的handler</param>
        public LifeTimeTrackingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected sealed override void Dispose(bool disposing)
        {
        }
    }
}
