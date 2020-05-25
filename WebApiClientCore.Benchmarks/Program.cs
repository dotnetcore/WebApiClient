using BenchmarkDotNet.Running;
using System;
using System.Linq;

namespace WebApiClientCore.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            var benchmarkTypes = typeof(Program).Assembly.GetTypes()
                .Where(item => typeof(IBenchmark).IsAssignableFrom(item))
                .Where(item => item.IsAbstract == false && item.IsClass);

            foreach (var item in benchmarkTypes)
            {
                BenchmarkRunner.Run(item);
            }
            Console.ReadLine();
        }
    }
}
