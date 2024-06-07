```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX2

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                    | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| HttpClient_GetAsync       |  3.059 μs | 0.1315 μs | 0.3816 μs |  0.49 |    0.09 | 0.4959 |   2.03 KB |        0.51 |
| WebApiClientCore_GetAsync |  6.277 μs | 0.2695 μs | 0.7903 μs |  1.00 |    0.00 | 0.9766 |   4.02 KB |        1.00 |
| Refit_GetAsync            | 14.295 μs | 0.4401 μs | 1.2626 μs |  2.30 |    0.31 | 1.2817 |   5.34 KB |        1.33 |
