```

BenchmarkDotNet v0.13.12, CentOS Linux 7 (Core)
Intel Xeon CPU E5-2650 v2 2.60GHz, 2 CPU, 32 logical and 16 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                        | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|------------------------------ |---------:|---------:|---------:|---------:|------:|--------:|--------:|-------:|----------:|------------:|
| WebApiClientCore_PostXmlAsync | 47.97 μs | 0.943 μs | 2.009 μs | 47.11 μs |  1.00 |    0.00 |  3.4180 | 0.1221 |  35.48 KB |        1.00 |
| Refit_PostXmlAsync            | 57.06 μs | 0.948 μs | 0.740 μs | 56.87 μs |  1.21 |    0.02 | 14.0381 | 2.3193 | 144.38 KB |        4.07 |
