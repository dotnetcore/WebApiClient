using BenchmarkDotNet.Attributes;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApiClientCore.Benchmarks.Others
{
    [InProcess]
    [MemoryDiagnoser]
    public class ReadAsJsonBenchmark
    {
        [Benchmark(Baseline = true)]
        public async Task<User[]> ReadAsJsonAsync()
        {
            var content = new StreamContent(new MemoryStream(User.Utf8Json, writable: false));
            return await content.ReadAsJsonAsync<User[]>(null, default);
        }

        [Benchmark]
        public async Task<User[]> ReadFromJsonAsync()
        {
            var content = new StreamContent(new MemoryStream(User.Utf8Json, writable: false));
            return await content.ReadFromJsonAsync<User[]>(default(JsonSerializerOptions));
        }

        [Benchmark]
        public async Task<User[]> ReadAsByteArrayAsync()
        {
            var content = new StreamContent(new MemoryStream(User.Utf8Json, writable: false));
            var utf8Json = await content.ReadAsUtf8ByteArrayAsync();
            return JsonSerializer.Deserialize<User[]>(utf8Json, default(JsonSerializerOptions));
        }
    }
}
