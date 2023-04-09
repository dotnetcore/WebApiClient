# 快速上手

::: warning

+ 如果你的项目所运行的.NET版本支持`.NET Standard2.1`，并具备依赖注入的环境，我们强烈建议你直接使用全新的`WebApiclientCore`
+ `WebApiClient.JIT`、`WebApiClient.AOT` 目前处于 `修补性维护` 阶段。你仍可用用它来构建项目，但我们仅修补致命性错误而不会为其带来任何功能性的更新。

:::

## 依赖环境

 对于`WebApiclientCore`，由于基于`.NET Standard2.1`它可以运行在以下平台

+ .NET Core 3 +
+ .NET 5、6、7、8
+ Mono 6.4 +
+ Xamarin.iOS 12.16 +
+ Xamarin.Mac 5.16 +
+ Xamarin.Android 10 +
+ 包括但不限于以上列举的实现`.NET Standard2.1`的平台

 对于`WebApiClient.JIT`、`WebApiClient.AOT`，由于基于`.NET Standard2.0`它可以运行在以下平台

+ .NET Framework 4.6.1+
+ .NET Core 2 +
+ .NET Core 3 +
+ .NET 5、6、7、8
+ Mono 4.6 +
+ Xamarin.iOS 10 +
+ Xamarin.Mac 3 +
+ Xamarin.Android 7 +
+ 通用Windows平台10 +
+ 包括但不限于以上列举的实现`.NET Standard2.0`的平台
+ 额外支持.NET Framework 4.5

## 从Nuget安装

这一章节会帮助你从头搭建一个简单的 VuePress 文档网站。如果你想在一个现有项目中使用 VuePress 管理文档，从步骤 3 开始。

<CodeGroup>

  <CodeGroupItem title=".NET CLI" active>

```bash
dotnet add package WebApiClientCore
```

  </CodeGroupItem>

  <CodeGroupItem title=" Package Manager">

```bash
NuGet\Install-Package WebApiClientCore 
```

  </CodeGroupItem>

  <CodeGroupItem title="PackageReference">

```xml
<PackageReference Include="WebApiClientCore" Version="2.0.4" />
```

  </CodeGroupItem>
    <CodeGroupItem title="Paket CLI">

```bash
paket add WebApiClientCore
```

  </CodeGroupItem>
</CodeGroup>

## 声明接口

```csharp
[HttpHost("http://localhost:5000/")]
public interface IUserApi
{
    [HttpGet("api/users/{id}")]
    Task<User> GetAsync(string id);
    
    [HttpPost("api/users")]
    Task<User> PostAsync([JsonContent] User user);
}
```

## 注册接口

AspNetCore Startup

```csharp
public void ConfigureServices(IServiceCollection services)
{
  //注册
  services.AddHttpApi<IUserApi>();
}
```

Console

```csharp
public static void Main(string[] args)
{
    //无依赖注入的环境需要自行创建
    IServiceCollection services = new ServiceCollection();
    services.AddHttpApi<IUserApi>();
}
```

## 配置

```csharp
public void ConfigureServices(IServiceCollection services)
{
  // 注册并配置
  services.AddHttpApi(typeof(IUserApi), o =>
  {
      o.UseLogging = Environment.IsDevelopment();
      o.HttpHost = new Uri("http://localhost:5000/");
  });
  //注册，然后配置
  services.AddHttpApi<IUserApi>().ConfigureHttpApi(o =>
  {
      o.UseLogging = Environment.IsDevelopment();
      o.HttpHost = new Uri("http://localhost:5000/");
  });
  //添加全局配置
  services.AddWebApiClient().ConfigureHttpApi(o =>
  {
      o.UseLogging = Environment.IsDevelopment();
      o.HttpHost = new Uri("http://localhost:5000/");
  });
}
```

## 注入接口

```csharp
public class MyService
{
    private readonly IUserApi userApi;
    public MyService(IUserApi userApi)
    {
        this.userApi = userApi;
    }

    public async Task GetAsync(){
        //使用接口
        var user=await userApi.GetAsync(100);
    }
}
```
