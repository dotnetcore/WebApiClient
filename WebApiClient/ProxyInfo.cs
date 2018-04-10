using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示代理信息
    /// </summary>
    public class ProxyInfo
    {
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
        public string UserName { get; set; }

        /// <summary>
        /// 获取代理服务器密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 代理信息
        /// </summary>
        /// <param name="proxyAddress">代理服务器地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public ProxyInfo(string proxyAddress)
            : this(new Uri(proxyAddress ?? throw new ArgumentNullException(nameof(proxyAddress))))
        {
        }

        /// <summary>
        /// 代理信息
        /// </summary>
        /// <param name="proxyAddress">代理服务器地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyInfo(Uri proxyAddress)
        {
            if (proxyAddress == null)
            {
                throw new ArgumentNullException(nameof(proxyAddress));
            }
            this.Host = proxyAddress.Host;
            this.Port = proxyAddress.Port;
        }

        /// <summary>
        /// 代理信息
        /// </summary>
        /// <param name="host">代理服务器域名或ip</param>
        /// <param name="port">代理服务器端口</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyInfo(string host, int port)
        {
            this.Host = host ?? throw new ArgumentNullException(nameof(host));
            this.Port = Port;
        }

        /// <summary>
        /// 从IWebProxy实例转换获得
        /// </summary>
        /// <param name="webProxy">IWebProxy</param>
        /// <param name="targetAddress">目标url地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ProxyInfo FromWebProxy(IWebProxy webProxy, Uri targetAddress)
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
            var proxyInfo = new ProxyInfo(proxyAddress);

            if (webProxy.Credentials != null)
            {
                var credentials = webProxy.Credentials.GetCredential(null, null);
                proxyInfo.UserName = credentials?.UserName;
                proxyInfo.Password = credentials?.Password;
            }
            return proxyInfo;
        }

        /// <summary>
        /// 转换Http Tunnel请求字符串
        /// </summary>      
        /// <param name="targetAddress">目标url地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public string ToHttpTunnelRequestString(Uri targetAddress)
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
        /// 转换为web代理
        /// </summary>
        /// <returns></returns>
        public IWebProxy ToWebProxy()
        {
            return new WebProxy(this.Host, this.Port)
            {
                Credentials = new NetworkCredential(this.UserName, this.Password)
            };
        }
    }
}
