using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore.Parameters;

namespace App.Clients
{
    public class UserClientHostedService : BackgroundService
    {
        private readonly IServiceProvider service;

        public UserClientHostedService(IServiceProvider service)
        {
            this.service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = this.service.CreateScope();
            await this.RunAsync(scope.ServiceProvider);
        }

        private async Task RunAsync(IServiceProvider services)
        {
            var userApi = services.GetService<IUserApi>();

            var user = new User
            {
                Account = "laojiu",
                Password = "123456",
                BirthDay = DateTime.Parse("2018-01-01 12:30:30"),
                Email = "laojiu@webapiclient.com",
                Gender = Gender.Male
            };

            // 上传的文件
            var file = new FormDataFile("TextFile.txt");

            var response = await userApi.GetAsync(account: "get1");
            var @string = await userApi.GetAsStringAsync(account: "get2");
            var byteArray = await userApi.GetAsByteArrayAsync(account: "get3");
            var stream = await userApi.GetAsStreamAsync(account: "get4");
            var model = await userApi.GetAsModelAsync(account: "get5");

            var post1 = await userApi.PostByJsonAsync(user);
            var post2 = await userApi.PostByXmlAsync(user);
            var post3 = await userApi.PostByFormAsync(user);
            var post4 = await userApi.PostByFormDataAsync(user, file);

            var retry = await userApi.GetAsync(account: "retry")
                .Retry(maxCount: 3)
                .WhenCatch<Exception>();

            await userApi.DeleteAsync(account: "account");
        }
    }
}
