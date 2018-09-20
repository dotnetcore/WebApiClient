using NetworkSocket;
using System;

namespace Demo.HttpServices
{
    class HttpPlug : NetworkSocket.Plugs.PlugBase
    {
        protected override void OnConnected(object sender, IContenxt context)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"HttpServer-> {DateTime.Now} {context.Session}连接..");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        protected override void OnDisconnected(object sender, IContenxt context)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"HttpServer-> {DateTime.Now} {context.Session}断开..");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
