using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace WebApiClient
{
    /// <summary>
    /// 表示http代理信息
    /// </summary>
    public class HttpProxy : IWebProxy
    {
        /// <summary>
        /// 授权字段
        /// </summary>
        private ICredentials credentials;

        /// <summary>
        /// 获取代理服务器域名或ip
        /// </summary>
        public string Host { get; private set; }

        /// <summary>
        /// 获取代理服务器端口
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 获取代理服务器账号
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// 获取代理服务器密码
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// 获取或设置授权信息
        /// </summary>
        ICredentials IWebProxy.Credentials
        {
            get
            {
                return this.credentials;
            }
            set
            {
                this.SetCredentialsByInterface(value);
            }
        }

        /// <summary>
        /// http代理信息
        /// </summary>
        /// <param name="proxyAddress">代理服务器地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public HttpProxy(string proxyAddress)
            : this(new Uri(proxyAddress ?? throw new ArgumentNullException(nameof(proxyAddress))))
        {
        }

        /// <summary>
        /// http代理信息
        /// </summary>
        /// <param name="proxyAddress">代理服务器地址</param>
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
        /// http代理信息
        /// </summary>
        /// <param name="host">代理服务器域名或ip</param>
        /// <param name="port">代理服务器端口</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpProxy(string host, int port)
        {
            this.Host = host ?? throw new ArgumentNullException(nameof(host));
            this.Port = port;
        }

        /// <summary>
        /// http代理信息
        /// </summary>
        /// <param name="host">代理服务器域名或ip</param>
        /// <param name="port">代理服务器端口</param>
        /// <param name="userName">代理服务器账号</param>
        /// <param name="password">代理服务器密码</param>
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
        /// 通过接口设置授权信息
        /// </summary>
        /// <param name="value"></param>
        private void SetCredentialsByInterface(ICredentials value)
        {
            var userName = default(string);
            var password = default(string);
            if (value != null)
            {
                var networkCredentialsd = value.GetCredential(null, null);
                userName = networkCredentialsd?.UserName;
                password = networkCredentialsd?.Password;
            }

            this.UserName = userName;
            this.Password = password;
            this.credentials = value;
        }

        /// <summary>
        /// 转换Http Tunnel请求字符串
        /// </summary>      
        /// <param name="targetAddress">目标url地址</param>
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
        /// 获取代理服务器地址
        /// </summary>
        /// <param name="destination">目标地址</param>
        /// <returns></returns>
        public Uri GetProxy(Uri destination)
        {
            return new Uri(this.ToString());
        }

        /// <summary>
        /// 是否忽略代理
        /// </summary>
        /// <param name="host">目标地址</param>
        /// <returns></returns>
        public bool IsBypassed(Uri host)
        {
            return false;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"http://{this.Host}:{this.Port}/";
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return $"{this.Host}{this.Port}{this.UserName}{this.Port}".GetHashCode();
        }

        /// <summary>
        /// 返回和obj是否相等
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
        /// 转换为代理验证器
        /// </summary>
        /// <returns></returns>
        public ProxyValidator ToValidator()
        {
            return new ProxyValidator(this);
        }

        /// <summary>
        /// 从IWebProxy实例转换获得
        /// </summary>
        /// <param name="webProxy">IWebProxy</param>
        /// <param name="targetAddress">目标url地址</param>
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
        /// 指定ip范围构建http代理服务
        /// </summary>
        /// <param name="start">代理服务器起始ip</param>
        /// <param name="port">代理服务器端口</param>
        /// <param name="count">ip数量</param>
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
        /// 返回ip范围
        /// </summary>
        /// <param name="start">起始ip</param>
        /// <param name="count">ip数量</param>
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
        /// ip转换为int
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static int IPAddressToInt32(IPAddress ip)
        {
            var bytes = ip.GetAddressBytes();
            if (BitConverter.IsLittleEndian == true)
            {
                return BitConverter.ToInt32(bytes.Reverse().ToArray(), 0);
            }
            else
            {
                return BitConverter.ToInt32(bytes, 0);
            }
        }

        /// <summary>
        /// int转换为ip
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static IPAddress Int32ToIPAddress(int value)
        {
            if (BitConverter.IsLittleEndian == true)
            {
                var bytes = BitConverter.GetBytes(value).Reverse().ToArray();
                return new IPAddress(bytes);
            }
            else
            {
                var bytes = BitConverter.GetBytes(value);
                return new IPAddress(bytes);
            }
        }


        /// <summary>
        /// 比较两个代理是否等效
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool IsProxyEquals(IWebProxy x, IWebProxy y)
        {
            if (x == null && y == null)
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
