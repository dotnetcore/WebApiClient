using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.RequestBenchmark
{
    /// <summary> 
    /// 跳过真实的http请求环节的模拟Get请求
    /// </summary>
    public class PostBenchmark : BenChmark
    {
        /// <summary>
        /// 使用WebApiClient.JIT请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> WebApiClient_PostAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
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
            using var scope = this.ServiceProvider.CreateScope();
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
            using var scope = this.ServiceProvider.CreateScope();
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
