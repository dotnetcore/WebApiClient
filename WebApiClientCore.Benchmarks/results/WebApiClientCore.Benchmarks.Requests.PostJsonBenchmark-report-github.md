```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX2

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                         | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| HttpClient_PostJsonAsync       |  3.804 μs | 0.1678 μs | 0.4813 μs |  0.60 |    0.11 | 0.5646 |   2.31 KB |        0.55 |
| WebApiClientCore_PostJsonAsync |  6.528 μs | 0.2695 μs | 0.7945 μs |  1.00 |    0.00 | 1.0147 |   4.17 KB |        1.00 |
| Refit_PostJsonAsync            | 13.823 μs | 0.5368 μs | 1.5658 μs |  2.16 |    0.38 | 1.4038 |   5.83 KB |        1.40 |
