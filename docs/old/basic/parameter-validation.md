# 参数及参数属性输入验证

这些验证特性都有相同的基类ValidationAttribute，命名空间为System.ComponentModel.DataAnnotations，由netfx或corefx提供。

## 参数值的验证

```csharp
[HttpGet("webapi/user/GetById/{id}")]
ITask<HttpResponseMessage> GetByIdAsync(
    [Required, StringLength(10)] string id);
```

id的参数要求必填且最大长度为10的字符串，否则抛出ValidationException的异常。

## 参数的属性值验证

```csharp
public class UserInfo
{
    [Required]
    [StringLength(10, MinimumLength = 1)]
    public string Account { get; set; }

    [Required]
    [StringLength(10, MinimumLength = 6)]
    public string Password { get; set; }
}

[HttpPut("webapi/user/UpdateWithJson")]
ITask<UserInfo> UpdateWithJsonAsync(
    [JsonContent] UserInfo user);
```

当user参数不为null的情况，就会验证它的Account和Password两个属性，HttpApiConfig有个UseParameterPropertyValidate属性，设置为false就禁用验证参数的属性值。

## 两者同时验证

对于上节的例子，如果我们希望user参数值也不能为null，可以如下声明方法：

```csharp
[HttpPut("webapi/user/UpdateWithJson")]
ITask<UserInfo> UpdateWithJsonAsync(
    [Required, JsonContent] UserInfo user);
```
