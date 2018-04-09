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
    /// 提供代理的验证
    /// </summary>
    public static class ProxyValidate
    {
        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="proxyHost">代理服务器域名或ip</param>
        /// <param name="proxyPort">代理服务器端口</param>
        /// <param name="targetAddress">目标url地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static async Task<HttpStatusCode> ValidateAsync(string proxyHost, int proxyPort, Uri targetAddress)
        {
            return await ValidateAsync(proxyHost, proxyPort, null, null, targetAddress);
        }

        /// <summary>
        /// 使用http tunnel检测代理状态
        /// </summary>
        /// <param name="proxyHost">代理服务器域名或ip</param>
        /// <param name="proxyPort">代理服务器端口</param>
        /// <param name="userName">代理账号</param>
        /// <param name="password">代理密码</param>
        /// <param name="targetAddress">目标url地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <returns></returns>
        public static async Task<HttpStatusCode> ValidateAsync(string proxyHost, int proxyPort, string userName, string password, Uri targetAddress)
        {
            var remoteEndPoint = new DnsEndPoint(proxyHost, proxyPort, AddressFamily.InterNetwork);
            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await Task.Factory.FromAsync(socket.BeginConnect, socket.EndConnect, remoteEndPoint, null);

                var request = BuildHttpTunnelRequestString(proxyHost, proxyPort, userName, password, targetAddress);
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


        /// <summary>
        /// 生成Http Tunnel请求字符串
        /// </summary>
        /// <param name="proxyHost">代理服务器域名或ip</param>
        /// <param name="proxyPort">代理服务器端口</param>
        /// <param name="userName">代理账号</param>
        /// <param name="password">代理密码</param>
        /// <param name="targetAddress">目标url地址</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        private static string BuildHttpTunnelRequestString(string proxyHost, int proxyPort, string userName, string password, Uri targetAddress)
        {
            if (proxyHost == null)
            {
                throw new ArgumentNullException(nameof(proxyHost));
            }

            if (targetAddress == null)
            {
                throw new ArgumentNullException(nameof(targetAddress));
            }

            var builder = new StringBuilder()
                .AppendLine($"CONNECT {targetAddress.Authority} HTTP/1.1")
                .AppendLine($"Host: {targetAddress.Authority}")
                .AppendLine("Accept: */*")
                .AppendLine("Content-Type: text/html")
                .AppendLine("Proxy-Connection: Keep-Alive")
                .AppendLine("Content-length: 0");

            if (userName != null && password != null)
            {
                var bytes = Encoding.ASCII.GetBytes($"{userName}:{password}");
                var base64 = Convert.ToBase64String(bytes);
                builder.AppendLine($"Proxy-Authorization: Basic {base64}");
            }

            return builder.AppendLine().AppendLine().ToString();
        }
    }
}
