using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 代理验证器
    /// 提供代理的验证
    /// </summary>
    public class ProxyValidator
    {
        /// <summary>
        /// 获取代理
        /// </summary>
        public IWebProxy WebProxy { get; private set; }

        /// <summary>
        /// 代理验证器
        /// </summary>
        /// <param name="proxyHost">代理服务器域名或ip</param>
        /// <param name="proxyPort">代理服务器端口</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyValidator(string proxyHost, int proxyPort)
            : this(new HttpProxy(proxyHost, proxyPort))
        {
        }

        /// <summary>
        /// 代理验证器
        /// </summary>
        /// <param name="webProxy">代理</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyValidator(IWebProxy webProxy)
        {
            this.WebProxy = webProxy ?? throw new ArgumentNullException(nameof(webProxy));
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="targetAddress">目标地址，可以是http或https</param>
        /// <param name="timeout">发送或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public HttpStatusCode Validate(Uri targetAddress, TimeSpan? timeout = null)
        {
            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }
            return Validate(this.WebProxy, targetAddress, timeout);
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="targetAddress">目标地址，可以是http或https</param>
        /// <param name="timeout">连接或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public Task<HttpStatusCode> ValidateAsync(Uri targetAddress, TimeSpan? timeout = null)
        {
            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }
            return ValidateAsync(this.WebProxy, targetAddress, timeout);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.WebProxy.ToString();
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="webProxy">web代理</param>      
        /// <param name="targetAddress">目标地址，可以是http或https</param>
        /// <param name="timeout">发送或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>    
        /// <returns></returns>
        public static HttpStatusCode Validate(IWebProxy webProxy, Uri targetAddress, TimeSpan? timeout = null)
        {
            var httpProxy = CastToHttpProxy(webProxy, targetAddress);
            var remoteEndPoint = new DnsEndPoint(httpProxy.Host, httpProxy.Port, AddressFamily.InterNetwork);
            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                if (timeout.HasValue == true)
                {
                    socket.SendTimeout = (int)timeout.Value.TotalMilliseconds;
                    socket.ReceiveTimeout = (int)timeout.Value.TotalMilliseconds;
                }
                socket.Connect(remoteEndPoint);

                var request = httpProxy.ToTunnelRequestString(targetAddress);
                var sendBuffer = Encoding.ASCII.GetBytes(request);
                socket.Send(sendBuffer);

                var recvBuffer = new byte[150];
                var length = socket.Receive(recvBuffer);
                return ParseStatusCode(recvBuffer, length);
            }

            catch (Exception)
            {
                return HttpStatusCode.ServiceUnavailable;
            }
            finally
            {
                socket.Dispose();
            }
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="webProxy">web代理</param>      
        /// <param name="targetAddress">目标地址，可以是http或https</param>
        /// <param name="timeout">连接或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>    
        /// <returns></returns>
        public static async Task<HttpStatusCode> ValidateAsync(IWebProxy webProxy, Uri targetAddress, TimeSpan? timeout = null)
        {
            var httpProxy = CastToHttpProxy(webProxy, targetAddress);
            var remoteEndPoint = new DnsEndPoint(httpProxy.Host, httpProxy.Port, AddressFamily.InterNetwork);
            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await socket.ConnectTaskAsync(remoteEndPoint, timeout);

                var request = httpProxy.ToTunnelRequestString(targetAddress);
                var sendBuffer = Encoding.ASCII.GetBytes(request);
                await socket.SendTaskAsync(new ArraySegment<byte>(sendBuffer), timeout);

                var recvBufferSegment = new ArraySegment<byte>(new byte[150]);
                var length = await socket.ReceiveTaskAsync(recvBufferSegment, timeout);
                return ParseStatusCode(recvBufferSegment.Array, length);
            }
            catch (Exception)
            {
                return HttpStatusCode.ServiceUnavailable;
            }
            finally
            {
                socket.Dispose();
            }
        }

        /// <summary>
        /// IWebProxy转换为HttpProxy
        /// </summary>
        /// <param name="webProxy"></param>
        /// <param name="targetAddress"></param>
        /// <returns></returns>
        private static HttpProxy CastToHttpProxy(IWebProxy webProxy, Uri targetAddress)
        {
            if (webProxy == null)
            {
                throw new ArgumentNullException(nameof(webProxy));
            }

            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }

            if (webProxy is HttpProxy httpProxy)
            {
                return httpProxy;
            }
            return HttpProxy.FromWebProxy(webProxy, targetAddress);
        }

        /// <summary>
        /// 解析响应的状态码
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static HttpStatusCode ParseStatusCode(byte[] buffer, int length)
        {
            var response = Encoding.ASCII.GetString(buffer, 0, length);
            var match = Regex.Match(response, "(?<=HTTP/1.1 )\\d+", RegexOptions.IgnoreCase);
            if (match.Success == false)
            {
                return HttpStatusCode.BadRequest;
            }
            var statusCode = int.Parse(match.Value);
            return (HttpStatusCode)statusCode;
        }
    }
}
