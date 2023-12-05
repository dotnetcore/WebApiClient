﻿# 概览

[![Member project of .NET Core Community](https://img.shields.io/badge/member%20project%20of-NCC-9e20c9.svg)](https://github.com/dotnetcore)
[![nuget](https://img.shields.io/nuget/v/WebApiClientCore.svg?style=flat-square)](https://www.nuget.org/packages/WebApiClientCore)
[![nuget](https://img.shields.io/nuget/vpre/WebApiClientCore.svg?style=flat-square)](https://www.nuget.org/packages/WebApiClientCore)
[![stats](https://img.shields.io/nuget/dt/WebApiClientCore.svg?style=flat-square)](https://www.nuget.org/stats/packages/WebApiClientCore?groupby=Version)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](https://github.com/dotnetcore/WebApiClient/blob/master/LICENSE)

[![Stargazers over time](https://starchart.cc/dotnetcore/WebApiClient.svg)](https://starchart.cc/dotnetcore/WebApiClient)

## 简介

WebApiClient有两个版本

+ `WebApiclientCore` 基于`.NET Standard2.1`重新设计的新版本，与全新的`依赖注入`、`配置`、`选项`、`日志`等重新设计过的.NET抽象Api完美契合
+ `WebApiClient.JIT`、`WebApiClient.AOT` 基于`.NET Standard2.0`的旧版本(额外支持`.NET Framework 4.5+`)，支持`.NET Core 2.0+`,在老版本的.NET上亦能独当一面
+ [QQ群 825135345](<https://shang.qq.com/wpa/qunwpa?idkey=c6df21787c9a774ca7504a954402c9f62b6595d1e63120eabebd6b2b93007410>)进群时请注明**WebApiClient**，在咨询问题之前，请先认真阅读以下剩余的文档，避免消耗作者不必要的重复解答时间。
+ 反馈问题请前往 [https://github.com/dotnetcore/WebApiClient/issues](https://github.com/dotnetcore/WebApiClient/issues)

## 特性

+ 支持编译时代理类生成包，提高运行时性能和兼容性
+ 支持OAuth2与token管理扩展包，方便实现身份认证和授权
+ 支持Json.Net扩展包，提供灵活的Json序列化和反序列化
+ 支持JsonRpc调用扩展包，支持使用JsonRpc协议进行远程过程调用
+ 支持将本地或远程OpenApi文档解析生成WebApiClientCore接口代码的dotnet tool，简化接口声明的工作量
+ 提供接口声明的语法分析与提示，帮助开发者避免使用不当的语法
