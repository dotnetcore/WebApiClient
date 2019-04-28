using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 表示自主管理生命周期的HttpApiConfig
    /// </summary>
    class LifetimeHttpApiConfig : HttpApiConfig
    {
        /// <summary>
        /// 自主生命周期管理的HttpApiConfig
        /// </summary>
        /// <param name="handler"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public LifetimeHttpApiConfig(HttpMessageHandler handler)
            : base(handler, false)
        {
        }

        /// <summary>
        /// 在垃圾回收时释放HttpClient
        /// </summary>
        ~LifetimeHttpApiConfig()
        {
            base.Dispose(false);
        }

        /// <summary>
        /// 这里不释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected sealed override void Dispose(bool disposing)
        {
        }
    }
}
