[README](README.md) | [中文文档](README_zh.md)

## WebApiClient　　　　　　　　　　　　　　　　　
一个在功能、性能和可扩展性均优于 refit 的 REST API 库

### 功能特性
#### 语义化声明
客户端的开发，只需语义化的声明接口。

#### 多样序列化
支持json、xml、form等序列化和其它自定义序列化方式。

#### 裁剪与AOT
支持.net8的代码完全裁剪和AOT发布。

#### 面向切面
支持多种拦截器、过滤器、日志、重试、缓存自定义等功能。

#### 语法分析
提供接口声明的语法分析与提示，帮助开发者声明接口时避免使用不当的语法。

#### 快速接入
支持OAuth2与token管理扩展包，方便实现身份认证和授权。

#### 自动代码
支持将本地或远程OpenApi文档解析生成WebApiClientCore接口代码的dotnet tool，简化接口声明的工作量

#### 性能强劲
在[BenchmarkDotNet](WebApiClientCore.Benchmarks/results)中，各种请求下2.X倍性能领先于同类产品[refit](https://github.com/reactiveui/refit)。

### 文档支持
https://webapiclient.github.io/
