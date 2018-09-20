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

            using (var userApi = HttpApiClient.Create<IUserApi>())
            {
                RequestAsync(userApi).Wait();
            }
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
                .GetAboutAsync("http://localhost:9999/webapi/user/about", user, "somevalue");

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

            var file = new MulitpartFile("about.txt");
            var user6 = await userApi
                .UpdateWithMulitpartAsync(user, "老九", 18, file);
        }
    }
}
