using NetworkSocket;
using NetworkSocket.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
            var myWebApi = HttpApiClient.Create<UserApi>();

            var user = new UserInfo { Account = "laojiu", Password = "123456" };
            var file = new MulitpartFile("NetworkSocket.dll");

            var auth = "Basic eW91ck5hbWU6MTIzNDU2";
            var url = "http://localhost:9999/webapi/user/about";
            var about = await myWebApi.GetAboutAsync(url, auth, user, "some-value");
            Console.WriteLine(about);
            Console.WriteLine();


            var user1 = await myWebApi.GetByIdAsync("id001");
            Console.WriteLine(await user1.Content.ReadAsStringAsync());
            Console.WriteLine();

            var user2 = await myWebApi.GetByAccountAsync("laojiu");
            Console.WriteLine(user2);
            Console.WriteLine();

            var user3 = await myWebApi.UpdateWithFormAsync(user, nickName: "老九", nullableAge: 18)
                .Retry(3, i => TimeSpan.FromSeconds(i))
                .WhenCatch<TimeoutException>()
                .WhenCatch<HttpRequestException>()
                .WhenResult(u => u.Account != "laojiu")

                .Handle()
                .WhenCatch<RetryException>(ex => new UserInfo { Account = "RetryException" })
                .WhenCatch<Exception>(ex => new UserInfo { Account = "Exception" })
            ;

            Console.WriteLine(user3);
            Console.WriteLine();

            var user4 = await myWebApi.UpdateWithJsonAsync(user);
            Console.WriteLine(user4);
            Console.WriteLine();

            var user5 = await myWebApi.UpdateWithXmlAsync(user);
            Console.WriteLine(user5);
            Console.WriteLine();

            var user6 = await myWebApi.UpdateWithMulitpartAsync(user, "老九", 18, file);
            Console.WriteLine(user6);

            myWebApi.Dispose();
        }
    }
}
