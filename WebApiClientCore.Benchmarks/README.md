## WebApiClientCore.Benchmarks　
```

BenchmarkDotNet v0.13.12, Windows 10 (10.0.19045.4412/22H2/2022Update)
Intel Core i3-4150 CPU 3.50GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET SDK 9.0.100-preview.4.24267.66
  [Host] : .NET 6.0.29 (6.0.2924.17105), X64 RyuJIT AVX2

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                    | Mean      | Error     | StdDev    | Ratio | RatioSD | Gen0   | Allocated | Alloc Ratio |
|-------------------------- |----------:|----------:|----------:|------:|--------:|-------:|----------:|------------:|
| HttpClient_GetAsync       |  3.169 μs | 0.0427 μs | 0.0357 μs |  1.00 |    0.00 | 1.4381 |    2.2 KB |        1.00 |
| WebApiClientCore_GetAsync |  9.481 μs | 0.1110 μs | 0.1520 μs |  3.02 |    0.06 | 2.9907 |   4.59 KB |        2.08 |
| Refit_GetAsync            | 20.601 μs | 0.1449 μs | 0.1210 μs |  6.50 |    0.10 | 3.6926 |   5.67 KB |        2.57 |
