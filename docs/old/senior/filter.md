# 1、过滤器

过滤器的接口是IApiActionFilterAttribute，WebApiClient提供默认ApiActionFilterAttribute抽象基类，比从IApiActionFilterAttribute实现一个过滤器要简单得多。

## 1.1 TraceFilterAttribute

这是一个用于调试追踪的过滤器，可以将请求与响应内容写入指定输出目标。如果输出目标是LoggerFactory，需要在HttpApiConfig配置LoggerFactory实例或ServiceProvider实例。

接口或方法使用[TraceFilter]

```csharp
[TraceFilter(OutputTarget = OutputTarget.Console)] // 输出到控制台窗口
public interface IUserApi : IHttpApi
{
    // GET {url}?account={account}&password={password}&something={something}
    [HttpGet]
    [Timeout(10 * 1000)] // 10s超时
    Task<string> GetAboutAsync(
        [Url] string url,
        UserInfo user,
        string something);
}
```

请求之后输出请求信息

```csharp
var userApi = HttpApi.Resolve<IUserApi>();
var about = await userApi.GetAboutAsync("webapi/user/about", user, "somevalue");
```

```text
IUserApi.GetAboutAsync
[REQUEST] 2018-10-08 23:55:25.775
GET /webapi/user/about?Account=laojiu&password=123456&BirthDay=2018-01-01&Gender=1&something=somevalue HTTP/1.1
Host: localhost:9999
[RESPONSE] 2018-10-08 23:55:27.047
This is from NetworkSocket.Http
[TIMESPAN] 00:00:01.2722715
```

## 1.2 自定义过滤器

```csharp
[SignFilter]
public interface IUserApi : IHttpApi
{
    ...
}

class SignFilter : ApiActionFilterAttribute
{
    public override Task OnBeginRequestAsync(ApiActionContext context)
    {
        var sign = DateTime.Now.Ticks.ToString();
        context.RequestMessage.AddUrlQuery("sign", sign);
        return base.OnBeginRequestAsync(context);
    }
}
```

当我们需要为每个请求的url额外的动态添加一个叫sign的参数，这个sign可能和配置文件等有关系，而且每次都需要计算，就可以如上设计与应用一个SignFilter。
