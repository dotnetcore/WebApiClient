#if NETSTANDARD2_0 || NETCOREAPP2_0

using System.Net.Http;

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