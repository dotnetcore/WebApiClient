using NetworkSocket;
using NetworkSocket.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WebApiClient;

namespace Demo
{
    class Program
    {
        /// <summary>
        /// http服务器
        /// </summary>
        private static readonly TcpListener httpServer = new TcpListener();

        static void Main(string[] args)
        {
            InitHttpServer();
            RunApisAsync();
            Console.ReadLine();
        }

        static void InitHttpServer()
        {
            httpServer.Use<HttpMiddleware>();
            httpServer.UsePlug<HttpPlug>();
            httpServer.Start(9999);
        }

        static async void RunApisAsync()
        {
            var webApiClient = new HttpApiClient();
            var myWebApi = webApiClient.Implement<UserApi>();

            var user = new UserInfo { Account = "laojiu", Password = "123456" };
            var file = new MulitpartFile("head.jpg");

            var auth = "Basic eW91ck5hbWU6MTIzNDU2";
            var about = await myWebApi.GetAboutAsync("http://localhost:9999/webapi/user/about", auth, user, "some value here");
            Console.WriteLine(about);
            Console.WriteLine();


            var user1 = await myWebApi.GetByIdAsync("id001");
            Console.WriteLine(await user1.Content.ReadAsStringAsync());
            Console.WriteLine();

            var user2 = await myWebApi.GetByAccountAsync("laojiu");
            Console.WriteLine(user2);
            Console.WriteLine();

            var user3 = await myWebApi.UpdateWithFormAsync(user);
            Console.WriteLine(user3);
            Console.WriteLine();

            var user4 = await myWebApi.UpdateWithJsonAsync(user);
            Console.WriteLine(user4);
            Console.WriteLine();

            var user5 = await myWebApi.UpdateWithXmlAsync(user);
            Console.WriteLine(user5);
            Console.WriteLine();

            var user6 = await myWebApi.UpdateWithMulitpartAsync(user, file);
            Console.WriteLine(user6);

        }
    }
}
