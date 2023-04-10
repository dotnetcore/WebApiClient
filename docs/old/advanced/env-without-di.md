# 1、没有依赖注入的环境

## 1.1 使用HttpApi.Register/Resolve

接口声明

```csharp
public interface IMyWebApi : IHttpApi
{
    [HttpGet("user/{id}")]
    ITask<UserInfo> GetUserAsync(string id);
}
```

初始化代码（只能调用一次）

```csharp
HttpApi.Register<IMyWebApi>().ConfigureHttpApiConfig(c =>
{
    // 可以替换的序列化工具
    c.JsonFormatter = null;
    c.XmlFormatter = null;
    c.KeyValueFormatter = null;

    // 参数验证和返回值验证，使用System.ComponentModel.DataAnnotations验证特性
    c.UseParameterPropertyValidate = false;
    c.UseReturnValuePropertyValidate = false;

    // 请求主机和HttpClient相关配置
    c.HttpHost = new Uri("http://localhost:9999/");
    c.HttpClient.Timeout = TimeSpan.FromMinutes(2d);               

    // 格式相关配置
    c.FormatOptions.UseCamelCase = true;
    c.FormatOptions.DateTimeFormat = DateTimeFormats.ISO8601_WithMillisecond;

    // 响应缓存提供者配置，配合[CacheAttribute]来使用
    c.ResponseCacheProvider = null; 

    // 服务提供者，实例一般由DI创建得到
    // 对于Asp.net core，此ServiceProvider应该为请求时创建的ServiceProvider，而不是ConfigureServices()创建的根ServiceProvider
    c.ServiceProvider = null;
    // 日志工厂，可以自主创建并赋值，如果保留为null，获取其实例时则从ServiceProvider获取 
    c.LoggerFactory = null;
});
```

调用http请求代码

```csharp
var myWebApi = HttpApi.Resolve<IMyWebApi>();
var user = await myWebApi.GetUserAsync("id001");
```

使用Register/Resolve的好处是在入口处只Register一次IMyWebApi，由HttpApiFactory自动接理IMyWebApi的生命周期管理。在使用中，不用处理myWebApi实例的释放（手动Dispose也不会释放），在一定的时间内都是获取到同一个myWebApi实例，当实例生命超过配置的周期时，自动被跟踪释放，并提供返回下一个一样配置的myWebApi实例。
