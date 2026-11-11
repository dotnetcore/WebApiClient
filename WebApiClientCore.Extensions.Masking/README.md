# WebApiClientCore.Extensions.Masking

为 WebApiClientCore 提供声明式的日志脱敏能力：在输出请求和响应日志之前，
按接口方法上声明的字段规则对 JSON 内容执行脱敏。

## 安装

```shell
dotnet add package WebApiClientCore.Extensions.Masking
```

## 使用

```csharp
using WebApiClientCore.Attributes;

[MaskedLoggingFilter]
public interface IUserApi : IHttpApi
{
    [HttpPost("user")]
    [MaskField("phone", StrategyName = "Phone")]
    [MaskField("email", StrategyName = "Email")]
    [MaskField("idCard", StrategyName = "IdCard")]
    [MaskField("password")]
    [MaskField("data.*.token", StrategyName = "Replace")]
    Task<UserInfo> GetAsync([JsonContent] UserInfo user, CancellationToken token = default);
}
```

- `[MaskedLoggingFilter]` 继承自 `LoggingFilterAttribute`，请求和响应日志的输出方式与原生一致
- `[MaskField]` 声明需要脱敏的字段，一个方法可以声明多个
- 未指定 `StrategyName` 时使用 `Replace` 策略
- 被脱敏的内容同时作用于请求和响应日志

字段路径规则：

| 声明 | 含义 |
| --- | --- |
| `phone` | 匹配任意层级的 `phone` 字段 |
| `user.email` | 精确匹配 `user` 下的 `email` 字段 |
| `data.*.phone` | `*` 匹配任意一段 |

数组不产生路径层级，数组元素的字段路径与数组字段一致。

## 内置策略

| 名称 | 说明 | 示例 |
| --- | --- | --- |
| `Replace` | 整体替换，默认替换为 `***` | `13812345678` → `***` |
| `Truncate` | 保留首尾，默认首 3 尾 4 | `abcdefghij` → `abc***ghij` |
| `Phone` | 手机号保留前 3 后 4 | `13812345678` → `138****5678` |
| `Email` | 保留邮箱首字符与域名 | `abc@test.com` → `a***@test.com` |
| `IdCard` | 身份证保留前 6 后 4 | `110101199001011234` → `110101********1234` |

## 注册与覆盖策略

默认使用内置策略。需要自定义参数或新增策略时，在服务注册阶段配置：

```csharp
using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore.Masking;
using WebApiClientCore.Masking.Strategies;

services.AddMasking(options =>
{
    // 覆盖内置的 Replace 策略
    options.Strategies.Register(new ReplaceMaskStrategy("[已脱敏]"));

    // 新增策略，供 [MaskField("remark", StrategyName = "ShortTruncate")] 使用
    options.Strategies.Register(new TruncateMaskStrategy(keepStart: 2, keepEnd: 2, name: "ShortTruncate"));
});
```

注册到服务容器后，过滤器会优先使用容器中的 `MaskStrategyRegistry`；未注册时使用全局默认实例。

## 自定义策略

```csharp
using WebApiClientCore.Masking;

public sealed class Base64MaskStrategy : IMaskStrategy
{
    public string Name => "Base64";

    public string Mask(string value)
    {
        return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(value));
    }
}

options.Strategies.Register(new Base64MaskStrategy());
```

## 直接脱敏 JSON

不经过过滤器时，也可以直接使用 `JsonMasker` 与策略：

```csharp
using WebApiClientCore.Masking;
using WebApiClientCore.Masking.Strategies;

var rules = new[]
{
    new MaskRule("phone", new PhoneMaskStrategy()),
    new MaskRule("data.*.token", new ReplaceMaskStrategy())
};

var masked = JsonMasker.Mask(json, rules);
```

## 注意事项

- 仅处理 JSON 内容；文本不是合法 JSON 时回退为按字段名的正则匹配
- 字段值在输出时统一按 JSON 字符串写入，不会影响业务反序列化（脱敏只作用于日志内容）
- 策略名称解析失败会抛出 `MaskStrategyNotFoundException`，以避免输出未脱敏的数据

## 目标框架

`netstandard2.1`、`net8.0`、`net10.0`
