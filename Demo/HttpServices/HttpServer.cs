using NetworkSocket;
using NetworkSocket.Http;

namespace Demo.HttpServices
{
    /// <summary>
    /// 表示http服务器
    /// </summary>
    static class HttpServer
    {
        /// <summary>
        /// http服务器
        /// </summary>
        private static readonly TcpListener httpServer = new TcpListener();

        /// <summary>
        /// 启动Http服务
        /// </summary>
        /// <param name="port">服务端口</param>
        public static void Start(int port = 9999)
        {
            httpServer.Use<HttpMiddleware>();
            httpServer.UsePlug<HttpPlug>();
            httpServer.Start(port);
        }
    }
}
