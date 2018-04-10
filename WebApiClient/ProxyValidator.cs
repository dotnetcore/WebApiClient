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
        /// 代理
        /// </summary>
        private readonly IWebProxy webProxy;

        /// <summary>
        /// 代理验证器
        /// </summary>
        /// <param name="proxyHost">代理服务器域名或ip</param>
        /// <param name="proxyPort">代理服务器端口</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyValidator(string proxyHost, int proxyPort)
        {
            this.webProxy = new HttpProxy(proxyHost, proxyPort);
        }

        /// <summary>
        /// 代理验证器
        /// </summary>
        /// <param name="webProxy">代理</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyValidator(IWebProxy webProxy)
        {
            this.webProxy = webProxy ?? throw new ArgumentNullException(nameof(webProxy));
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="targetAddress">目标地址</param>
        /// <param name="timeout">发送或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public HttpStatusCode Validate(Uri targetAddress, TimeSpan? timeout = null)
        {
            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }
            return Validate(this.webProxy, targetAddress, timeout);
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="targetAddress">目标地址</param>
        /// <param name="timeout">连接或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public Task<HttpStatusCode> ValidateAsync(Uri targetAddress, TimeSpan? timeout = null)
        {
            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }
            return ValidateAsync(this.webProxy, targetAddress, timeout);
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="webProxy">web代理</param>      
        /// <param name="targetAddress">目标url地址</param>
        /// <param name="timeout">发送或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>    
        /// <returns></returns>
        public static HttpStatusCode Validate(IWebProxy webProxy, Uri targetAddress, TimeSpan? timeout = null)
        {
            if (webProxy == null)
            {
                throw new ArgumentNullException(nameof(webProxy));
            }

            var httpProxy = webProxy as HttpProxy;
            if (httpProxy == null)
            {
                httpProxy = HttpProxy.FromWebProxy(webProxy, targetAddress);
            }

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

                var response = Encoding.ASCII.GetString(recvBuffer, 0, length);
                var statusCode = int.Parse(Regex.Match(response, "(?<=HTTP/1.1 )\\d+", RegexOptions.IgnoreCase).Value);
                return (HttpStatusCode)statusCode;
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
        /// <param name="targetAddress">目标url地址</param>
        /// <param name="timeout">连接或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>    
        /// <returns></returns>
        public static async Task<HttpStatusCode> ValidateAsync(IWebProxy webProxy, Uri targetAddress, TimeSpan? timeout = null)
        {
            if (webProxy == null)
            {
                throw new ArgumentNullException(nameof(webProxy));
            }

            var httpProxy = webProxy as HttpProxy;
            if (httpProxy == null)
            {
                httpProxy = HttpProxy.FromWebProxy(webProxy, targetAddress);
            }

            var remoteEndPoint = new DnsEndPoint(httpProxy.Host, httpProxy.Port, AddressFamily.InterNetwork);
            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await socket.ConnectTaskAsync(remoteEndPoint, timeout);

                var request = httpProxy.ToTunnelRequestString(targetAddress);
                var sendBuffer = Encoding.ASCII.GetBytes(request);
                var sendArraySegment = new List<ArraySegment<byte>> { new ArraySegment<byte>(sendBuffer) };
                await Task.Factory.FromAsync(socket.BeginSend, socket.EndSend, sendArraySegment, SocketFlags.None, null);

                var recvBufferSegment = new ArraySegment<byte>(new byte[150]);
                var length = await socket.ReceiveTaskAsync(recvBufferSegment, timeout);

                var response = Encoding.ASCII.GetString(recvBufferSegment.Array, 0, length);
                var statusCode = int.Parse(Regex.Match(response, "(?<=HTTP/1.1 )\\d+", RegexOptions.IgnoreCase).Value);
                return (HttpStatusCode)statusCode;
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
    }
}
