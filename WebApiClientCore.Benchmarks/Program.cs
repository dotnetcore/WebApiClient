using BenchmarkDotNet.Running;
using WebApiClientCore.Benchmarks.Requests;

namespace WebApiClientCore.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            // 使用 BenchmarkSwitcher 支持命令行参数
            // 这样可以通过 --filter, --exporters 等参数控制执行
            var switcher = BenchmarkSwitcher.FromTypes(new[]
            {
                typeof(HttpGetBenchmark),
                typeof(HttpGetJsonBenchmark),
                typeof(HttpPostJsonBenchmark),
                typeof(HttpPostXmlBenchmark),
                typeof(HttpPutFormBenchmark)
            });

            // 如果没有传入参数，使用默认参数执行所有测试
            if (args == null || args.Length == 0)
            {
                args = new[] { "--filter", "*" };
            }

            switcher.Run(args);
        }
    }
}
