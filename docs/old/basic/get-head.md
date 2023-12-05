# 1、GET/HEAD请求

## 1.1 Get请求简单例子

```csharp
public interface IMyWebApi : IHttpApi
{
    // GET <http://www.mywebapi.com/webapi/user?account=laojiu>
    [HttpGet("http://www.mywebapi.com/webapi/user")]
    ITask<HttpResponseMessage> GetUserByAccountAsync(string account);
}

var api = HttpApi.Create<IMyWebApi>();
var response = await api.GetUserByAccountAsync("laojiu");
```

## 1.2 使用`[HttpHost]`特性

```csharp

[HttpHost("http://www.mywebapi.com/")]
public interface IMyWebApi : IHttpApi
{
    // GET /webapi/user?account=laojiu
    [HttpGet("webapi/user")]
    ITask<HttpResponseMessage> GetUserByAccountAsync(string account);
}
```

如果接口IMyWebApi有多个方法且都指向同一服务器，可以将请求的域名抽出来放到HttpHost特性。

## 1.3 响应的json/xml内容转换为强类型模型

### 1.3.1 隐式转换为强类型模型

```csharp

[HttpHost("http://www.mywebapi.com/")]
public interface IMyWebApi : IHttpApi
{
    // GET /webapi/user?account=laojiu
    [HttpGet("webapi/user")]
    ITask<UserInfo> GetUserByAccountAsync(string account);
}
```

当方法的返回数据是UserInfo类型的json或xml文本，且响应的Content-Type为application/json或application/xml值时，方法的原有返回类型ITask(Of HttpResponseMessage)就可以声明为ITask(Of UserInfo)。

### 1.3.2 显式转换为强类型模型

```csharp
[HttpHost("http://www.mywebapi.com/")]
public interface IMyWebApi : IHttpApi
{
    // GET /webapi/user?account=laojiu
    [HttpGet("webapi/user")]  
    [JsonReturn] // 指明使用Json处理返回值为UserInfo类型
    ITask<UserInfo> GetUserByAccountAsync(string account);
}
```

当方法的返回数据是UserInfo类型的json或xml文本，但响应的Content-Type可能不是期望的application/json或application/xml值时，就需要显式声明[JsonReturn]或[XmlReturn]特性。

## 1.4 请求取消令牌参数CancellationToken

```csharp
[HttpHost("http://www.mywebapi.com/")]
public interface IMyWebApi : IHttpApi
{
    // GET /webapi/user?account=laojiu
    [HttpGet("webapi/user")]
    ITask<UserInfo> GetUserByAccountAsync(string account, CancellationToken token);
}
```

CancellationToken.None表示永不取消；创建一个CancellationTokenSource，可以提供一个CancellationToken。
