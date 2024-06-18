using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    public class HttpPutFormBenchmark : Benchmark
    {
        [Benchmark(Baseline = true)]
        public async Task<User> WebApiClientCore_PutFormAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreApi>();
            return await benchmarkApi.PutFormAsync("id001", User.Instance);
        }

        [Benchmark]
        public async Task<User> Refit_PutFormAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IRefitApi>();
            return await benchmarkApi.PutFormAsync("id001", User.Instance);
        }
    }
}
