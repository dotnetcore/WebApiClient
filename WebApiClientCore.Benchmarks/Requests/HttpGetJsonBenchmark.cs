using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    public class HttpGetJsonBenchmark : Benchmark
    {
        [Benchmark(Baseline = true)]
        public async Task<User> WebApiClientCore_GetJsonAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreJsonApi>();
            return await benchmarkApi.GetJsonAsync(id: "id001");
        }

        [Benchmark]
        public async Task<User> Refit_GetJsonAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IRefitJsonApi>();
            return await benchmarkApi.GetJsonAsync(id: "id001");
        }
    }
}
