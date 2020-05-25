using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiClientCore.Benchmarks.RequestBenchmark
{
    public abstract class BenChmark : IBenchmark
    {
        protected IServiceProvider ServiceProvider { get; }

        public BenChmark()
        {
            var services = new ServiceCollection();

            services
                .AddHttpClient(typeof(HttpClient).FullName)
                .AddHttpMessageHandler(() => new MockResponseHandler());

            services
                .AddHttpApi<IWebApiClientCoreApi>(o => o.HttpHost = new Uri("http://webapiclient.com/"))
                .AddHttpMessageHandler(() => new MockResponseHandler());

            WebApiClient.Extension
                .AddHttpApi<IWebApiClientApi>(services, o => o.HttpHost = new Uri("http://webapiclient.com/"))
                .AddHttpMessageHandler(() => new MockResponseHandler());

            this.ServiceProvider = services.BuildServiceProvider();

            this.PreheatAsync().Wait();
        }

        private async Task PreheatAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var api = scope.ServiceProvider.GetService<IWebApiClientApi>();
            var coreApi = scope.ServiceProvider.GetService<IWebApiClientCoreApi>();
            await api.GetAsyc("id");
            await coreApi.GetAsyc("id");
            await api.PostAsync(new Model { });
            await coreApi.PostAsync(new Model { });
        }
    }
}
