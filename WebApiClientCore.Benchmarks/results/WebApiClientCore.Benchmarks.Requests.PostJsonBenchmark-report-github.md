```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4412/22H2/2022Update)
Intel Core i3-4150 CPU 3.50GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX2

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                         | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| HttpClient_PostJsonAsync       |  2.760 μs | 0.0294 μs | 0.0246 μs |  0.48 |    0.01 | 1.5068 |   2.31 KB |        0.55 |
| WebApiClientCore_PostJsonAsync |  5.712 μs | 0.0614 μs | 0.0512 μs |  1.00 |    0.00 | 2.7237 |   4.17 KB |        1.00 |
| Refit_PostJsonAsync            | 13.246 μs | 0.0457 μs | 0.0382 μs |  2.32 |    0.02 | 3.9215 |   6.02 KB |        1.44 |
