using BenchmarkDotNet.Running;
using WebApiClientCore.Benchmarks.Requests;

namespace WebApiClientCore.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<HttpGetBenchmark>();
            BenchmarkRunner.Run<HttpGetJsonBenchmark>();
            BenchmarkRunner.Run<HttpPostXmlBenchmark>();
            BenchmarkRunner.Run<HttpPostJsonBenchmark>();
            BenchmarkRunner.Run<HttpPutFormBenchmark>();
        }
    }
}
