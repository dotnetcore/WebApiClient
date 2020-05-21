## WebApiClientCore.Benchmarks　
WebApiClientCore、WebApiClient.JIT与原生HttpClient的性能比较


BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18362.778 (1903/May2019Update/19H1)
Intel Core i3-4150 CPU 3.50GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.1.202
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT


|                     Method |       Mean |      Error |     StdDev |
|--------------------------- |-----------:|-----------:|-----------:|
|      WebApiClient_GetAsync | 279.479 us | 22.5466 us | 64.3268 us |
|  WebApiClientCore_GetAsync |  25.298 us |  0.4953 us |  0.7999 us |
|        HttpClient_GetAsync |   2.849 us |  0.0568 us |  0.1393 us |
|     WebApiClient_PostAsync |  25.942 us |  0.3817 us |  0.3188 us |
| WebApiClientCore_PostAsync |  13.462 us |  0.2551 us |  0.6258 us |
|       HttpClient_PostAsync |   4.515 us |  0.0866 us |  0.0926 us |

// * Warnings *
MinIterationTime
  Benchmark.WebApiClient_GetAsync: Default -> The minimum observed iteration time is 174.6000 us which is very small. It's recommended to increase it to at least 100.0000 ms using more operations.

// * Hints *
Outliers
  Benchmark.WebApiClient_GetAsync: Default      -> 6 outliers were removed (490.50 us..1.53 ms)
  Benchmark.WebApiClientCore_GetAsync: Default  -> 2 outliers were removed (30.65 us, 31.51 us)
  Benchmark.HttpClient_GetAsync: Default        -> 3 outliers were removed (3.57 us..3.77 us)
  Benchmark.WebApiClient_PostAsync: Default     -> 3 outliers were removed, 5 outliers were detected (25.39 us, 25.44 us, 27.01 us..27.56 us)
  Benchmark.WebApiClientCore_PostAsync: Default -> 6 outliers were removed (15.54 us..20.17 us)

// * Legends *
  Mean   : Arithmetic mean of all measurements
  Error  : Half of 99.9% confidence interval
  StdDev : Standard deviation of all measurements
  1 us   : 1 Microsecond (0.000001 sec)
