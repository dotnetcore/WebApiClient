```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4412/22H2/2022Update)
Intel Core i3-4150 CPU 3.50GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX2

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                    | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| HttpClient_GetAsync       |  2.203 μs | 0.0430 μs | 0.0574 μs |  0.42 |    0.01 | 1.3237 |   2.03 KB |        0.51 |
| WebApiClientCore_GetAsync |  5.245 μs | 0.1027 μs | 0.1142 μs |  1.00 |    0.00 | 2.6169 |   4.02 KB |        1.00 |
| Refit_GetAsync            | 12.336 μs | 0.2447 μs | 0.6615 μs |  2.37 |    0.13 | 3.4790 |   5.34 KB |        1.33 |
