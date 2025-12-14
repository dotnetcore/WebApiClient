using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Requests
{
    public class HttpPutFormBenchmark : Benchmark
    {
        [Benchmark(Baseline = true)]
        public async Task<User> WebApiClientCore_PutFormAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreJsonApi>();
            return await benchmarkApi.PutFormAsync(id: "id001", User.Instance);
        }

        [Benchmark]
        public async Task<User> Refit_PutFormAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IRefitJsonApi>();
            return await benchmarkApi.PutFormAsync(id: "id001", User.Instance);
        }

        [Benchmark]
        public async Task<User> RestSharp_PutFormAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<RestSharpJsonClient>();
            var request = new RestRequest($"/benchmarks/id001")
                .AddParameter("id", User.Instance.Id)
                .AddParameter("name", User.Instance.Name)
                .AddParameter("bio", User.Instance.Bio)
                .AddParameter("followers", User.Instance.Followers)
                .AddParameter("following", User.Instance.Following)
                .AddParameter("url", User.Instance.Url)
                .AddHeader("Content-Type", "application/x-www-form-urlencoded");
            return await client.PutAsync<User>(request);
        }
    }
}
