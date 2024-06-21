```

BenchmarkDotNet v0.13.12, CentOS Linux 7 (Core)
Intel Xeon CPU E5-2650 v2 2.60GHz, 2 CPU, 32 logical and 16 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                        | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |---------:|---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| WebApiClientCore_PutFormAsync | 19.94 μs | 0.394 μs | 0.679 μs | 19.62 μs |  1.00 |    0.00 | 0.5493 |    5.7 KB |        1.00 |
| Refit_PutFormAsync            | 79.90 μs | 1.551 μs | 2.321 μs | 78.62 μs |  3.98 |    0.17 | 1.0986 |  11.57 KB |        2.03 |
