# 2、请求URI

## 2.1 URI的格式

无论是GET还是POST等哪种http请求方法，都遵循如下的URI格式：
{Scheme}://{UserName}:{Password}@{Host}:{Port}{Path}{Query}{Fragment}
例如：`<http://account:password@www.baidu.com/path1/?p1=abc#tag>`

## 2.2 动态PATH

```csharp
public interface IMyWebApi : IHttpApi
{
    // GET <http://www.webapiclient.com/laojiu>
    [HttpGet("http://www.webapiclient.com/{account}"]
    ITask<string> GetUserByAccountAsync(string account);
}
```

某些接口方法将路径的一个分段语意化，比如GET `<http://www.webapiclient.com/{account}`，这里不同的`{account}`代表不同账号下的个人信息，使用{参数名}声明路径，在请求前会自动从参数（或参数模型的同名属性）取值替换>。

## 2.3 动态URI

```csharp
public interface IMyWebApi : IHttpApi
{
    // GET {URI}
    [HttpGet]
    ITask<string> GetUserByAccountAsync([Uri] string url);

    // GET {URI}?account=laojiu
    [HttpGet]
    ITask<string> GetUserByAccountAsync([Uri] string url, string account);
}
```

如果请求URI在运行时才确定，可以将请求URI作为一个参数，使用`[Uri]`特性修饰这个参数并作为第一个参数。

## 2.4 Query参数

### 2.4.1 多个query参数平铺

```csharp
// GET /webapi/user?account=laojiu&password=123456
[HttpGet("webapi/user")]
ITask<UserInfo> GetUserAsync(string account, string password);
```

### 2.4.2 多个query参数合并到模型

```csharp
public class LoginInfo
{
    public string Account { get; set; }
    public string Password { get; set; }
}

// GET /webapi/user?account=laojiu&password=123456
[HttpGet("webapi/user")]
ITask<UserInfo> GetUserAsync(LoginInfo loginInfo);
```

### 2.4.3 多个query参数平铺+部分合并到模型

```csharp
public class LoginInfo
{
    public string Account { get; set; }
    public string Password { get; set; }
}

// GET /webapi/user?account=laojiu&password=123456&role=admin
[HttpGet("webapi/user")]
ITask<UserInfo> GetUserAsync(LoginInfo loginInfo, string role);
```

### 2.4.4 显式声明`[PathQuery]`特性

```csharp
// GET /webapi/user?account=laojiu&password=123456&role=admin
[HttpGet("webapi/user")]
ITask<UserInfo> GetUserAsync(
    [PathQuery]LoginInfo loginInfo,
    [PathQuery]string role);
```

对于没有任何特性修饰的每个参数，都默认被`[PathQuery]`修饰，表示做为请求路径或请求参数处理，`[PathQuery]`特性可以设置`Encoding`、`IgnoreWhenNull`和`DateTimeFormat`多个属性。

### 2.4.5 使用`[Parameter(Kind.Query)]`特性

```csharp
// GET /webapi/user?account=laojiu&password=123456&role=admin
[HttpGet("webapi/user")]
ITask<UserInfo> GetUserAsync(
    [Parameter(Kind.Query)]LoginInfo loginInfo,
    [Parameter(Kind.Query)]string role);
```
