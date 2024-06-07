```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX2

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                        | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| WebApiClientCore_PutFormAsync | 10.45 μs | 0.415 μs | 1.204 μs |  1.00 |    0.00 | 1.2207 |      5 KB |        1.00 |
| Refit_PutFormAsync            | 22.70 μs | 1.261 μs | 3.717 μs |  2.19 |    0.33 | 1.6785 |   6.89 KB |        1.38 |
