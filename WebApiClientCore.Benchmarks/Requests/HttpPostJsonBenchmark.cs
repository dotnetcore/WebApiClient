using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    [MemoryDiagnoser]
    public class HttpPostJsonBenchmark : Benchmark
    {       
        [Benchmark]
        public async Task<User> HttpClient_PostJsonAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(HttpClient).FullName);
             
            var response = await httpClient.PostAsJsonAsync($"http://webapiclient.com/", User.Instance);
            return await response.Content.ReadFromJsonAsync<User>();
        }

        [Benchmark(Baseline = true)]
        public async Task<User> WebApiClientCore_PostJsonAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreApi>();            
            return await benchmarkApi.PostJsonAsync(User.Instance);
        }


        [Benchmark]
        public async Task<User> Refit_PostJsonAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IRefitApi>();             
            return await benchmarkApi.PostJsonAsync(User.Instance);
        }
    }
}
