# Json.NET 扩展

使用 WebApiClientCore.Extensions.NewtonsoftJson 扩展，轻松支持 Newtonsoft 的 `Json.NET` 来序列化和反序列化 json。

## 配置[可选]

```csharp
// ConfigureNewtonsoftJson
services.AddHttpApi<IUserApi>().ConfigureNewtonsoftJson(o =>
{
    o.JsonSerializeOptions.NullValueHandling = NullValueHandling.Ignore;
});
```

## 声明特性

使用[JsonNetReturn]替换内置的[JsonReturn]，[JsonNetContent]替换内置[JsonContent]

```csharp
/// <summary>
/// 用户操作接口
/// </summary>
[JsonNetReturn]
public interface IUserApi
{
    [HttpPost("/users")]
    Task PostAsync([JsonNetContent] User user);
}
```
