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
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<UserHostedService> logger;

        public UserHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<UserHostedService> logger)
        { 
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = this.serviceScopeFactory.CreateScope();
                var userService = scope.ServiceProvider.GetRequiredService<UserService>();
                await userService.RunRequestsAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }
        }
    }
}
