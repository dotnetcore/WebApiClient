using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.RequestBenchmark
{
    /// <summary> 
    /// 跳过真实的http请求环节的模拟Get请求
    /// </summary>
    public class Benchmark : IBenchmark
    {
        private readonly IServiceProvider serviceProvider;

        public Benchmark()
        {
            var model = new Model { A = "A", B = 2, C = 3d };
            var services = new ServiceCollection();

            services
                .AddHttpApi<IBenchmarkApi>(o => o.HttpHost = new Uri("http://webapiclient.com/"))
                .AddHttpMessageHandler(() => new NoneHttpDelegatingHandler());

            this.serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// 使用WebApiClientCore请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> WebApiClientCore_GetAsync()
        {
            using var scope = this.serviceProvider.CreateScope();
            var banchmarkApi = scope.ServiceProvider.GetRequiredService<IBenchmarkApi>();
            return await banchmarkApi.GetAsyc(id: "id");
        }

        /// <summary>
        /// 使用原生HttpClient请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> HttpClient_GetAsync()
        {
            using var scope = this.serviceProvider.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(IBenchmarkApi).FullName);

            var id = "id";
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://webapiclient.com/{id}");
            var response = await httpClient.SendAsync(request);
            var json = await response.Content.ReadAsByteArrayAsync();
            return JsonSerializer.Deserialize<Model>(json);
        }
    }
}
