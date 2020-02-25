using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WebApiClient
{
    /// <summary>
    /// Represents http proxy information
    /// </summary>
    public class HttpProxy : IWebProxy
    {
        /// <summary>
        /// Authorization field
        /// </summary>
        private ICredentials credentials;

        /// <summary>
        /// Get proxy server domain name or ip
        /// </summary>
        public string Host { get; }

        /// <summary>
        /// Get the proxy server port
        /// </summary>
        public int Port { get; }

        /// <summary>
        /// Get proxy server account
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// Get proxy server password
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// Get or set authorization information
        /// </summary>
        ICredentials IWebProxy.Credentials
        {
            get => this.credentials;
            set => this.SetCredentialsByInterface(value);
        }

        /// <summary>
        /// http proxy information
        /// </summary>
        /// <param name="proxyAddress">Proxy server address</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public HttpProxy(string proxyAddress)
            : this(new Uri(proxyAddress ?? throw new ArgumentNullException(nameof(proxyAddress))))
        {
        }

        /// <summary>
        /// http proxy information
        /// </summary>
        /// <param name="proxyAddress">Proxy server address</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpProxy(Uri proxyAddress)
        {
            if (proxyAddress == null)
            {
                throw new ArgumentNullException(nameof(proxyAddress));
            }
            this.Host = proxyAddress.Host;
            this.Port = proxyAddress.Port;
        }

        /// <summary>
        /// http proxy information
        /// </summary>
        /// <param name="host">Proxy server domain name or ip</param>
        /// <param name="port">Proxy server port</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpProxy(string host, int port)
        {
            this.Host = host ?? throw new ArgumentNullException(nameof(host));
            this.Port = port;
        }

        /// <summary>
        /// http proxy information
        /// </summary>
        /// <param name="host">Proxy server domain name or ip</param>
        /// <param name="port">Proxy server port</param>
        /// <param name="userName">Proxy server account</param>
        /// <param name="password">Proxy server password</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpProxy(string host, int port, string userName, string password)
            : this(host, port)
        {
            this.UserName = userName;
            this.Password = password;

            if (string.IsNullOrEmpty(userName + password) == false)
            {
                this.credentials = new NetworkCredential(userName, password);
            }
        }

        /// <summary>
        /// Setting authorization information through the interface
        /// </summary>
        /// <param name="value"></param>
        private void SetCredentialsByInterface(ICredentials value)
        {
            var userName = default(string);
            var password = default(string);
            if (value != null)
            {
                var networkCredentials = value.GetCredential(null, null);
                userName = networkCredentials?.UserName;
                password = networkCredentials?.Password;
            }

            this.UserName = userName;
            this.Password = password;
            this.credentials = value;
        }

        /// <summary>
        /// Convert Http Tunnel request string
        /// </summary>      
        /// <param name="targetAddress">Destination url</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public string ToTunnelRequestString(Uri targetAddress)
        {
            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }

            const string CRLF = "\r\n";
            var builder = new StringBuilder()
                .Append($"CONNECT {targetAddress.Host}:{targetAddress.Port} HTTP/1.1{CRLF}")
                .Append($"Host: {targetAddress.Host}:{targetAddress.Port}{CRLF}")
                .Append($"Accept: */*{CRLF}")
                .Append($"Content-Type: text/html{CRLF}")
                .Append($"Proxy-Connection: Keep-Alive{CRLF}")
                .Append($"Content-length: 0{CRLF}");

            if (this.UserName != null && this.Password != null)
            {
                var bytes = Encoding.ASCII.GetBytes($"{this.UserName}:{this.Password}");
                var base64 = Convert.ToBase64String(bytes);
                builder.AppendLine($"Proxy-Authorization: Basic {base64}{CRLF}");
            }
            return builder.Append(CRLF).ToString();
        }

        /// <summary>
        /// Get proxy server address
        /// </summary>
        /// <param name="destination">target address</param>
        /// <returns></returns>
        public Uri GetProxy(Uri destination)
        {
            return new Uri(this.ToString());
        }

        /// <summary>
        /// Whether to ignore proxies
        /// </summary>
        /// <param name="host">target address</param>
        /// <returns></returns>
        public bool IsBypassed(Uri host)
        {
            return false;
        }

        /// <summary>
        /// Convert to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"http://{this.Host}:{this.Port}/";
        }

        /// <summary>
        /// Get the hash value
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return $"{this.Host}{this.Port}{this.UserName}{this.Password}".GetHashCode();
        }

        /// <summary>
        /// Returns whether it is equal to obj
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is HttpProxy proxy)
            {
                return proxy.GetHashCode() == this.GetHashCode();
            }
            return false;
        }

        /// <summary>
        /// Return and obj converted to proxy validator
        /// </summary>
        /// <returns></returns>
        public ProxyValidator ToValidator()
        {
            return new ProxyValidator(this);
        }

        /// <summary>
        /// Obtained from IWebProxy instance conversion
        /// </summary>
        /// <param name="webProxy">IWebProxy</param>
        /// <param name="targetAddress">Destination url</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static HttpProxy FromWebProxy(IWebProxy webProxy, Uri targetAddress)
        {
            if (webProxy == null)
            {
                throw new ArgumentNullException(nameof(webProxy));
            }

            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }

            var proxyAddress = webProxy.GetProxy(targetAddress);
            var httpProxy = new HttpProxy(proxyAddress);
            httpProxy.SetCredentialsByInterface(webProxy.Credentials);

            return httpProxy;
        }


        /// <summary>
        /// Specify the IP range to build the http proxy service
        /// </summary>
        /// <param name="start">Proxy server start ip</param>
        /// <param name="port">Proxy server port</param>
        /// <param name="count">ip count</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static IEnumerable<HttpProxy> Range(IPAddress start, int port, int count)
        {
            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            foreach (var ip in GetIPAddressRange(start, count))
            {
                yield return new HttpProxy(ip.ToString(), port);
            }
        }

        /// <summary>
        /// Return ip range
        /// </summary>
        /// <param name="start">Starting ip</param>
        /// <param name="count">ip count</param>
        /// <returns></returns>
        private static IEnumerable<IPAddress> GetIPAddressRange(IPAddress start, int count)
        {
            var c = 0;
            var ip = start;

            while (c++ < count)
            {
                yield return ip;
                var next = IPAddressToInt32(ip) + 1;
                ip = Int32ToIPAddress(next);
            }
        }

        /// <summary>
        /// ip to int
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static int IPAddressToInt32(IPAddress ip)
        {
            var value = BitConverter.ToInt32(ip.GetAddressBytes(), 0);
            if (BitConverter.IsLittleEndian == true)
            {
                value = IPAddress.NetworkToHostOrder(value);
            }
            return value;
        }

        /// <summary>
        /// int to ip
        /// </summary>
        /// <param name="value">value</param>
        /// <returns></returns>
        private static IPAddress Int32ToIPAddress(int value)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                value = IPAddress.HostToNetworkOrder(value);
            }
            return new IPAddress(value);
        }

        /// <summary>
        /// Compare if two agents are equivalent
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsProxyEquals(IWebProxy x, IWebProxy y)
        {
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            var destination = new Uri("http://www.webapiclient.com");
            var xProxy = FromWebProxy(x, destination);
            var yProxy = FromWebProxy(y, destination);
            return xProxy.GetHashCode() == yProxy.GetHashCode();
        }
    }
}
