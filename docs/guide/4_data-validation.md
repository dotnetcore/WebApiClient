# 数据验证

使用 ValidationAttribute 的子类特性来验证请求参数值和响应结果。

## 参数值验证

```csharp
public interface IUserApi
{    
    [HttpGet("api/users/{email}")]
    Task<User> GetAsync(        
        [EmailAddress, Required] // 这些验证特性用于请求前验证此参数
        string email);
}
```

## 请求或响应模型验证

请求和相应用到的 User 的两个属性值都得到验证。

```csharp
public interface IUserApi
{
    [HttpPost("api/users")]
    Task<User> PostAsync([Required][JsonContent] User user);
}

public class User
{
    [Required]
    [StringLength(10, MinimumLength = 1)]
    public string Account { get; set; }

    [Required]
    [StringLength(10, MinimumLength = 1)]
    public string Password { get; set; }
}
```

## 关闭数据验证功能

数据验证功能默认是开启的，可以在接口的 HttpApiOptions 配置关闭数据验证功能。

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpApi<IUserApi>().ConfigureHttpApi(o =>
    {
        // 关闭数据验证功能，即使打了验证特性也不验证。
        o.UseParameterPropertyValidate = false;
        o.UseReturnValuePropertyValidate = false;
    }); 
}
```
