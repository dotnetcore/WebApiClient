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
        /// 代理信息
        /// </summary>
        private readonly ProxyInfo proxyInfo;

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
        /// 代理验证器
        /// </summary>
        /// <param name="proxyInfo">代理信息</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyValidator(ProxyInfo proxyInfo)
        {
            this.proxyInfo = proxyInfo ?? throw new ArgumentNullException(nameof(proxyInfo));
        }

        /// <summary>
        /// 获取代理信息
        /// </summary>
        /// <param name="targetAddress"></param>
        /// <returns></returns>
        private ProxyInfo GetProxy(Uri targetAddress)
        {
            if (this.webProxy != null)
            {
                return ProxyInfo.FromWebProxy(this.webProxy, targetAddress);
            }
            else
            {
                return this.proxyInfo;
            }
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="targetAddress">目标地址</param>
        /// <param name="timeout">发送或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public HttpStatusCode Validate(Uri targetAddress, TimeSpan timeout)
        {
            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }

            var proxyInfo = this.GetProxy(targetAddress);
            return ProxyValidator.Validate(proxyInfo, targetAddress, timeout);
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="targetAddress">目标地址</param>
        /// <param name="timeout">连接或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public Task<HttpStatusCode> ValidateAsync(Uri targetAddress, TimeSpan timeout)
        {
            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }

            var proxyInfo = this.GetProxy(targetAddress);
            return ProxyValidator.ValidateAsync(proxyInfo, targetAddress, timeout);
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="proxyInfo">代理服务器信息</param>      
        /// <param name="targetAddress">目标url地址</param>
        /// <param name="timeout">发送或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>    
        /// <returns></returns>
        public static HttpStatusCode Validate(ProxyInfo proxyInfo, Uri targetAddress, TimeSpan timeout)
        {
            if (proxyInfo == null)
            {
                throw new ArgumentNullException(nameof(proxyInfo));
            }

            var remoteEndPoint = new DnsEndPoint(proxyInfo.Host, proxyInfo.Port, AddressFamily.InterNetwork);
            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.SendTimeout = (int)timeout.TotalMilliseconds;
                socket.ReceiveTimeout = (int)timeout.TotalMilliseconds;
                socket.Connect(remoteEndPoint);

                var request = proxyInfo.ToHttpTunnelRequestString(targetAddress);
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
        /// <param name="proxyInfo">代理服务器信息</param>      
        /// <param name="targetAddress">目标url地址</param>
        /// <param name="timeout">连接或等待数据的超时时间</param>
        /// <exception cref="ArgumentNullException"></exception>    
        /// <returns></returns>
        public static async Task<HttpStatusCode> ValidateAsync(ProxyInfo proxyInfo, Uri targetAddress, TimeSpan timeout)
        {
            if (proxyInfo == null)
            {
                throw new ArgumentNullException(nameof(proxyInfo));
            }

            var remoteEndPoint = new DnsEndPoint(proxyInfo.Host, proxyInfo.Port, AddressFamily.InterNetwork);
            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await socket.ConnectTaskAsync(remoteEndPoint, timeout);

                var request = proxyInfo.ToHttpTunnelRequestString(targetAddress);
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
