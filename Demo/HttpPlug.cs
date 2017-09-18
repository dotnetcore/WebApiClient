using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkSocket;

namespace Demo
{
    class HttpPlug : IPlug
    {
        public void OnAuthenticated(object sender, IContenxt context)
        {
        }

        public void OnConnected(object sender, IContenxt context)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} {1}连接..", DateTime.Now.ToString("HH:mm:ss.fff"), context.Session);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void OnDisconnected(object sender, IContenxt context)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0} {1}断开..", DateTime.Now.ToString("HH:mm:ss.fff"), context.Session);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void OnException(object sender, Exception exception)
        {
        }

        public void OnRequested(object sender, IContenxt context)
        {
        }
    }
}
