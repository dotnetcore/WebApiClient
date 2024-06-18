using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    public class HttpGetBenchmark : Benchmark
    {
        [Benchmark(Baseline = true)]
        public async Task WebApiClientCore_GetAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreApi>();
            await benchmarkApi.GetAsync(id: "id001");
        }

        [Benchmark]
        public async Task Refit_GetAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IRefitApi>();
            await benchmarkApi.GetAsync(id: "id001");
        }
    }
}
