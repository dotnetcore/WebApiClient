using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace WebApiClientCore.Benchmarks.Requests
{
    public class HttpPostXmlBenchmark : Benchmark
    {
        [Benchmark(Baseline = true)]
        public async Task<User> WebApiClientCore_PostXmlAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IWebApiClientCoreXmlApi>();
            return await benchmarkApi.PostXmlAsync(User.Instance);
        }

        [Benchmark]
        public async Task<User> Refit_PostXmlAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var benchmarkApi = scope.ServiceProvider.GetRequiredService<IRefitXmlApi>();
            return await benchmarkApi.PostXmlAsync(User.Instance);
        }

        [Benchmark]
        public async Task<User> RestSharp_PostXmlAsync()
        {
            using var scope = this.ServiceProvider.CreateScope();
            var client = scope.ServiceProvider.GetRequiredService<RestSharpXmlClient>();
            var request = new RestRequest($"/benchmarks")
                .AddXmlBody(User.Instance);
            return await client.PostAsync<User>(request);
        }
    }
}
