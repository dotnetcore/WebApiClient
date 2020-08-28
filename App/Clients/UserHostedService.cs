using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace App.Clients
{
    public class UserHostedService : BackgroundService
    {
        private readonly IServiceProvider service;
        private readonly ILogger logger;

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
                var userService = scope.ServiceProvider.GetService<UserService>();
                await userService.RunRequestAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }
        }
    }
}
