```

BenchmarkDotNet v0.13.12, CentOS Linux 7 (Core)
Intel Xeon CPU E5-2650 v2 2.60GHz, 2 CPU, 32 logical and 16 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                    | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| WebApiClientCore_GetAsync |  5.926 μs | 0.1162 μs | 0.1630 μs |  1.00 |    0.00 | 0.3052 |   3.45 KB |        1.00 |
| Refit_GetAsync            | 15.943 μs | 0.2997 μs | 0.3681 μs |  2.70 |    0.10 | 0.4883 |   5.18 KB |        1.50 |
