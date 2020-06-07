using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    /// <summary> 
    /// 跳过真实的http请求环节的模拟Get请求
    /// </summary>
    public class GetBenchmark : BenChmark
    { 
        /// <summary>
        /// 使用原生HttpClient请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> HttpClient_GetAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(HttpClient).FullName);

            var id = "id";
            var request = new HttpRequestMessage(HttpMethod.Get, $"http://webapiclient.com/{id}");
            var response = await httpClient.SendAsync(request);
            var json = await response.Content.ReadAsUtf8ByteArrayAsync();
            return JsonSerializer.Deserialize<Model>(json);
        }


        /// <summary>
        /// 使用WebApiClientCore请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> WebApiClientCore_GetAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var banchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreApi>();
            return await banchmarkApi.GetAsyc(id: "id");
        }


        /// <summary>
        /// Refit的Get请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> Refit_GetAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var banchmarkApi = scope.ServiceProvider.GetRequiredService<IRefitApi>();
            return await banchmarkApi.GetAsyc(id: "id");
        }
    }
}
