using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace App.Clients
{
    public class DynamicHostDemoHostedService : BackgroundService
    {
        private readonly IServiceProvider service;
        private readonly ILogger<DynamicHostDemoHostedService> logger;

        public DynamicHostDemoHostedService(IServiceProvider service, ILogger<DynamicHostDemoHostedService> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                using var scope = this.service.CreateScope();
                var dynamicHostDemoService = scope.ServiceProvider.GetService<DynamicHostDemoService>();
                await dynamicHostDemoService.RunRequestAsync();
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, ex.Message);
            }
        }
    }
}
