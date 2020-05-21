using BenchmarkDotNet.Running;
using System;

namespace WebApiClientCore.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<GetAsModelContext>();
            Console.ReadLine();
        }
    }
}
