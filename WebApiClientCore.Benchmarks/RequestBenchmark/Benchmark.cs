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

            this.serviceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// 使用WebApiClient.JIT请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> WebApiClient_GetAsync()
        {
            using var scope = this.serviceProvider.CreateScope();
            var banchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientApi>();
            return await banchmarkApi.GetAsyc(id: "id");
        }

        /// <summary>
        /// 使用WebApiClientCore请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> WebApiClientCore_GetAsync()
        {
            using var scope = this.serviceProvider.CreateScope();
            var banchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreApi>();
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
            var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(HttpClient).FullName);

            var id = "id";
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://webapiclient.com/{id}");
            var response = await httpClient.SendAsync(request);
            var json = await response.Content.ReadAsByteArrayAsync();
            return JsonSerializer.Deserialize<Model>(json);
        }


        /// <summary>
        /// 使用WebApiClient.JIT请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> WebApiClient_PostAsync()
        {
            using var scope = this.serviceProvider.CreateScope();
            var banchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientApi>();
            var input = new Model { A = "a" };
            return await banchmarkApi.PostAsync(input);
        }

        /// <summary>
        /// 使用WebApiClientCore请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> WebApiClientCore_PostAsync()
        {
            using var scope = this.serviceProvider.CreateScope();
            var banchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreApi>();
            var input = new Model { A = "a" };
            return await banchmarkApi.PostAsync(input);
        }

        /// <summary>
        /// 使用原生HttpClient请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> HttpClient_PostAsync()
        {
            using var scope = this.serviceProvider.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(HttpClient).FullName);

            var input = new Model { A = "a" };
            var json = JsonSerializer.SerializeToUtf8Bytes(input);
            var request = new HttpRequestMessage(HttpMethod.Post, $"http://webapiclient.com/")
            {
                Content = new JsonContent(json)
            };

            var response = await httpClient.SendAsync(request);
            json = await response.Content.ReadAsByteArrayAsync();
            return JsonSerializer.Deserialize<Model>(json);
        }
    }
}
