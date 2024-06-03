```

BenchmarkDotNet v0.13.12, Windows 11 (10.0.22631.3593/23H2/2023Update/SunValley3)
Intel Core i7-8565U CPU 1.80GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
  [Host] : .NET 6.0.29, X64 NativeAOT AVX2

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                        | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------------------ |---------:|---------:|---------:|------:|--------:|-------:|----------:|------------:|
| WebApiClientCore_PutFormAsync | 15.09 μs | 0.290 μs | 0.705 μs |  1.00 |    0.00 | 1.4038 |   5.77 KB |        1.00 |
| Refit_PutFormAsync            | 27.61 μs | 0.544 μs | 1.333 μs |  1.83 |    0.12 | 1.8311 |   7.54 KB |        1.31 |
