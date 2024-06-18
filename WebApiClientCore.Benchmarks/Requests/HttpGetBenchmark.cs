using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    public class HttpGetBenchmark : Benchmark
    {
        [Benchmark]
        public async Task<User> HttpClient_GetAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var httpClient = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>().CreateClient(typeof(HttpClient).FullName);

            var id = "id";
            var requestUri = $"http://webapiclient.com/{id}";
            return await httpClient.GetFromJsonAsync<User>(requestUri);
        }

        [Benchmark(Baseline = true)]
        public async Task<User> WebApiClientCore_GetAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreApi>();
            return await benchmarkApi.GetJsonAsync(id: "id");
        }

        [Benchmark]
        public async Task<User> Refit_GetAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IRefitApi>();
            return await benchmarkApi.GetAsync(id: "id");
        }
    }
}
