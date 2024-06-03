```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
  [Host] : .NET 6.0.29, X64 NativeAOT AVX2

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                         | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| HttpClient_PostJsonAsync       |  4.372 μs | 0.0680 μs | 0.0568 μs |  1.00 |    0.00 | 0.6409 |   2.64 KB |        1.00 |
| WebApiClientCore_PostJsonAsync | 10.299 μs | 0.1485 μs | 0.1389 μs |  2.35 |    0.04 | 1.1902 |    4.9 KB |        1.86 |
| Refit_PostJsonAsync            | 21.006 μs | 0.4003 μs | 0.7320 μs |  4.84 |    0.19 | 1.5869 |   6.53 KB |        2.47 |
