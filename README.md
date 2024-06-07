[README](README.md) | [中文文档](README_zh.md)

## WebApiClient　　　　　　　　　　　　　　　　　
A REST API library with better functionality, performance, and scalability than refit.

### Features
#### Semantic Declaration
Client development only requires semantic declaration of C# interfaces.

#### Diverse serialization
Supports json, xml, form and other custom serialization methods.

#### Full trimmed and AOT
Supports full trimmed and AOT publishing of .NET8.

#### Aspect-Oriented Programming
Supports multiple interceptors, filters, logs, retries, custom caches and other aspects.

#### Code Syntax Analysis
Provides syntax analysis and prompts for interface code declarations to help developers avoid using improper syntax when declaring interfaces.

#### Quick access
Supports OAuth2 and token management extension packages to facilitate identity authentication and authorization.

#### Swagger to code
Supports parsing local or remote OpenApi documents to generate WebApiClientCore interface code, which simplifies the workload of interface declaration.

#### Powerful performance
In [BenchmarkDotNet](WebApiClientCore.Benchmarks/results), the performance is 2.X times ahead of the similar product [refit](https://github.com/reactiveui/refit) under various requests.

### Documentation support
https://webapiclient.github.io/
