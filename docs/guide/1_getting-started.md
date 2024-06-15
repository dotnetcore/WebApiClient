# 快速上手

## 依赖环境

`WebApiclientCore`要求项目的`.NET`版本支持`.NET Standard2.1`，并且具备依赖注入的环境。

## 从 Nuget 安装

| 包名                                                                                                                    | Nuget                                                                                   | 描述                                                                      |
| ----------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------- | ------------------------------------------------------------------------- |
| [WebApiClientCore](https://www.nuget.org/packages/WebApiClientCore)                                                     | ![NuGet logo](https://buildstats.info/nuget/WebApiClientCore)                           | 基础包                                                                    |
| [WebApiClientCore.Extensions.OAuths](https://www.nuget.org/packages/WebApiClientCore.Extensions.OAuths)                 | ![NuGet logo](https://buildstats.info/nuget/WebApiClientCore.Extensions.OAuths)         | OAuth2 与 token 管理扩展包                                                |
| [WebApiClientCore.Extensions.NewtonsoftJson](https://www.nuget.org/packages/WebApiClientCore.Extensions.NewtonsoftJson) | ![NuGet logo](https://buildstats.info/nuget/WebApiClientCore.Extensions.NewtonsoftJson) | Newtonsoft 的 Json.NET 扩展包                                                           |
| [WebApiClientCore.Extensions.JsonRpc](https://www.nuget.org/packages/WebApiClientCore.Extensions.JsonRpc)               | ![NuGet logo](https://buildstats.info/nuget/WebApiClientCore.Extensions.JsonRpc)        | JsonRpc 调用扩展包                                                        |
| [WebApiClientCore.OpenApi.SourceGenerator](https://www.nuget.org/packages/WebApiClientCore.OpenApi.SourceGenerator)     | ![NuGet logo](https://buildstats.info/nuget/WebApiClientCore.OpenApi.SourceGenerator)   | 将本地或远程 OpenApi 文档解析生成 WebApiClientCore 接口代码的 dotnet tool |

## 声明接口

```csharp
[LoggingFilter]
[HttpHost("http://localhost:5000/")]
public interface IUserApi
{
    [HttpGet("api/users/{id}")]
    Task<User> GetAsync(string id);

    [HttpPost("api/users")]
    Task<User> PostAsync([JsonContent] User user);
}

public class User
{ 
    [JsonPropertyName("account")]
    public string Account { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}
```

## 注册和配置接口

AspNetCore Startup

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpApi<IUserApi>().ConfigureHttpApi(o =>
    {
        o.UseLogging = Environment.IsDevelopment();
        o.HttpHost = new Uri("http://localhost:5000/");

        // o.JsonSerializeOptions -> json 序列化选项
        // o.JsonDeserializeOptions -> json 反序列化选项
        // o.KeyValueSerializeOptions -> 键值对序列化选项
        // o.XmlSerializeOptions -> xml 序列化选项
        // o.XmlDeserializeOptions -> xml 反序列化选项
        // o.GlobalFilters -> 全局过滤器集合
    });
}
```

Console

```csharp
public static void Main(string[] args)
{
    // 无依赖注入的环境需要自行创建
    var services = new ServiceCollection();
    services.AddHttpApi<IUserApi>().ConfigureHttpApi(o =>
    {       
        o.UseLogging = Environment.IsDevelopment();
        o.HttpHost = new Uri("http://localhost:5000/");
    });
}
```

## 全局配置接口

全局配置可以做为所有接口的默认初始配置，当项目中有很多接口时就很有用。

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddWebApiClient().ConfigureHttpApi(o =>
    {
        o.JsonSerializeOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonDeserializeOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.KeyValueSerializeOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
}
```

## 注入和调用接口

在Scoped或Transient服务中注入

```csharp
public class YourService
{
    private readonly IUserApi userApi;
    public YourService(IUserApi userApi)
    {
        this.userApi = userApi;
    }

    public async Task GetAsync()
    {
        // 调用接口
        var user = await this.userApi.GetAsync(id:"id001");
        ...
    }
}
```

在Singleton服务中注入

```csharp
public class YourService
{
    private readonly IServiceScopeFactory serviceScopeFactory;
    public YourService(IServiceScopeFactory serviceScopeFactory)
    {
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public async Task GetAsync()
    {
        // 从创建的scope中获取接口实例
        using var scope = this.serviceScopeFactory.CreateScope();
        var userApi = scope.ServiceProvider.GetRequiredService<IUserApi>();
        var user = await userApi.GetAsync(id:"id001");
        ...
    }
}
```
