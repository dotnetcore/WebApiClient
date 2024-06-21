```

BenchmarkDotNet v0.13.12, CentOS Linux 7 (Core)
Intel Xeon CPU E5-2650 v2 2.60GHz, 2 CPU, 32 logical and 16 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                         | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------- |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| WebApiClientCore_PostJsonAsync | 11.24 μs | 0.223 μs | 0.441 μs |  1.00 |    0.00 | 0.4120 |   4.23 KB |        1.00 |
| Refit_PostJsonAsync            | 24.80 μs | 0.487 μs | 0.598 μs |  2.18 |    0.09 | 0.5798 |   6.08 KB |        1.44 |
