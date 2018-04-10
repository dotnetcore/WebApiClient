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
        private readonly IWebProxy proxy;

        /// <summary>
        /// 代理验证器
        /// </summary>
        /// <param name="proxy">代理</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyValidator(IWebProxy proxy)
        {
            this.proxy = proxy ?? throw new ArgumentNullException(nameof(proxy));
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="targetAddress">目标地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public HttpStatusCode Validate(Uri targetAddress)
        {
            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }

            var proxyInfo = ProxyInfo.FromWebProxy(this.proxy, targetAddress);
            return ProxyValidator.Validate(proxyInfo, targetAddress);
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="targetAddress">目标地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public Task<HttpStatusCode> ValidateAsync(Uri targetAddress)
        {
            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }

            var proxyInfo = ProxyInfo.FromWebProxy(this.proxy, targetAddress);
            return ProxyValidator.ValidateAsync(proxyInfo, targetAddress);
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="proxyInfo">代理服务器信息</param>      
        /// <param name="targetAddress">目标url地址</param>
        /// <exception cref="ArgumentNullException"></exception>    
        /// <returns></returns>
        public static HttpStatusCode Validate(ProxyInfo proxyInfo, Uri targetAddress)
        {
            if (proxyInfo == null)
            {
                throw new ArgumentNullException(nameof(proxyInfo));
            }

            var remoteEndPoint = new DnsEndPoint(proxyInfo.Host, proxyInfo.Port, AddressFamily.InterNetwork);
            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.SendTimeout = 3 * 1000;
                socket.ReceiveTimeout = 5 * 1000;
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
        /// <exception cref="ArgumentNullException"></exception>    
        /// <returns></returns>
        public static async Task<HttpStatusCode> ValidateAsync(ProxyInfo proxyInfo, Uri targetAddress)
        {
            if (proxyInfo == null)
            {
                throw new ArgumentNullException(nameof(proxyInfo));
            }

            var remoteEndPoint = new DnsEndPoint(proxyInfo.Host, proxyInfo.Port, AddressFamily.InterNetwork);
            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.SendTimeout = 3 * 1000;
                socket.ReceiveTimeout = 5 * 1000;
                await Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, remoteEndPoint, null);

                var request = proxyInfo.ToHttpTunnelRequestString(targetAddress);
                var sendBuffer = Encoding.ASCII.GetBytes(request);
                var sendArraySegment = new List<ArraySegment<byte>> { new ArraySegment<byte>(sendBuffer) };
                await Task.Factory.FromAsync(socket.BeginSend, socket.EndSend, sendArraySegment, SocketFlags.None, null);

                var recvArraySegment = new List<ArraySegment<byte>> { new ArraySegment<byte>(new byte[150]) };
                var length = await Task.Factory.FromAsync(socket.BeginReceive, socket.EndReceive, recvArraySegment, SocketFlags.None, null);

                var response = Encoding.ASCII.GetString(recvArraySegment[0].Array, 0, length);
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
