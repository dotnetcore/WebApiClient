using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace WebApiClient
{
    /// <summary>
    /// Defines the interface of the HttpClient handler
    /// </summary>
    public interface IHttpHandler : IDisposable
    {
        /// <summary>
        /// Get the original Handler object
        /// </summary>
        HttpMessageHandler SourceHandler { get; }

        /// <summary>
        /// Gets or sets whether to use CookieContainer to manage cookies
        /// </summary>
        bool UseCookies { get; set; }

        /// <summary>
        /// Gets whether the redirect setting is supported
        /// </summary>
        bool SupportsRedirectConfiguration { get; }

        /// <summary>
        /// Get whether proxy is supported
        /// </summary>
        bool SupportsProxy { get; }

        /// <summary>
        /// Gets whether compressed transmission is supported
        /// </summary>
        bool SupportsAutomaticDecompression { get; }

        /// <summary>
        /// Get or set proxy
        /// </summary>
        IWebProxy Proxy { get; set; }

        /// <summary>
        /// Gets or sets whether the request is pre-authenticated
        /// </summary>
        bool PreAuthenticate { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of redirects per response
        /// </summary>
        int MaxAutomaticRedirections { get; set; }

        /// <summary>
        /// Gets or sets the maximum requested content byte length
        /// </summary>
        long MaxRequestContentBufferSize { get; set; }

        /// <summary>
        /// Get or set credential information
        /// </summary>
        ICredentials Credentials { get; set; }

        /// <summary>
        /// Gets or sets the cookie management container
        /// </summary>
        CookieContainer CookieContainer { get; set; }

        /// <summary>
        /// Gets or sets the cookie management container
        /// </summary>
        ClientCertificateOption ClientCertificateOptions { get; set; }

        /// <summary>
        /// Gets or sets the compression method
        /// </summary>
        DecompressionMethods AutomaticDecompression { get; set; }

        /// <summary>
        /// Gets or sets whether to support automatic redirection
        /// </summary>
        bool AllowAutoRedirect { get; set; }

        /// <summary>
        /// Gets or sets whether to use the default credential information
        /// </summary>
        bool UseDefaultCredentials { get; set; }

        /// <summary>
        /// Gets or sets whether to use a proxy
        /// </summary>
        bool UseProxy { get; set; }

#if !NET45     
        /// <summary>
        /// Get or set SSL version
        /// </summary>
        SslProtocols SslProtocols { get; set; }

        /// <summary>
        /// Get or set default proxy credentials
        /// </summary>
        ICredentials DefaultProxyCredentials { get; set; }

        /// <summary>
        /// Gets or sets whether to verify revoked certificates
        /// </summary>
        bool CheckCertificateRevocationList { get; set; }

        /// <summary>
        /// Get or set the certificate list
        /// </summary>
        X509CertificateCollection ClientCertificates { get; }

        /// <summary>
        /// Gets or sets the maximum number of connections per server
        /// </summary>
        int MaxConnectionsPerServer { get; set; }

        /// <summary>
        /// Gets or sets the byte length of the maximum response header
        /// </summary>
        int MaxResponseHeadersLength { get; set; }

        /// <summary>
        /// Setting up server certificate validation delegation
        /// </summary>
        Func<X509Certificate, X509Chain, SslPolicyErrors, bool> ServerCertificateCustomValidationCallback { set; }
#endif
    }
}
