```

BenchmarkDotNet v0.13.12, CentOS Linux 7 (Core)
Intel Xeon CPU E5-2650 v2 2.60GHz, 2 CPU, 32 logical and 16 physical cores
  [Host] : .NET 8.0.4, X64 NativeAOT AVX

Job=InProcess  Toolchain=InProcessEmitToolchain  

```
| Method                        | Mean     | Error    | StdDev   | Ratio | RatioSD | Gen0    | Gen1   | Allocated | Alloc Ratio |
|------------------------------ |---------:|---------:|---------:|------:|--------:|--------:|-------:|----------:|------------:|
| WebApiClientCore_PostXmlAsync | 48.34 μs | 0.926 μs | 0.866 μs |  1.00 |    0.00 |  3.4180 |      - |  35.48 KB |        1.00 |
| Refit_PostXmlAsync            | 59.37 μs | 1.180 μs | 2.961 μs |  1.20 |    0.06 | 14.0381 | 2.3193 | 144.38 KB |        4.07 |
