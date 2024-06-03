```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4412/22H2/2022Update)
Intel Core i3-4150 CPU 3.50GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX2

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                        | Mean      | Error     | StdDev    | Median    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |----------:|----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| WebApiClientCore_PutFormAsync |  8.689 μs | 0.1715 μs | 0.3136 μs |  8.733 μs |  1.00 |    0.00 | 3.2501 |      5 KB |        1.00 |
| Refit_PutFormAsync            | 20.598 μs | 0.4215 μs | 1.2429 μs | 21.112 μs |  2.37 |    0.17 | 4.5776 |   7.05 KB |        1.41 |
