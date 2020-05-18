using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace App.Clients
{
    public class UserHostedService : BackgroundService
    {
        private readonly IServiceProvider service;

        public UserHostedService(IServiceProvider service)
        {
            this.service = service;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = this.service.CreateScope();
            var userService = scope.ServiceProvider.GetService<UserService>();
            await userService.RunRequestAsync();
        }
    }
}
