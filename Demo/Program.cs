using Demo.HttpClients;
using Demo.HttpServices;
using Microsoft.Extensions.Logging;
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

            HttpApiFactory.Add<IUserApi>(c =>
            {
                c.HttpHost = new Uri("http://localhost:9999/");
                c.LoggerFactory = new LoggerFactory().AddConsole();
            });

            var userApi = HttpApiFactory.Create<IUserApi>();           
            Program.RequestAsync(userApi).Wait();

            Console.ReadLine();
        }


        private static async Task RequestAsync(IUserApi userApi)
        {
            var user = new UserInfo
            {
                Account = "laojiu",
                Password = "123456",
                BirthDay = DateTime.Parse("2018-01-01 12:30:30"),
                Email = "laojiu@webapiclient.com",
                Gender = Gender.Male
            };

            var about = await userApi
                .GetAboutAsync("webapi/user/about", user, "somevalue");

            var user1 = await userApi
                .GetByIdAsync("id001");

            var user2 = await userApi
                .GetByAccountAsync("laojiu");

            var user3 = await userApi
                .UpdateWithFormAsync(user, nickName: "老九", nullableAge: null)
                .Retry(3, i => TimeSpan.FromSeconds(i))
                .WhenCatch<HttpStatusFailureException>();

            var user4 = await userApi
                .UpdateWithJsonAsync(user);

            var user5 = await userApi
                .UpdateWithXmlAsync(user).HandleAsDefaultWhenException();

            var file = new MulitpartFile("file.data");
            var user6 = await userApi
                .UpdateWithMulitpartAsync(user, "老九", 18, file);
        }
    }
}
