using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    /// <summary> 
    /// 跳过真实的http请求环节的模拟Get请求
    /// </summary>
    [MemoryDiagnoser]
    public class GetBenchmark : Benchmark
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
            var requestUri = $"http://webapiclient.com/{id}";
            return await httpClient.GetFromJsonAsync<Model>(requestUri);
        }

        /// <summary>
        /// 使用WebApiClientCore请求
        /// </summary>
        /// <returns></returns>
        [Benchmark(Baseline = true)]
        public async Task<Model> WebApiClientCore_GetAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreApi>();
            return await benchmarkApi.GetAsync(id: "id");
        }


        /// <summary>
        /// Refit的Get请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> Refit_GetAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IRefitApi>();
            return await benchmarkApi.GetAsync(id: "id");
        }
    }
}
