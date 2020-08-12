# ApplicationInsightBenchMark

Run the following from the project folder to run the benchmark.

```sh
dotnet run --configuration Release
```

Actual output from one of my runs.

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1016 (1909/November2018Update/19H2)
Intel Core i7-8650U CPU 1.90GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.401
  [Host]     : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
  DefaultJob : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT


|               Method |     Mean |     Error |    StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |---------:|----------:|----------:|-------:|------:|------:|----------:|
|                Basic | 1.428 us | 0.0301 us | 0.0857 us | 0.1945 |     - |     - |     816 B |
| WithRequestExtractor | 2.698 us | 0.0525 us | 0.0605 us | 0.4082 |     - |     - |    1720 B |