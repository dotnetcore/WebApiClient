# 快速开始

## 1 Nuget包

| 包名 | 描述 | Nuget |
---|---|--|
| WebApiClient.JIT | 适用于非AOT编译的所有平台，稳定性好 | [![NuGet](https://buildstats.info/nuget/WebApiClient.JIT)](https://www.nuget.org/packages/WebApiClient.JIT) |
| WebApiClient.AOT | 适用于所有平台，包括IOS和UWP，复杂依赖项目可能编译不通过 | [![NuGet](https://buildstats.info/nuget/WebApiClient.AOT)](https://www.nuget.org/packages/WebApiClient.AOT) |

## 2. Http请求
>
> 接口的声明

```csharp
public interface IUserApi : IHttpApi
{
    // GET api/user?account=laojiu
    // Return json或xml内容
    [HttpGet("api/user")]
    ITask<UserInfo> GetAsync(string account);

    // POST api/user  
    // Body Account=laojiu&password=123456
    // Return json或xml内容
    [HttpPost("api/user")]
    ITask<boo> AddAsync([FormContent] UserInfo user);
}
```

> 接口的配置

```csharp
HttpApi.Register<IUserApi>().ConfigureHttpApiConfig(c =>
{
    c.HttpHost = new Uri("http://www.webapiclient.com/");
    c.FormatOptions.DateTimeFormat = DateTimeFormats.ISO8601_WithMillisecond;
});;
```

> 接口的调用

```csharp
var api = HttpApi.Resolve<IUserApi>();
var user = new UserInfo { Account = "laojiu", Password = "123456" }; 
var user1 = await api.GetAsync("laojiu");
var state = await api.AddAsync(user);
```
