using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace WebApiClient
{
    /// <summary>
    /// 定义HttpClient的处理程序的接口
    /// </summary>
    public interface IHttpHandler : IDisposable
    {
        /// <summary>
        /// 获取内部原始的Handler对象
        /// </summary>
        HttpMessageHandler InnerHanlder { get; }

        /// <summary>
        /// 获取或设置是否使用CookieContainer来管理Cookies
        /// </summary>
        bool UseCookies { get; set; }

        /// <summary>
        /// 获取是否支持重定向设置
        /// </summary>
        bool SupportsRedirectConfiguration { get; }

        /// <summary>
        /// 获取是否支持代理
        /// </summary>
        bool SupportsProxy { get; }

        /// <summary>
        /// 获取是否支持压缩传输
        /// </summary>
        bool SupportsAutomaticDecompression { get; }

        /// <summary>
        /// 获取或设置代理
        /// </summary>
        IWebProxy Proxy { get; set; }

        /// <summary>
        /// 获取或设置是否对请求进行预身份验证
        /// </summary>
        bool PreAuthenticate { get; set; }

        /// <summary>
        /// 获取或设置每个响应的最大重定向次数
        /// </summary>
        int MaxAutomaticRedirections { get; set; }

        /// <summary>
        /// 获取或设置最大请求内容字节长度
        /// </summary>
        long MaxRequestContentBufferSize { get; set; }

        /// <summary>
        /// 获取或设置凭证信息
        /// </summary>
        ICredentials Credentials { get; set; }

        /// <summary>
        /// 获取或设置Cookie管理容器
        /// </summary>
        CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// 获取或设置客户端证书选项
        /// </summary>
        ClientCertificateOption ClientCertificateOptions { get; set; }

        /// <summary>
        /// 获取或设置压缩方式
        /// </summary>
        DecompressionMethods AutomaticDecompression { get; set; }

        /// <summary>
        /// 获取或设置是否支持自动重定向
        /// </summary>
        bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// 获取或设置是否使用默认的凭证信息
        /// </summary>
        bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// 获取或设置是否使用代理
        /// </summary>
        bool UseProxy { get; set; }

#if !NET45     
        /// <summary>
        /// 获取或设置SSL版本
        /// </summary>
        SslProtocols SslProtocols { get; set; }

        /// <summary>
        /// 获取或设置默认代理凭证
        /// </summary>
        ICredentials DefaultProxyCredentials { get; set; }

        /// <summary>
        /// 获取或设置是否验证撤销的证书
        /// </summary>
        bool CheckCertificateRevocationList { get; set; }

        /// <summary>
        /// 获取或设置证书列表
        /// </summary>
        X509CertificateCollection ClientCertificates { get; }

        /// <summary>
        /// 获取或设置每个服务器的最大连接数
        /// </summary>
        int MaxConnectionsPerServer { get; set; }

        /// <summary>
        /// 获取或设置最大响应头的字节长度
        /// </summary>
        int MaxResponseHeadersLength { get; set; }

        /// <summary>
        /// 设置服务器证书验证委托
        /// </summary>
        Func<X509Certificate, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback { set; }
#endif
    }
}
