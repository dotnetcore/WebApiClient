using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace WebApiClientCore.Benchmarks.StringReplaces
{
    public class Benchmark : IBenchmark
    {
        private readonly string str = "WebApiClientCore.Benchmarks.StringReplaces.WebApiClientCore";
        private readonly string pattern = "core";
        private readonly string replacement = "CORE";

        [Benchmark]
        public void ReplaceByRegexNew()
        {
            new Regex(pattern, RegexOptions.IgnoreCase).Replace(str, replacement);           
        }

        [Benchmark]
        public void ReplaceByRegexStatic()
        {
            Regex.Replace(str, pattern, replacement, RegexOptions.IgnoreCase);
        }

        [Benchmark]
        public void ReplaceByCutomSpan()
        {
            str.RepaceIgnoreCase(pattern, replacement, out var _);
        }
    }
}
