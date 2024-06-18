using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    [InProcess]
    [MemoryDiagnoser]
    public abstract class Benchmark
    {
        protected IServiceProvider ServiceProvider { get; set; }

        [GlobalSetup]
        public void GlobalSetup()
        {
            var services = new ServiceCollection();
            services
                .AddHttpApi<IWebApiClientCoreApi>(o =>
                {
                    o.UseParameterPropertyValidate = false;
                    o.UseReturnValuePropertyValidate = false;
                })
                .AddHttpMessageHandler(() => new UserResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            services
                .AddRefitClient<IRefitApi>(new RefitSettings
                {
                })
                .AddHttpMessageHandler(() => new UserResponseHandler())
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://webapiclient.com/"));

            this.ServiceProvider = services.BuildServiceProvider();
            this.ServiceProvider.GetService<IWebApiClientCoreApi>();
            this.ServiceProvider.GetService<IRefitApi>();
        }

        private class UserResponseHandler : DelegatingHandler
        {
            private static readonly MediaTypeHeaderValue applicationJson = MediaTypeHeaderValue.Parse("application/json");
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var content = new StreamContent(new MemoryStream(User.Utf8Array, writable: false), User.Utf8Array.Length);
                content.Headers.ContentType = applicationJson;
                var response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = content
                };
                return Task.FromResult(response);
            }
        }
    }
}
