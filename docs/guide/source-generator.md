# SourceGenerator

SourceGenerator是一种新的c#编译时代码补充生成技术，可以非常方便的为WebApiClient生成接口的代理实现类，使用SourceGenerator扩展包，可以替换默认的内置Emit生成代理类的方案，支持需要完全AOT编译的平台。

引用WebApiClientCore.Extensions.SourceGenerator，并在项目启用如下配置:

```csharp
// 应用编译时生成接口的代理类型代码
services
    .AddWebApiClient()
    .UseSourceGeneratorHttpApiActivator();
```
