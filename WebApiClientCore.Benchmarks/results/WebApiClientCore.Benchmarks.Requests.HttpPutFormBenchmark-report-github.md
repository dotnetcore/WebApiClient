```

BenchmarkDotNet v0.13.12, CentOS Linux 7 (Core)
Intel Xeon CPU E5-2650 v2 2.60GHz, 2 CPU, 32 logical and 16 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                        | Mean     | Error    | StdDev   | Median   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |---------:|---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| WebApiClientCore_PutFormAsync | 20.10 μs | 0.395 μs | 0.470 μs | 19.96 μs |  1.00 |    0.00 | 0.5493 |    5.7 KB |        1.00 |
| Refit_PutFormAsync            | 75.53 μs | 1.476 μs | 2.163 μs | 74.24 μs |  3.78 |    0.12 | 1.0986 |  11.57 KB |        2.03 |
