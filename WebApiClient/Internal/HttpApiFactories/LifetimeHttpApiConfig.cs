using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示自主生命周期管理的HttpApiConfig
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
        /// 这里不释放资源
        /// </summary>
        /// <param name="disposing"></param>
        protected sealed override void Dispose(bool disposing)
        {
        }
    }
}
