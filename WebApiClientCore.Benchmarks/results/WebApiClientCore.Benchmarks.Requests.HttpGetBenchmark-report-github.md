```

BenchmarkDotNet v0.13.12, CentOS Linux 7 (Core)
Intel Xeon CPU E5-2650 v2 2.60GHz, 2 CPU, 32 logical and 16 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                    | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| WebApiClientCore_GetAsync |  5.558 μs | 0.1094 μs | 0.1384 μs |  1.00 |    0.00 | 0.3357 |   3.45 KB |        1.00 |
| Refit_GetAsync            | 14.494 μs | 0.2764 μs | 0.3394 μs |  2.61 |    0.10 | 0.4883 |   5.18 KB |        1.50 |
