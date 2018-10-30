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

            Exceptions.Catch<PlatformNotSupportedException>(() =>
                this.MaxConnectionsPerServer = HttpApiClient.ConnectionLimit);

            Exceptions.Catch<PlatformNotSupportedException>(() =>
                this.ServerCertificateCustomValidationCallback = (a, b, c, d) => true);
        }
    }
}

#endif