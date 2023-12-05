# 5、参数及属性注解

这些注解特性的命名空间在WebApiClient.DataAnnotations，用于影响参数的序列化行为。

## 5.1 参数别名

```csharp
public interface IMyWebApi : IHttpApi
{
    // GET <http://www.mywebapi.com/webapi/user?_name=laojiu>
    [HttpGet("http://www.mywebapi.com/webapi/user")]
    ITask<string> GetUserByAccountAsync(
        [AliasAs("_name")] string account);
}
```

## 5.2 参数模型属性注解

```csharp
public class UserInfo
{
    public string Account { get; set; }

    // 别名
    [AliasAs("a_password")]
    public string Password { get; set; }

    // 时间格式，优先级最高
    [DateTimeFormat("yyyy-MM-dd")]
    [IgnoreWhenNull] // 值为null则忽略序列化
    public DateTime? BirthDay { get; set; }
    
    // 忽略序列化
    [IgnoreSerialized]
    public string Email { get; set; } 
    
    // 时间格式
    [DateTimeFormat("yyyy-MM-dd HH:mm:ss")]
    public DateTime CreateTime { get; set; }
}
```
