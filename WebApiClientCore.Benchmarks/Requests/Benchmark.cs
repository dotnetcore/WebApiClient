using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net.Http;

namespace WebApiClientCore.Benchmarks.Requests
{
    [InProcess]
    [MemoryDiagnoser]
    public abstract class Benchmark : IBenchmark
    {
        protected IServiceProvider ServiceProvider { get; set; }


        [GlobalSetup]
        public void Setup()
        {
            var services = new ServiceCollection();

            services
                .AddHttpClient(typeof(HttpClient).FullName)
                .AddHttpMessageHandler(() => new MockResponseHandler());

            services
                .AddHttpApi<IWebApiClientCoreApi>(o =>
                {
                    o.UseParameterPropertyValidate = false;
                    o.UseReturnValuePropertyValidate = false;
                })
                .AddHttpMessageHandler(() => new MockResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            services
                .AddRefitClient<IRefitApi>(new RefitSettings
                {
                })
                .AddHttpMessageHandler(() => new MockResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            this.ServiceProvider = services.BuildServiceProvider();
            this.ServiceProvider.GetService<IWebApiClientCoreApi>();
            this.ServiceProvider.GetService<IRefitApi>();
        }
    }
}
