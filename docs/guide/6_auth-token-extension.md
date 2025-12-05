# OAuths&Token 扩展

使用 WebApiClientCore.Extensions.OAuths 扩展，轻松支持 token 的获取、刷新与应用。

## 对象与概念
### ITokenProviderFactory
ITokenProvider 的创建工厂，提供通过 HttpApi 接口类型获取或创建 ITokenProvider。

### ITokenProvider
token 提供者，用于获取 token，在 token 的过期后的头一次请求里触发重新请求或刷新 token。 

### OAuthTokenAttribute
token 的应用特性，使用 ITokenProviderFactory 创建 ITokenProvider，然后使用 ITokenProvider 获取 token，最后将 token 应用到请求消息中。

### OAuthTokenHandler
属于 http 消息处理器，功能与 OAuthTokenAttribute 一样，除此之外，如果因为意外的原因导致服务器仍然返回未授权(401 状态码)，其还会丢弃旧 token，申请新 token 来重试一次请求。

## Token 提前刷新

为避免 token 在检查后到实际使用前过期，支持配置提前刷新时间窗口。

### 默认行为

默认启用提前刷新，使用 Auto 策略：
- 固定窗口：60 秒
- 百分比窗口：10%
- 实际窗口：取两者较小值

### 配置方式1：通过 HttpApiOptions

```csharp
services.AddHttpApi<IUserApi>(o =>
{
    o.ConfigureOAuthToken(t =>
    {
        t.RefreshWindowSeconds = 120;           // 固定窗口120秒
        t.RefreshWindowPercentage = 0.15;       // 百分比15%
        t.RefreshWindowStrategy = RefreshWindowStrategy.Auto;
    });
});
```

### 配置方式2：通过独立配置

```csharp
services.AddHttpApi<IUserApi>()
    .AddOAuthTokenHandler()
    .ConfigureOAuthTokenOptions(o =>
    {
        o.RefreshWindowSeconds = 120;
        o.RefreshWindowStrategy = RefreshWindowStrategy.FixedSeconds;
    });
```

### 从配置文件加载

**appsettings.json:**
```json
{
  "OAuthToken": {
    "IUserApi": {
      "UseTokenRefreshWindow": true,
      "RefreshWindowSeconds": 120,
      "RefreshWindowPercentage": 0.15,
      "RefreshWindowStrategy": "Auto"
    }
  }
}
```

**Program.cs:**
```csharp
services.AddHttpApi<IUserApi>()
    .ConfigureOAuthTokenOptions(configuration.GetSection("OAuthToken:IUserApi"));
```

### 刷新策略

| 策略 | 说明 | 适用场景 |
|------|------|----------|
| `FixedSeconds` | 固定秒数 | Token 有效期固定 |
| `Percentage` | 百分比 | Token 有效期不固定 |
| `Auto` (默认) | 取较小值 | 通用场景 |

### 禁用提前刷新

```csharp
services.AddHttpApi<IUserApi>(o =>
{
    o.ConfigureOAuthToken(t => t.UseTokenRefreshWindow = false);
});
```
 

## OAuth 的 Client 模式

### 为接口注册 TokenProvider

```csharp
// 为接口注册与配置Client模式的tokenProvider
services.AddClientCredentialsTokenProvider<IUserApi>(o =>
{
    o.Endpoint = new Uri("http://localhost:6000/api/tokens");
    o.Credentials.Client_id = "clientId";
    o.Credentials.Client_secret = "xxyyzz";
});
```

### token 的应用

#### 使用 OAuthToken 特性

OAuthTokenAttribute 属于 WebApiClientCore 框架层，很容易操控请求内容和响应模型，比如将 token 作为表单字段添加到既有请求表单中，或者读取响应消息反序列化之后对应的业务模型都非常方便，但它不能在请求内部实现重试请求的效果。在服务器颁发 token 之后，如果服务器的 token 丢失了，使用 OAuthTokenAttribute 会得到一次失败的请求，本次失败的请求无法避免。

```csharp
/// <summary>
/// 用户操作接口
/// </summary>
[OAuthToken]
public interface IUserApi
{
    ...
}
```

OAuthTokenAttribute 默认实现将 token 放到 Authorization 请求头，如果你的接口需要请 token 放到其它地方比如 Uri 的 Query，需要重写 OAuthTokenAttribute：

```csharp
public class UriQueryTokenAttribute : OAuthTokenAttribute
{
    protected override void UseTokenResult(ApiRequestContext context, TokenResult tokenResult)
    {
        context.HttpContext.RequestMessage.AddUrlQuery("mytoken", tokenResult.Access_token);
    }
}

[UriQueryToken]
public interface IUserApi
{
    ...
}
```

#### 使用 OAuthTokenHandler

OAuthTokenHandler 的强项是支持在一个请求内部里进行多次尝试，在服务器颁发 token 之后，如果服务器的 token 丢失了，OAuthTokenHandler 在收到 401 状态码之后，会在本请求内部丢弃和重新请求 token，并使用新 token 重试请求，从而表现为一次正常的请求。但 OAuthTokenHandler 不属于 WebApiClientCore 框架层的对象，在里面只能访问原始的 HttpRequestMessage 与 HttpResponseMessage，如果需要将 token 追加到 HttpRequestMessage 的 Content 里，这是非常困难的，同理，如果不是根据 http 状态码(401 等)作为 token 无效的依据，而是使用 HttpResponseMessage 的 Content 对应的业务模型的某个标记字段，也是非常棘手的活。

```csharp
// 注册接口时添加OAuthTokenHandler
services
    .AddHttpApi<IUserApi>()
    .AddOAuthTokenHandler();
```

OAuthTokenHandler 默认实现将 token 放到 Authorization 请求头，如果你的接口需要请 token 放到其它地方比如 uri 的 query，需要重写 OAuthTokenHandler：

```csharp
public class UriQueryOAuthTokenHandler : OAuthTokenHandler
{
    /// <summary>
    /// token应用的http消息处理程序
    /// </summary>
    /// <param name="tokenProvider">token提供者</param>
    public UriQueryOAuthTokenHandler(ITokenProvider tokenProvider)
        : base(tokenProvider)
    {
    }

    /// <summary>
    /// 应用token
    /// </summary>
    /// <param name="request"></param>
    /// <param name="tokenResult"></param>
    protected override void UseTokenResult(HttpRequestMessage request, TokenResult tokenResult)
    {
        // var builder = new UriBuilder(request.RequestUri);
        // builder.Query += "mytoken=" + Uri.EscapeDataString(tokenResult.Access_token);
        // request.RequestUri = builder.Uri;

        var uriValue = new UriValue(request.RequestUri);
        uriValue = uriValue.AddQuery("myToken", tokenResult.Access_token);
        request.RequestUri = uriValue.ToUri();
    }
}


// 注册接口时添加UriQueryOAuthTokenHandler
services
    .AddHttpApi<IUserApi>()
    .AddOAuthTokenHandler((s, tp) => new UriQueryOAuthTokenHandler(tp));
```

## 多接口共享的 TokenProvider

可以给 http 接口设置基础接口，然后为基础接口配置 TokenProvider，例如下面的 xxx 和 yyy 接口，都属于 IBaidu，只需要给 IBaidu 配置 TokenProvider。

```csharp
[OAuthToken]
public interface IBaidu
{
}

public interface IBaidu_XXX_Api : IBaidu
{
    [HttpGet]
    Task xxxAsync();
}

public interface IBaidu_YYY_Api : IBaidu
{
    [HttpGet]
    Task yyyAsync();
}
```

```csharp
// 注册与配置password模式的token提者选项
services.AddPasswordCredentialsTokenProvider<IBaidu>(o =>
{
    o.Endpoint = new Uri("http://localhost:5000/api/tokens");
    o.Credentials.Client_id = "clientId";
    o.Credentials.Client_secret = "xxyyzz";
    o.Credentials.Username = "username";
    o.Credentials.Password = "password";
});
```

## 自定义 TokenProvider

扩展包已经内置了 OAuth 的 Client 和 Password 模式两种标准 token 请求，但是仍然还有很多接口提供方在实现上仅仅体现了它的精神，这时候就需要自定义 TokenProvider，假设接口提供方的获取 token 的接口如下：

```csharp
public interface ITokenApi
{
    [HttpPost("http://xxx.com/token")]
    Task<TokenResult> RequestTokenAsync([Parameter(Kind.Form)] string clientId, [Parameter(Kind.Form)] string clientSecret);
}
```

### 委托 TokenProvider

委托 TokenProvider 是一种最简单的实现方式，它将请求 token 的委托作为自定义 TokenProvider 的实现逻辑：

```csharp
// 为接口注册自定义tokenProvider
services.AddTokenProvider<IUserApi>(s =>
{
    return s.GetRequiredService<ITokenApi>().RequestTokenAsync("id", "secret");
});
```

### 完整实现的 TokenProvider

```csharp
// 为接口注册CustomTokenProvider
services.AddTokenProvider<IUserApi, CustomTokenProvider>();
```

```csharp
public class CustomTokenProvider : TokenProvider
{
    public CustomTokenProvider(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    protected override Task<TokenResult> RequestTokenAsync(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetRequiredService<ITokenApi>().RequestTokenAsync("id", "secret");
    }

    protected override Task<TokenResult> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token)
    {
        return this.RequestTokenAsync(serviceProvider);
    }
}
```

### 自定义 TokenProvider 的选项

每个 TokenProvider 都有一个 Name 属性，与 service.AddTokenProvider()返回的 ITokenProviderBuilder 的 Name 是同一个值。读取 Options 值可以使用 TokenProvider 的 GetOptionsValue()方法，配置 Options 则通过 ITokenProviderBuilder 的 Name 来配置。
