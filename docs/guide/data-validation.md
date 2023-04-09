# 数据验证

## 参数值验证

对于参数值，支持ValidationAttribute特性修饰来验证值。

```csharp
public interface IUserApi
{
    [HttpGet("api/users/{email}")]
    Task<User> GetAsync([EmailAddress, Required] string email);
}
```

## 参数或返回模型属性验证

```csharp
public interface IUserApi
{
    [HttpPost("api/users")]
    Task<User> PostAsync([Required][XmlContent] User user);
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
