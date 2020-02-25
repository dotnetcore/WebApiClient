using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// Proxy validator
    /// Provide proxy authentication
    /// </summary>
    public class ProxyValidator
    {
        /// <summary>
        /// Get Agent
        /// </summary>
        public IWebProxy WebProxy { get; }

        /// <summary>
        /// Proxy validator
        /// </summary>
        /// <param name="proxyHost">Proxy server domain name or ip</param>
        /// <param name="proxyPort">Proxy server port</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyValidator(string proxyHost, int proxyPort)
            : this(new HttpProxy(proxyHost, proxyPort))
        {
        }

        /// <summary>
        /// Proxy validator
        /// </summary>
        /// <param name="webProxy">proxy</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyValidator(IWebProxy webProxy)
        {
            this.WebProxy = webProxy ?? throw new ArgumentNullException(nameof(webProxy));
        }

        /// <summary>
        /// Detect proxy status using http tunnel
        /// </summary>
        /// <param name="targetAddress">Destination address, which can be http or https</param>
        /// <param name="timeout">Timeout for sending or waiting for data</param>
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
        /// Detect proxy status using http tunnel
        /// </summary>
        /// <param name="targetAddress">Destination address, which can be http or https</param>
        /// <param name="timeout">Timeout for connecting or waiting for data</param>
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
        /// Convert to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.WebProxy.ToString();
        }

        /// <summary>
        /// Detect proxy status using http tunnel
        /// </summary>
        /// <param name="webProxy">web proxy</param>      
        /// <param name="targetAddress">Destination address, which can be http or https</param>
        /// <param name="timeout">Timeout for sending or waiting for data</param>
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

                var recvBuffer = new byte[20];
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
        /// Detect proxy status using http tunnel
        /// </summary>
        /// <param name="webProxy">web proxy</param>      
        /// <param name="targetAddress">Destination address, which can be http or https</param>
        /// <param name="timeout">Timeout for connecting or waiting for data</param>
        /// <exception cref="ArgumentNullException"></exception>    
        /// <returns></returns>
        public static async Task<HttpStatusCode> ValidateAsync(IWebProxy webProxy, Uri targetAddress, TimeSpan? timeout = null)
        {
            var httpProxy = CastToHttpProxy(webProxy, targetAddress);
            var remoteEndPoint = new DnsEndPoint(httpProxy.Host, httpProxy.Port, AddressFamily.InterNetwork);
            var socket = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                await socket.ConnectTaskAsync(remoteEndPoint, timeout).ConfigureAwait(false);

                var request = httpProxy.ToTunnelRequestString(targetAddress);
                var sendBuffer = Encoding.ASCII.GetBytes(request);
                await socket.SendTaskAsync(new ArraySegment<byte>(sendBuffer), timeout).ConfigureAwait(false);

                var recvBufferSegment = new ArraySegment<byte>(new byte[20]);
                var length = await socket.ReceiveTaskAsync(recvBufferSegment, timeout).ConfigureAwait(false);
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
        /// IWebProxy to HttpProxy
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
        /// Parse the status code of the response
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static HttpStatusCode ParseStatusCode(byte[] buffer, int length)
        {
            var response = Encoding.ASCII.GetString(buffer, 0, length);
            if (response.StartsWith("HTTP/", StringComparison.OrdinalIgnoreCase))
            {
                var items = response.Split(' ');
                if (items.Length >= 2 && Enum.TryParse<HttpStatusCode>(items[1], out HttpStatusCode statusCode))
                {
                    return statusCode;
                }
            }
            return HttpStatusCode.BadRequest;
        }
    }
}
