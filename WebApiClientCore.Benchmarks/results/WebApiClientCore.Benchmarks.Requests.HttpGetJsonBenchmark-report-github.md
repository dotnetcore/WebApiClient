```

BenchmarkDotNet v0.13.12, CentOS Linux 7 (Core)
Intel Xeon CPU E5-2650 v2 2.60GHz, 2 CPU, 32 logical and 16 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                        | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| WebApiClientCore_GetJsonAsync | 11.31 μs | 0.225 μs | 0.322 μs |  1.00 |    0.00 | 0.4120 |    4.3 KB |        1.00 |
| Refit_GetJsonAsync            | 29.03 μs | 0.575 μs | 0.788 μs |  2.57 |    0.09 | 0.5493 |   5.67 KB |        1.32 |
