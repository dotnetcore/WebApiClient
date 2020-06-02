using BenchmarkDotNet.Attributes;

namespace WebApiClientCore.Benchmarks.Buffers
{
    public class Benchmark : IBenchmark
    {
        [Benchmark]
        public void Rent()
        {
            using (new BufferWriter<byte>()) { }
        }

        [Benchmark]
        public void New()
        {
            _ = new byte[1024];
        }
    }
}
