using Demo.HttpClients;
using Demo.HttpServices;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebApiClient;
using WebApiClient.Parameterables;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpServer.Start(9999);
            Program.RunIUserApi(1);
            Console.ReadLine();
        }

        static async void RunIUserApi(int loop = 1)
        {
            var watch = new Stopwatch();
            watch.Start();

            using (var client = HttpApiClient.Create<IUserApi>())
            {
                for (var i = 0; i < loop; i++)
                {
                    await Program.RunApisAsync(client);
                }
            }

            watch.Stop();
            Console.WriteLine($"总共耗时：{watch.Elapsed}");
        }

        /// <summary>
        /// 执行一遍所有请求接口
        /// </summary>
        private static async Task RunApisAsync(IUserApi userApiClient)
        {
            var user = new UserInfo
            {
                Account = "laojiu",
                Password = "123456",
                BirthDay = DateTime.Parse("2018-01-01 12:30:30"),
                Email = "laojiu@webapiclient.com",
                Gender = Gender.Male
            };

            var aboutResult = await userApiClient.GetAboutAsync(
                "http://localhost:9999/webapi/user/about",
                "Basic eW91ck5hbWU6MTIzNDU2", user, "some -value");

            var user1 = await userApiClient.GetByIdAsync("id001");
            var user2 = await userApiClient.GetByAccountAsync("laojiu");

            // Retry & Handle
            var user3 = await userApiClient.UpdateWithFormAsync(user, nickName: "老九", nullableAge: null)
                .Retry(3, i => TimeSpan.FromSeconds(i))
                .WhenCatch<Exception>()
                .WhenResult(u => u == null || u.Account == null)
                .HandleAsDefaultWhenException();

            var user4 = await userApiClient.UpdateWithJsonAsync(user);
            var user5 = await userApiClient.UpdateWithXmlAsync(user);

            // Upload Files
            var stream = typeof(Program).Assembly.GetManifestResourceStream("Demo.HttpClients.about.txt");
            var file = new MulitpartFile(stream, "about.txt");
            var user6 = await userApiClient.UpdateWithMulitpartAsync(user, "老九", 18, file);
        }
    }
}
