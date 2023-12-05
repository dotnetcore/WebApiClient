# 2、全局过滤器

全局过滤器的执行优先级比非全局过滤器的要高，且影响全部的请求方法，其要求实现`IApiActionFilter`接口，并实例化添加到`HttpApiConfig`的GlobalFilters。像`[TraceFilter]`等一般过滤器，也是实现了`IApiActionFilter`接口，也可以添加到`GlobalFilters`作为全局过滤器。

## 2.1 自定义全局过滤器

```csharp
class MyGlobalFilter : IApiActionFilter
{
    public Task OnBeginRequestAsync(ApiActionContext context)
    {
        // do something
        return Task.CompletedTask;
    }

    public Task OnEndRequestAsync(ApiActionContext context)
    {
        // do something
        return Task.CompletedTask;
    }
}
```

添加到GlobalFilters

```csharp
var myFilter = new MyGlobalFilter();
HttpApi.Register<IUserApi>().ConfigureHttpApiConfig(c =>
{
    c.GlobalFilters.Add(myFilter);
});
```

## 2.2 自定义OAuth2全局过滤器

```csharp
/// <summary>
/// 表示提供client_credentials方式的token过滤器
/// </summary>
public class TokenFilter : AuthTokenFilter
{
    /// <summary>
    /// 获取提供Token获取的Url节点
    /// </summary>
    public string TokenEndpoint { get; private set; }

    /// <summary>
    /// 获取client_id
    /// </summary>
    public string ClientId { get; private set; }

    /// <summary>
    /// 获取client_secret
    /// </summary>
    public string ClientSecret { get; private set; }

    /// <summary>
    /// OAuth授权的token过滤器
    /// </summary>
    /// <param name="tokenEndPoint">提供Token获取的Url节点</param>
    /// <param name="client_id">客户端id</param>
    /// <param name="client_secret">客户端密码</param>
    public TokenFilter(string tokenEndPoint, string client_id, string client_secret)
    {
        this.TokenEndpoint = tokenEndPoint ?? throw new ArgumentNullException(nameof(tokenEndPoint));
        this.ClientId = client_id ?? throw new ArgumentNullException(nameof(client_id));
        this.ClientSecret = client_secret ?? throw new ArgumentNullException(nameof(client_secret));
    }

    /// <summary>
    /// 请求获取token
    /// 可以使用TokenClient来请求
    /// </summary>
    /// <returns></returns>
    protected override async Task<TokenResult> RequestTokenResultAsync()
    {
        var tokenClient = new TokenClient(this.TokenEndpoint);
        return await tokenClient.RequestClientCredentialsAsync(this.ClientId, this.ClientSecret);
    }

    /// <summary>
    /// 请求刷新token
    /// 可以使用TokenClient来刷新
    /// </summary>
    /// <param name="refresh_token">获取token时返回的refresh_token</param>
    /// <returns></returns>
    protected override async Task<TokenResult> RequestRefreshTokenAsync(string refresh_token)
    {
        var tokenClient = new TokenClient(this.TokenEndpoint);
        return await tokenClient.RequestRefreshTokenAsync(this.ClientId, this.ClientSecret, refresh_token);
    }
}
```

添加到GlobalFilters

```csharp
var tokenFilter = new TokenFilter ("http://localhost/tokenEndpoint","client","secret");
HttpApi.Register<IUserApi>().ConfigureHttpApiConfig(c =>
{
    c.GlobalFilters.Add(tokenFilter);
});
```
