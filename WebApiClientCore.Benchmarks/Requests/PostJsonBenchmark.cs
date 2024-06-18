using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    /// <summary> 
    /// 跳过真实的http请求环节的模拟Post json请求
    /// </summary>
    [MemoryDiagnoser]
    public class PostJsonBenchmark : Benchmark
    {
        /// <summary>
        /// 使用原生HttpClient请求
        /// </summary>
        /// <returns></returns>
        [Benchmark]
        public async Task<Model> HttpClient_PostJsonAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(HttpClient).FullName);

            var input = new Model { A = "a" };
            var response = await httpClient.PostAsJsonAsync($"http://webapiclient.com/", input);
            return await response.Content.ReadFromJsonAsync<Model>();
        }

        /// <summary>
        /// 使用WebApiClientCore请求
        /// </summary>
        /// <returns></returns>
        [Benchmark(Baseline = true)]
        public async Task<Model> WebApiClientCore_PostJsonAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreApi>();
            var input = new Model { A = "a" };
            return await benchmarkApi.PostJsonAsync(input);
        }


        [Benchmark]
        public async Task<Model> Refit_PostJsonAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IRefitApi>();
            var input = new Model { A = "a" };
            return await benchmarkApi.PostJsonAsync(input);
        }
    }
}
