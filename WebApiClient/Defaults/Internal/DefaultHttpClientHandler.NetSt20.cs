#if NETSTANDARD2_0 || NETCOREAPP2_0

using System;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 默认的HttpClientHandler
    /// </summary>
    class DefaultHttpClientHandler : HttpClientHandler, IHttpHandler
    {
        /// <summary>
        /// 获取内部的原始Handler对象       
        /// </summary>
        public HttpMessageHandler InnerHanlder
        {
            get
            {
                return this;
            }
        }

        /// <summary>
        /// 设置服务器证书验证委托
        /// </summary>
        Func<X509Certificate, X509Chain, SslPolicyErrors, bool> IHttpHandler.ServerCertificateCustomValidationCallback
        {
            set
            {
                Func<HttpRequestMessage, X509Certificate2, X509Chain, SslPolicyErrors, bool> callBack = (a, b, c, d) => value(b, c, d);
                base.ServerCertificateCustomValidationCallback = callBack;
            }
        }

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