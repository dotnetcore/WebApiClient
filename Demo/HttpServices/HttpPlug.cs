using NetworkSocket;
using System;

namespace Demo.HttpServices
{
    class HttpPlug : NetworkSocket.Plugs.PlugBase
    {
        protected override void OnConnected(object sender, IContenxt context)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} HttpServer->{1}连接..", DateTime.Now.ToString("HH:mm:ss.fff"), context.Session);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        protected override void OnDisconnected(object sender, IContenxt context)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0} HttpServer->{1}断开..", DateTime.Now.ToString("HH:mm:ss.fff"), context.Session);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
