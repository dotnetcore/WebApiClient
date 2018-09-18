#if !NET45

using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace WebApiClient
{
    /// <summary>
    /// 默认的HttpClientHandler
    /// </summary>
    class DefaultHttpClientHandler : HttpClientHandler
    {
        /// <summary>
        /// HttpClientHandler
        /// </summary>
        public DefaultHttpClientHandler()
        {
            this.UseProxy = false;
            this.Proxy = null;
            this.ServerCertificateCustomValidationCallback = (a, b, c, d) => true;
            this.MaxConnectionsPerServer = HttpApiClient.ConnectionLimit;
        }
    }
}

#endif