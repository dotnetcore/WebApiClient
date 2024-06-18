using BenchmarkDotNet.Running;
using System;
using WebApiClientCore.Benchmarks.Requests;

namespace WebApiClientCore.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {          
            BenchmarkRunner.Run<HttpGetBenchmark>();
            BenchmarkRunner.Run<HttpPostJsonBenchmark>();
            BenchmarkRunner.Run<HttpPutFormBenchmark>();
            Console.ReadLine();
        }
    }
}
