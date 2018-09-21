using NetworkSocket;
using System;

namespace Demo.HttpServices
{
    class HttpPlug : NetworkSocket.Plugs.PlugBase
    {
        protected override void OnConnected(object sender, IContenxt context)
        {
            Console.WriteLine($"HttpServer-> {DateTime.Now} {context.Session}连接..");
        }

        protected override void OnDisconnected(object sender, IContenxt context)
        {
            Console.WriteLine($"HttpServer-> {DateTime.Now} {context.Session}断开..");
        }
    }
}
