
# OAuths&Token

使用WebApiClientCore.Extensions.OAuths扩展，轻松支持token的获取、刷新与应用。

## 对象与概念

对象 | 用途
---|---
ITokenProviderFactory | tokenProvider的创建工厂，提供通过HttpApi接口类型获取或创建tokenProvider
ITokenProvider | token提供者，用于获取token，在token的过期后的头一次请求里触发重新请求或刷新token
OAuthTokenAttribute | token的应用特性，使用ITokenProviderFactory创建ITokenProvider，然后使用ITokenProvider获取token，最后将token应用到请求消息中
OAuthTokenHandler | 属于http消息处理器，功能与OAuthTokenAttribute一样，除此之外，如果因为意外的原因导致服务器仍然返回未授权(401状态码)，其还会丢弃旧token，申请新token来重试一次请求。

## OAuth的Client模式

### 1 为接口注册tokenProvider

```csharp
// 为接口注册与配置Client模式的tokenProvider
services.AddClientCredentialsTokenProvider<IUserApi>(o =>
{
    o.Endpoint = new Uri("http://localhost:6000/api/tokens");
    o.Credentials.Client_id = "clientId";
    o.Credentials.Client_secret = "xxyyzz";
});
```

### 2 token的应用

#### 2.1 使用OAuthToken特性

OAuthTokenAttribute属于WebApiClientCore框架层，很容易操控请求内容和响应模型，比如将token作为表单字段添加到既有请求表单中，或者读取响应消息反序列化之后对应的业务模型都非常方便，但它不能在请求内部实现重试请求的效果。在服务器颁发token之后，如果服务器的token丢失了，使用OAuthTokenAttribute会得到一次失败的请求，本次失败的请求无法避免。

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

OAuthTokenAttribute默认实现将token放到Authorization请求头，如果你的接口需要请token放到其它地方比如uri的query，需要重写OAuthTokenAttribute：

```csharp
class UriQueryTokenAttribute : OAuthTokenAttribute
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

#### 2.1 使用OAuthTokenHandler

OAuthTokenHandler的强项是支持在一个请求内部里进行多次尝试，在服务器颁发token之后，如果服务器的token丢失了，OAuthTokenHandler在收到401状态码之后，会在本请求内部丢弃和重新请求token，并使用新token重试请求，从而表现为一次正常的请求。但OAuthTokenHandler不属于WebApiClientCore框架层的对象，在里面只能访问原始的HttpRequestMessage与HttpResponseMessage，如果需要将token追加到HttpRequestMessage的Content里，这是非常困难的，同理，如果不是根据http状态码(401等)作为token无效的依据，而是使用HttpResponseMessage的Content对应的业务模型的某个标记字段，也是非常棘手的活。

```csharp
// 注册接口时添加OAuthTokenHandler
services
    .AddHttpApi<IUserApi>()
    .AddOAuthTokenHandler();
```

OAuthTokenHandler默认实现将token放到Authorization请求头，如果你的接口需要请token放到其它地方比如uri的query，需要重写OAuthTokenHandler：

```csharp
class UriQueryOAuthTokenHandler : OAuthTokenHandler
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
        
        var uriValue = new UriValue(request.RequestUri).AddQuery("myToken", tokenResult.Access_token);
        request.RequestUri = uriValue.ToUri();
    }
}


// 注册接口时添加UriQueryOAuthTokenHandler
services
    .AddHttpApi<IUserApi>()
    .AddOAuthTokenHandler((s, tp) => new UriQueryOAuthTokenHandler(tp));
```

## 多接口共享的TokenProvider

可以给http接口设置基础接口，然后为基础接口配置TokenProvider，例如下面的xxx和yyy接口，都属于IBaidu，只需要给IBaidu配置TokenProvider。

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

## 自定义TokenProvider

扩展包已经内置了OAuth的Client和Password模式两种标准token请求，但是仍然还有很多接口提供方在实现上仅仅体现了它的精神，这时候就需要自定义TokenProvider，假设接口提供方的获取token的接口如下：

```csharp
public interface ITokenApi
{
    [HttpPost("http://xxx.com/token")]
    Task<TokenResult> RequestTokenAsync([Parameter(Kind.Form)] string clientId, [Parameter(Kind.Form)] string clientSecret);
}
```

### 委托TokenProvider

委托TokenProvider是一种最简单的实现方式，它将请求token的委托作为自定义TokenProvider的实现逻辑：

```csharp
// 为接口注册自定义tokenProvider
services.AddTokeProvider<IUserApi>(s =>
{
    return s.GetService<ITokenApi>().RequestTokenAsync("id", "secret");
});
```

### 完整实现的TokenProvider

```csharp
// 为接口注册CustomTokenProvider
services.AddTokeProvider<IUserApi, CustomTokenProvider>();
```

```csharp
class CustomTokenProvider : TokenProvider
{
    public CustomTokenProvider(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    protected override Task<TokenResult> RequestTokenAsync(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetService<ITokenApi>().RequestTokenAsync("id", "secret");
    }

    protected override Task<TokenResult> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token)
    {
        return this.RequestTokenAsync(serviceProvider);
    }
}
```

### 自定义TokenProvider的选项

每个TokenProvider都有一个Name属性，与service.AddTokeProvider()返回的ITokenProviderBuilder的Name是同一个值。读取Options值可以使用TokenProvider的GetOptionsValue()方法，配置Options则通过ITokenProviderBuilder的Name来配置。
