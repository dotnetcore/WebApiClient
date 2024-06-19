using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    public class HttpPostJsonBenchmark : Benchmark
    {   
        [Benchmark(Baseline = true)]
        public async Task<User> WebApiClientCore_PostJsonAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreJsonApi>();            
            return await benchmarkApi.PostJsonAsync(User.Instance);
        }


        [Benchmark]
        public async Task<User> Refit_PostJsonAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IRefitJsonApi>();             
            return await benchmarkApi.PostJsonAsync(User.Instance);
        }
    }
}
