using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Concurrent;

namespace WebApiClientCore.Benchmarks.Caches
{
    public class Benchmark : IBenchmark
    {
        private readonly ConcurrentCache<Type, string> cache = new ConcurrentCache<Type, string>();
        private readonly ConcurrentDictionary<Type, string> dic = new ConcurrentDictionary<Type, string>();

        [Benchmark]
        public void ConcurrentCache_GetOrAdd()
        {
            cache.GetOrAdd(typeof(Benchmark), key => string.Empty);
        }

        [Benchmark]
        public void ConcurrentDictionary_GetOrAdd()
        {
            dic.GetOrAdd(typeof(Benchmark), key => string.Empty);
        }
    }
}
