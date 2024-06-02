using BenchmarkDotNet.Attributes;
using WebApiClientCore.Internals;

namespace WebApiClientCore.Benchmarks.Buffers
{
    [InProcess]
    public class Benchmark : IBenchmark
    {
        [Benchmark]
        public void Rent()
        {
            using (new RecyclableBufferWriter<byte>()) { }
        }

        [Benchmark]
        public void New()
        {
            _ = new byte[1024];
        }
    }
}
