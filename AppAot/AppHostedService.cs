using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppAot
{
    class AppHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory serviceScopeFactory;
        private readonly ILogger<AppHostedService> logger;

        public AppHostedService(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<AppHostedService> logger)
        {
            this.serviceScopeFactory = serviceScopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = this.serviceScopeFactory.CreateScope();
            var api = scope.ServiceProvider.GetRequiredService<ICloudflareApi>();
            var appData = await api.GetAppDataAsync();
            appData = await api.GetAppData2Async();
            this.logger.LogInformation($"WebpackCompilationHash: {appData.WebpackCompilationHash}");
        }
    }
}
