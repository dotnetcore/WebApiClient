#if NETCOREAPP2_1
using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace WebApiClient
{
    /// <summary>
    /// 表示SocketsHttpHandler包装为IHttpClientHandler
    /// </summary>
    class HttpHandlerOfSocketsHttpHandler : IHttpHandler
    {
        /// <summary>
        /// 内部的SocketsHttpHandler
        /// </summary>
        private readonly SocketsHttpHandler socketsHandler;

        /// <summary>
        /// 获取原始的Handler对象
        /// </summary>
        public HttpMessageHandler SourceHanlder { get; private set; }


        /// <summary>
        /// 获取是否支持重定向设置
        /// </summary>
        public bool SupportsRedirectConfiguration { get; } = true;

        /// <summary>
        /// 获取是否支持代理
        /// </summary>
        public bool SupportsProxy { get; } = true;

        /// <summary>
        /// 获取是否支持压缩传输
        /// </summary>
        public bool SupportsAutomaticDecompression { get; } = true;


        /// <summary>
        /// 获取或设置最大请求内容字节长度
        /// </summary>
        public long MaxRequestContentBufferSize
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取或设置客户端证书选项
        /// </summary>
        public ClientCertificateOption ClientCertificateOptions
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取或设置是否使用默认的凭证信息
        /// </summary>
        public bool UseDefaultCredentials
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        /// <summary>
        /// 获取或设置是否使用CookieContainer来管理Cookies
        /// </summary>
        public bool UseCookies
        {
            get => this.socketsHandler.UseCookies;
            set => this.socketsHandler.UseCookies = value;
        }

        /// <summary>
        /// 获取或设置代理
        /// </summary>
        public IWebProxy Proxy
        {
            get => this.socketsHandler.Proxy;
            set => this.socketsHandler.Proxy = value;
        }

        /// <summary>
        /// 获取或设置是否对请求进行预身份验证
        /// </summary>
        public bool PreAuthenticate
        {
            get => this.socketsHandler.PreAuthenticate;
            set => this.socketsHandler.PreAuthenticate = value;
        }

        /// <summary>
        /// 获取或设置每个响应的最大重定向次数
        /// </summary>
        public int MaxAutomaticRedirections
        {
            get => this.socketsHandler.MaxAutomaticRedirections;
            set => this.socketsHandler.MaxAutomaticRedirections = value;
        }


        /// <summary>
        /// 获取或设置凭证信息
        /// </summary>
        public ICredentials Credentials
        {
            get => this.socketsHandler.Credentials;
            set => this.socketsHandler.Credentials = value;
        }

        /// <summary>
        /// 获取或设置Cookie管理容器
        /// </summary>
        public CookieContainer CookieContainer
        {
            get => this.socketsHandler.CookieContainer;
            set => this.socketsHandler.CookieContainer = value;
        }

        /// <summary>
        /// 获取或设置压缩方式
        /// </summary>
        public DecompressionMethods AutomaticDecompression
        {
            get => this.socketsHandler.AutomaticDecompression;
            set => this.socketsHandler.AutomaticDecompression = value;
        }

        /// <summary>
        /// 获取或设置是否支持自动重定向
        /// </summary>
        public bool AllowAutoRedirect
        {
            get => this.socketsHandler.AllowAutoRedirect;
            set => this.socketsHandler.AllowAutoRedirect = value;
        }


        /// <summary>
        /// 获取或设置是否使用代理
        /// </summary>
        public bool UseProxy
        {
            get => this.socketsHandler.UseProxy;
            set => this.socketsHandler.UseProxy = value;
        }


        /// <summary>
        /// 获取或设置SSL版本
        /// </summary>
        public SslProtocols SslProtocols
        {
            get => this.socketsHandler.SslOptions.EnabledSslProtocols;
            set => this.socketsHandler.SslOptions.EnabledSslProtocols = value;
        }

        /// <summary>
        /// 获取或设置默认代理凭证
        /// </summary>
        public ICredentials DefaultProxyCredentials
        {
            get => this.socketsHandler.DefaultProxyCredentials;
            set => this.socketsHandler.DefaultProxyCredentials = value;
        }

        /// <summary>
        /// 获取或设置是否验证撤销的证书
        /// </summary>
        public bool CheckCertificateRevocationList
        {
            get => this.socketsHandler.SslOptions.CertificateRevocationCheckMode != X509RevocationMode.NoCheck;
            set => this.socketsHandler.SslOptions.CertificateRevocationCheckMode = value ? X509RevocationMode.Offline : X509RevocationMode.NoCheck;
        }

        /// <summary>
        /// 获取或设置证书列表
        /// </summary>
        public X509CertificateCollection ClientCertificates
        {
            get => this.socketsHandler.SslOptions.ClientCertificates;
        }

        /// <summary>
        /// 获取或设置每个服务器的最大连接数
        /// </summary>
        public int MaxConnectionsPerServer
        {
            get => this.socketsHandler.MaxConnectionsPerServer;
            set => this.socketsHandler.MaxConnectionsPerServer = value;
        }

        /// <summary>
        /// 获取或设置最大响应头的字节长度
        /// </summary>
        public int MaxResponseHeadersLength
        {
            get => this.socketsHandler.MaxResponseHeadersLength;
            set => this.socketsHandler.MaxResponseHeadersLength = value;
        }

        /// <summary>
        /// 设置服务器证书验证委托
        /// </summary>
        public Func<X509Certificate, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback
        {
            set
            {
                bool callBack(object a, X509Certificate b, X509Chain c, SslPolicyErrors d) => value(b, c, d);
                this.socketsHandler.SslOptions.RemoteCertificateValidationCallback = callBack;
            }
        }

        /// <summary>
        /// SocketsHttpHandler包装为IHttpClientHandler
        /// </summary>
        /// <param name="socketsHandler">内部的SocketsHttpHandler</param>
        /// <param name="sourceHandler">原始Handler</param>
        public HttpHandlerOfSocketsHttpHandler(SocketsHttpHandler socketsHandler, HttpMessageHandler sourceHandler)
        {
            this.socketsHandler = socketsHandler;
            this.SourceHanlder = sourceHandler;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            this.SourceHanlder.Dispose();
        }
    }
}
#endif