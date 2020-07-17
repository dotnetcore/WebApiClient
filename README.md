## WebApiClient 　　　　　
一款声明式的http客户端库，只需要定义c#接口并修饰相关特性，即可异步调用远程http接口。

### 1 Nuget包
| 包名 | 描述 | Nuget |
---|---|--|
| WebApiClient.JIT | 适用于非AOT编译的所有平台，稳定性好 | [![NuGet](https://buildstats.info/nuget/WebApiClient.JIT)](https://www.nuget.org/packages/WebApiClient.JIT) |
| WebApiClient.AOT | 适用于所有平台，包括IOS和UWP，复杂依赖项目可能编译不通过 | [![NuGet](https://buildstats.info/nuget/WebApiClient.AOT)](https://www.nuget.org/packages/WebApiClient.AOT) | 

### 2. Http请求
> 接口的声明

```c#
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

```c#
HttpApi.Register<IUserApi>().ConfigureHttpApiConfig(c =>
{
    c.HttpHost = new Uri("http://www.webapiclient.com/");
    c.FormatOptions.DateTimeFormat = DateTimeFormats.ISO8601_WithMillisecond;
});;
```

> 接口的调用

```c#
var api = HttpApi.Resolve<IUserApi>();
var user = new UserInfo { Account = "laojiu", Password = "123456" }; 
var user1 = await api.GetAsync("laojiu");
var state = await api.AddAsync(user);
```  

#### 3. Wiki文档
1. [WebApiClient基础](https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80)
2. [WebApiClient进阶](https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E8%BF%9B%E9%98%B6)
3. [WebApiClient高级](https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E9%AB%98%E7%BA%A7)
4. [WebApiClient.Extensions](https://github.com/xljiulang/WebApiClient.Extensions)
5. [WebApiClient.Tools.Swagger](https://github.com/xljiulang/WebApiClient.Tools)

### 4 QQ群
> 加Q群[825135345](https://shang.qq.com/wpa/qunwpa?idkey=c6df21787c9a774ca7504a954402c9f62b6595d1e63120eabebd6b2b93007410),注明WeApiClient

