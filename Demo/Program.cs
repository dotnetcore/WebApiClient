using Demo.HttpClients;
using Demo.HttpServices;
using System;
using System.Net.Http;
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
            Program.RunIUserApi();
            Console.ReadLine();
        }


        static async void RunIUserApi()
        {
            // 创建接口客户端
            using (var client = HttpApiClient.Create<IUserApi>())
            {
                await Program.RunApisAsync(client);
            }
        }

        /// <summary>
        /// 执行一遍所有请求接口
        /// </summary>
        private static async Task RunApisAsync(IUserApi userApiClient)
        {
            var file = new MulitpartFile("NetworkSocket.dll");
            var user = new UserInfo
            {
                Account = "laojiu",
                Password = "123456",
                BirthDay = DateTime.Parse("2018-01-01 12:30:30"),
                Email = "laojiu@webapiclient.com"
            };

            var aboutResult = await userApiClient.GetAboutAsync(
                "http://localhost:9999/webapi/user/about",
                "Basic eW91ck5hbWU6MTIzNDU2",
                user,
                "some -value");

            var user1 = await userApiClient.GetByIdAsync("id001");

            var user2 = await userApiClient.GetByAccountAsync("laojiu");

            var user3 = await userApiClient.UpdateWithFormAsync(user, nickName: "老九", nullableAge: 18)
                .Retry(3, i => TimeSpan.FromSeconds(i))
                .WhenCatch<Exception>()
                .WhenResult(u => u == null || u.Account == null)
                .HandleAsDefaultWhenException();

            var user4 = await userApiClient.UpdateWithJsonAsync(user);

            var user5 = await userApiClient.UpdateWithXmlAsync(user);

            var user6 = await userApiClient.UpdateWithMulitpartAsync(user, "老九", 18, file);
        }
    }
}
