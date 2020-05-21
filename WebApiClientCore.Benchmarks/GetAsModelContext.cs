using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks
{
    /// <summary>
    /// 使用ActionInvoker直接Action
    /// 跳过真实的http请求环节，模拟返回HttpResponseMessage
    /// </summary>
    public class GetAsModelContext
    {
        private readonly IServiceProvider serviceProvider;

        public GetAsModelContext()
        {
            var model = new BenchmarkModel { A = "A", B = 2, C = 3d };
            var services = new ServiceCollection();

            services
                .AddHttpApi<IBenchmarkApi>(o => o.HttpHost = new Uri("http://webapiclient.com/"))
                .AddHttpMessageHandler(() => new NoneHttpHandler(model));

            this.serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// 使用WebApiClientCore请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<BenchmarkModel> WebApiClientCore_GetAsModelAsync()
        {
            using var scope = this.serviceProvider.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<IBenchmarkApi>();
            return await client.GetAsModelAsync(id: "id");
        }

        /// <summary>
        /// 使用HttpClientFactory的HttpClient请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<BenchmarkModel> HttpClientFactory_GetAsModelAsync()
        {
            using var scope = this.serviceProvider.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(IBenchmarkApi).FullName);

            var id = "id";
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://webapiclient.com/{id}");
            var response = await client.SendAsync(request);
            var json = await response.Content.ReadAsByteArrayAsync();
            return JsonSerializer.Deserialize<BenchmarkModel>(json);
        }

        /// <summary>
        /// 无真实http请求的Handler
        /// </summary>
        private class NoneHttpHandler : DelegatingHandler
        {
            private readonly HttpResponseMessage benchmarkModelResponseMessage;

            public NoneHttpHandler(BenchmarkModel model)
            {
                var json = JsonSerializer.SerializeToUtf8Bytes(model);
                this.benchmarkModelResponseMessage = new HttpResponseMessage(HttpStatusCode.OK) { Content = new JsonContent(json) };
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(this.benchmarkModelResponseMessage);
            }
        }
    }
}
