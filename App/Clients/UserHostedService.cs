using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebApiClientCore.Parameterables;

namespace App.Clients
{
    public class UserHostedService : BackgroundService
    {
        private readonly IServiceProvider service;
        private readonly ILogger<UserHostedService> logger;

        public UserHostedService(IServiceProvider service, ILogger<UserHostedService> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = this.service.CreateScope();
                await this.RunAsync(scope.ServiceProvider);
                await this.RunAsync(scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }
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

            using var fileStream = new FileStream("TextFile.txt", FileMode.Open);
            var file = new FormDataFile(fileStream, "TextFile.txt");


            var response = await userApi.GetAsync(account: "get1");
            var @string = await userApi.GetAsStringAsync(account: "get2");
            var byteArray = await userApi.GetAsByteArrayAsync(account: "get3");
            var stream = await userApi.GetAsStreamAsync(account: "get4");
            var model = await userApi.GetAsModelAsync(account: "get5");

            var post1 = await userApi.PostByJsonAsync(user);
            var post2 = await userApi.PostByXmlAsync(user);
            var post3 = await userApi.PostByFormAsync(user);
            var post4 = await userApi.PostByFormDataAsync(user, file);


            await userApi.DeleteAsync(account: "account");
        }
    }
}
