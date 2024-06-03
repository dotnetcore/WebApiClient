```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
  [Host] : .NET 6.0.29, X64 NativeAOT AVX2

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                    | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| HttpClient_GetAsync       |  3.201 μs | 0.0624 μs | 0.0811 μs |  1.00 |    0.00 | 0.5379 |    2.2 KB |        1.00 |
| WebApiClientCore_GetAsync |  8.726 μs | 0.1179 μs | 0.1103 μs |  2.71 |    0.10 | 1.1139 |   4.59 KB |        2.08 |
| Refit_GetAsync            | 17.237 μs | 0.2066 μs | 0.1933 μs |  5.34 |    0.17 | 1.3733 |   5.67 KB |        2.57 |
