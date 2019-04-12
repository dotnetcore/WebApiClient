using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpClientHandler包装为IHttpClientHandler
    /// </summary>
    class HttpHandlerOfHttpClientHandler : IHttpHandler
    {
        /// <summary>
        /// 内部的HttpClientHandler
        /// </summary>
        private readonly HttpClientHandler clientHandler;

        /// <summary>
        /// 获取原始的Handler对象
        /// </summary>
        public HttpMessageHandler SourceHandler { get; private set; }

        /// <summary>
        /// 获取或设置是否使用CookieContainer来管理Cookies
        /// </summary>
        public bool UseCookies
        {
            get => this.clientHandler.UseCookies;
            set => this.clientHandler.UseCookies = value;
        }

        /// <summary>
        /// 获取是否支持重定向设置
        /// </summary>
        public bool SupportsRedirectConfiguration
        {
            get => this.clientHandler.SupportsRedirectConfiguration;
        }

        /// <summary>
        /// 获取是否支持代理
        /// </summary>
        public bool SupportsProxy
        {
            get => this.clientHandler.SupportsProxy;
        }

        /// <summary>
        /// 获取是否支持压缩传输
        /// </summary>
        public bool SupportsAutomaticDecompression
        {
            get => this.clientHandler.SupportsAutomaticDecompression;
        }

        /// <summary>
        /// 获取或设置代理
        /// </summary>
        public IWebProxy Proxy
        {
            get => this.clientHandler.Proxy;
            set => this.clientHandler.Proxy = value;
        }

        /// <summary>
        /// 获取或设置是否对请求进行预身份验证
        /// </summary>
        public bool PreAuthenticate
        {
            get => this.clientHandler.PreAuthenticate;
            set => this.clientHandler.PreAuthenticate = value;
        }

        /// <summary>
        /// 获取或设置每个响应的最大重定向次数
        /// </summary>
        public int MaxAutomaticRedirections
        {
            get => this.clientHandler.MaxAutomaticRedirections;
            set => this.clientHandler.MaxAutomaticRedirections = value;
        }

        /// <summary>
        /// 获取或设置最大请求内容字节长度
        /// </summary>
        public long MaxRequestContentBufferSize
        {
            get => this.clientHandler.MaxRequestContentBufferSize;
            set => this.clientHandler.MaxRequestContentBufferSize = value;
        }

        /// <summary>
        /// 获取或设置凭证信息
        /// </summary>
        public ICredentials Credentials
        {
            get => this.clientHandler.Credentials;
            set => this.clientHandler.Credentials = value;
        }

        /// <summary>
        /// 获取或设置Cookie管理容器
        /// </summary>
        public CookieContainer CookieContainer
        {
            get => this.clientHandler.CookieContainer;
            set => this.clientHandler.CookieContainer = value;
        }

        /// <summary>
        /// 获取或设置客户端证书选项
        /// </summary>
        public ClientCertificateOption ClientCertificateOptions
        {
            get => this.clientHandler.ClientCertificateOptions;
            set => this.clientHandler.ClientCertificateOptions = value;
        }

        /// <summary>
        /// 获取或设置压缩方式
        /// </summary>
        public DecompressionMethods AutomaticDecompression
        {
            get => this.clientHandler.AutomaticDecompression;
            set => this.clientHandler.AutomaticDecompression = value;
        }

        /// <summary>
        /// 获取或设置是否支持自动重定向
        /// </summary>
        public bool AllowAutoRedirect
        {
            get => this.clientHandler.AllowAutoRedirect;
            set => this.clientHandler.AllowAutoRedirect = value;
        }

        /// <summary>
        /// 获取或设置是否使用默认的凭证信息
        /// </summary>
        public bool UseDefaultCredentials
        {
            get => this.clientHandler.UseDefaultCredentials;
            set => this.clientHandler.UseDefaultCredentials = value;
        }

        /// <summary>
        /// 获取或设置是否使用代理
        /// </summary>
        public bool UseProxy
        {
            get => this.clientHandler.UseProxy;
            set => this.clientHandler.UseProxy = value;
        }

#if !NET45
        /// <summary>
        /// 获取或设置SSL版本
        /// </summary>
        public SslProtocols SslProtocols
        {
            get => this.clientHandler.SslProtocols;
            set => this.clientHandler.SslProtocols = value;
        }

        /// <summary>
        /// 获取或设置默认代理凭证
        /// </summary>
        public ICredentials DefaultProxyCredentials
        {
            get => this.clientHandler.DefaultProxyCredentials;
            set => this.clientHandler.DefaultProxyCredentials = value;
        }

        /// <summary>
        /// 获取或设置是否验证撤销的证书
        /// </summary>
        public bool CheckCertificateRevocationList
        {
            get => this.clientHandler.CheckCertificateRevocationList;
            set => this.clientHandler.CheckCertificateRevocationList = value;
        }

        /// <summary>
        /// 获取或设置证书列表
        /// </summary>
        public X509CertificateCollection ClientCertificates
        {
            get => this.clientHandler.ClientCertificates;
        }

        /// <summary>
        /// 获取或设置每个服务器的最大连接数
        /// </summary>
        public int MaxConnectionsPerServer
        {
            get => this.clientHandler.MaxConnectionsPerServer;
            set => this.clientHandler.MaxConnectionsPerServer = value;
        }

        /// <summary>
        /// 获取或设置最大响应头的字节长度
        /// </summary>
        public int MaxResponseHeadersLength
        {
            get => this.clientHandler.MaxResponseHeadersLength;
            set => this.clientHandler.MaxResponseHeadersLength = value;
        }

        /// <summary>
        /// 设置服务器证书验证委托
        /// </summary>
        public Func<X509Certificate, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback
        {
            set
            {
                bool callBack(HttpRequestMessage a, X509Certificate2 b, X509Chain c, SslPolicyErrors d) => value(b, c, d);
                this.clientHandler.ServerCertificateCustomValidationCallback = callBack;
            }
        }
#endif

        /// <summary>
        /// HttpClientHandler包装为IHttpClientHandler
        /// </summary>
        /// <param name="clientHandler">内部的clientHandler</param>
        /// <param name="sourceHandler">原始Handler</param>
        public HttpHandlerOfHttpClientHandler(HttpClientHandler clientHandler, HttpMessageHandler sourceHandler)
        {
            this.clientHandler = clientHandler;
            this.SourceHandler = sourceHandler;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.SourceHandler.Dispose();
        }
    }
}