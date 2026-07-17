# WebApiClientCore 完整参考手册

> WebApiClientCore 是一个基于 .NET Standard 2.1+ 的高性能、高可扩展性的声明式 HTTP 客户端库，通过接口和特性来定义 HTTP 请求。

---

## 1. 项目概述

### 1.1 简介
- **GitHub**: https://github.com/dotnetcore/WebApiClient
- **NuGet**: `WebApiClientCore` (基础包)
- **版本**: 2.x
- **目标**: .NET Standard 2.1+ / .NET Core 3.1+ (AOT 需要 .NET 8+)
- **许可证**: MIT

### 1.2 NuGet 包

| 包名 | 描述 |
|------|------|
| `WebApiClientCore` | 基础包 |
| `WebApiClientCore.Extensions.OAuths` | OAuth2 与 Token 管理 |
| `WebApiClientCore.Extensions.NewtonsoftJson` | Newtonsoft.Json 支持 |
| `WebApiClientCore.Extensions.JsonRpc` | JSON-RPC 协议支持 |
| `WebApiClientCore.OpenApi.SourceGenerator` | OpenApi 代码生成 dotnet tool |

### 1.3 核心概念架构

```
Your Interface (声明式接口)
    ↓
HttpApiProxy (运行时/编译时生成代理类)
    ↓
HttpClient (底层传输 - HttpClientFactory)
```

**核心组件**:
- **特性系统**: HTTP 方法/内容/返回/过滤器特性
- **IApiParameter**: 自解释参数类型
- **IApiFilter**: 请求/响应拦截过滤器
- **ITask\<T\>**: 支持条件重试的异步任务
- **依赖注入**: 与 ASP.NET Core DI 深度集成

---

## 2. 快速开始

### 2.1 环境要求
- .NET Standard 2.1+ / .NET Core 3.1+
- 具备依赖注入环境 (ASP.NET Core / 控制台应用)

### 2.2 声明接口

```csharp
using WebApiClientCore;
using WebApiClientCore.Attributes;
using WebApiClientCore.Parameters;

[LoggingFilter]
[HttpHost("http://localhost:5000/")]
public interface IUserApi
{
    [HttpGet("api/users/{id}")]
    Task<User> GetAsync(string id, CancellationToken token = default);

    [HttpPost("api/users")]
    Task<User> PostJsonAsync([JsonContent] User user, CancellationToken token = default);

    [HttpPost("api/users")]
    Task<User> PostXmlAsync([XmlContent] User user, CancellationToken token = default);

    [HttpPost("api/users")]
    Task<User> PostFormAsync([FormContent] User user, CancellationToken token = default);

    [HttpPost("api/users")]
    Task<User> PostFormDataAsync([FormDataContent] User user, FormDataFile avatar, CancellationToken token = default);
}
```

### 2.3 注册服务

**ASP.NET Core**:
```csharp
using Microsoft.Extensions.DependencyInjection;

services.AddHttpApi<IUserApi>().ConfigureHttpApi(o =>
{
    o.HttpHost = new Uri("http://localhost:5000/");
});
```

**控制台应用**:
```csharp
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddHttpApi<IUserApi>().ConfigureHttpApi(o =>
{
    o.HttpHost = new Uri("http://localhost:5000/");
});
var provider = services.BuildServiceProvider();
var userApi = provider.GetRequiredService<IUserApi>();
```

### 2.4 使用接口

```csharp
using System.Net.Http;

public class UserService
{
    private readonly IUserApi _userApi;
    public UserService(IUserApi userApi) => _userApi = userApi;

    public async Task<User?> GetUserAsync(string id)
    {
        try { return await _userApi.GetAsync(id); }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"请求失败: {ex.Message}");
            return null;
        }
    }
}
```

---

## 3. HTTP 特性 (Http Attributes)

### 3.1 执行顺序

**请求前**: 参数值验证 → IApiActionAttribute → IApiParameterAttribute → IApiReturnAttribute → IApiFilterAttribute

**响应后**: IApiReturnAttribute → 返回值验证 → IApiFilterAttribute

### 3.2 特性位置

```csharp
using System.Net.Http;
using WebApiClientCore;
using WebApiClientCore.Attributes;

[IApiFilterAttribute]          // 接口级别 Filter
[IApiReturnAttribute]          // 接口级别 Return
public interface IApi
{
    [IApiActionAttribute]       // 方法级别 Action
    [IApiFilterAttribute]       // 方法级别 Filter
    [IApiReturnAttribute]       // 方法级别 Return
    Task<HttpResponseMessage> Method([IApiParameterAttribute] Param param);
}
```

### 3.3 HTTP 方法特性

| 特性 | 用途 | 路径参数支持 |
|------|------|:---:|
| `[HttpHost("url")]` | 接口基础地址 | - |
| `[HttpGet("path")]` | GET 请求 | ✅ `{id}` |
| `[HttpPost("path")]` | POST 请求 | ✅ |
| `[HttpPut("path")]` | PUT 请求 | ✅ |
| `[HttpDelete("path")]` | DELETE 请求 | ✅ |
| `[HttpPatch("path")]` | PATCH 请求 | ✅ |
| `[HttpHead("path")]` | HEAD 请求 | ✅ |
| `[HttpOptions("path")]` | OPTIONS 请求 | ✅ |

**HttpHost 示例**:
```csharp
using WebApiClientCore.Attributes;

[HttpHost("http://localhost:5000/")]       // 接口级别
public interface IApi
{
    [HttpHost("http://localhost:8000/")]   // 覆盖接口级别
    Task<User> PostAsync(User user);
}
```

### 3.4 请求头特性

| 特性 | 用法 |
|------|------|
| `[Header("name", "value")]` | 常量值请求头 |
| `[Header]` (参数) | 参数值作为请求头 |
| `[Headers]` (参数) | 参数对象的键值对作为请求头 |
| `[BasicAuth("user","pwd")]` | Basic 授权 |

```csharp
using WebApiClientCore.Attributes;

public interface IApi
{
    [Header("X-Version", "1.0")]
    [HttpGet("api/users/{id}")]
    Task<User> GetAsync(string id, [Header] string token);
}
```

### 3.5 其他 HTTP 特性

| 特性 | 说明 |
|------|------|
| `[Timeout(ms)]` | 超时 (常量或参数) |
| `[Cache(ms)]` | 响应缓存，`IncludeHeaders` 指定缓存键包含的请求头 |
| `[Uri]` | 参数值作为请求 URI (必须是第一个参数) |
| `[PathQuery]` | 路径/查询参数 (缺省时对普通参数隐性生效) |
| `[HttpCompletionOption(...)]` | 响应读取行为控制 |
| `[AliasAs("name")]` | 参数别名 (解决 C# 不允许的命名) |

---

## 4. 内容特性 (Content Attributes)

### 4.1 请求体特性

| 特性 | Content-Type | 说明 |
|------|:---:|------|
| `[JsonContent]` | `application/json` | JSON 请求体。属性: `CharSet`, `AllowChunked` |
| `[XmlContent]` | `application/xml` | XML 请求体。属性: `CharSet` |
| `[FormContent]` | `application/x-www-form-urlencoded` | 表单。属性: `CollectionFormat` (默认 Multi) |
| `[FormDataContent]` | `multipart/form-data` | 文件/表单混合。属性: `CollectionFormat` |
| `[RawStringContent("text/plain")]` | 自定义 | 原始文本。属性: `CharSet` |
| `[RawJsonContent]` | `application/json` | 原始 JSON 字符串 |
| `[RawXmlContent]` | `application/xml` | 原始 XML 字符串 |
| `[RawFormContent]` | `application/x-www-form-urlencoded` | 原始表单编码内容 |

### 4.2 表单字段附加特性

| 特性 | 用途 |
|------|------|
| `[FormField("name","value")]` | 常量或参数的 x-www-form-urlencoded 字段 |
| `[FormDataText("name","value")]` | 常量或参数的 multipart 文本字段 |
| `[JsonFormField]` | 参数序列化为 JSON 字符串作为表单字段 |
| `[JsonFormDataText]` | 参数序列化为 JSON 字符串作为 multipart 字段 |

### 4.3 表单元数据

```
对于 id = ["001","002"]:
- CollectionFormat.Csv  →  id=001,002
- CollectionFormat.Ssv  →  id=001 002
- CollectionFormat.Tsv  →  id=001\002
- CollectionFormat.Pipes →  id=001|002
- CollectionFormat.Multi →  id=001&id=002
```

### 4.4 UseJsonFirstApiActionDescriptor

自动为非 GET/HEAD 请求的复杂参数应用 `[JsonContent]`，减少重复标注：

```csharp
using Microsoft.Extensions.DependencyInjection;

services.AddWebApiClient().UseJsonFirstApiActionDescriptor();
```

---

## 5. 返回特性 (Return Attributes)

### 5.1 规则
- `EnsureMatchAcceptContentType` (默认 false): 为 true 时要求 Content-Type 匹配才生效
- `EnsureSuccessStatusCode` (默认 true): 为 true 时非 2xx 状态码抛出 `ApiResponseStatusException`
- 多个 Return 特性场景: `AcceptQuality` 最大的生效；都不匹配时抛出 `ApiReturnNotSupportedException`

### 5.2 内置返回特性

| 特性 | 说明 |
|------|------|
| `[JsonReturn]` | 使用 System.Text.Json 反序列化 JSON 响应 |
| `[XmlReturn]` | 使用 System.Xml.Serialization 反序列化 XML 响应 |
| `[RawReturn]` | 原始类型返回 (string/byte[]/Stream/HttpResponseMessage) |
| `[NoneReturn]` | 204 或空响应时返回默认值 |

### 5.3 默认隐式特性

每个接口方法默认有多个 AcceptQuality=0.1 的 Return 特性:
```csharp
using WebApiClientCore.Attributes;

// 等效于声明了:
[RawReturn(0.1)]
[NoneReturn(0.1)]
[JsonReturn(0.1, EnsureMatchAcceptContentType = true)]
[XmlReturn(0.1, EnsureMatchAcceptContentType = true)]
```

### 5.4 处理非标准 Content-Type

```csharp
using WebApiClientCore.Attributes;

[JsonReturn(EnsureMatchAcceptContentType = false)]  // 强制处理
public interface IUserApi { }
```

### 5.5 自定义返回特性

```csharp
using System.Text.Json;
using WebApiClientCore;
using WebApiClientCore.Attributes;

public class CustomJsonReturnAttribute : JsonReturnAttribute
{
    protected override JsonSerializerOptions GetSerializerOptions(ApiResponseContext context)
    {
        var options = base.GetSerializerOptions(context);
        options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        return options;
    }
}
```

---

## 6. 过滤器特性 (Filter Attributes)

### 6.1 内置过滤器

**LoggingFilter**:
```csharp
using WebApiClientCore.Attributes;

[LoggingFilter]
public interface IUserApi
{
    [LoggingFilter(Enable = false)]  // 禁用日志
    [HttpPost("api/users")]
    Task<User> PostAsync([JsonContent] User user);
}
```

**Cache**:
```csharp
using WebApiClientCore.Attributes;

[Cache(60 * 1000)]                            // 缓存 60 秒
[Cache(60 * 1000, MaxStatusCode = 200)]       // 只缓存状态码 200 的响应
[Cache(60 * 1000, IncludeHeaders = "Authorization")]  // 缓存键包含请求头
```

### 6.2 自定义过滤器

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore;
using WebApiClientCore.Attributes;

public class SignFilterAttribute : ApiFilterAttribute
{
    public override Task OnRequestAsync(ApiRequestContext context)
    {
        var signService = context.HttpContext.ServiceProvider.GetRequiredService<SignService>();
        var sign = signService.SignValue(DateTime.Now);
        context.HttpContext.RequestMessage.AddUrlQuery("sign", sign);
        return Task.CompletedTask;
    }
}
```

### 6.3 过滤器执行顺序

1. 接口级别 `GlobalFilters` (按添加顺序)
2. 接口声明的 `IApiFilterAttribute` 特性
3. 方法声明的 `IApiFilterAttribute` 特性

---

## 7. 全局过滤器

### 7.1 注册全局过滤器

```csharp
using Microsoft.Extensions.DependencyInjection;

services.AddHttpApi<IUserApi>().ConfigureHttpApi(o =>
{
    o.GlobalFilters.Add(new UserFilter());
});
```

### 7.2 实现 IApiFilter

```csharp
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore;

public class UserFilter : IApiFilter
{
    public Task OnRequestAsync(ApiRequestContext context)
    {
        var userId = context.HttpContext.ServiceProvider
            .GetRequiredService<IUserContext>().UserId;
        context.HttpContext.RequestMessage.Headers.Add("X-User-Id", userId);
        return Task.CompletedTask;
    }

    public Task OnResponseAsync(ApiResponseContext context)
    {
        return Task.CompletedTask;
    }
}
```

### 7.3 依赖注入支持

```csharp
using Microsoft.Extensions.DependencyInjection;

services.AddHttpApi<IUserApi>().ConfigureHttpApi((o, sp) =>
{
    o.GlobalFilters.Add(sp.GetRequiredService<SignFilter>());
});
```

---

## 8. 特殊参数类型

### 8.1 无需特性标注的参数

| 类型 | 说明 |
|------|------|
| `CancellationToken` | 请求取消 (推荐默认值 `default`) |
| `FileInfo` | multipart 文件上传 |
| `HttpContent` / `StringContent` / `StreamContent` / `ByteArrayContent` | 原始 HTTP 内容 |
| `FormDataFile` | multipart 文件项 (等效 FileInfo) |
| `JsonPatchDocument<T>` | JSON Patch 请求文档 |

### 8.2 IApiParameter 自解释参数

```csharp
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Parameters;
using WebApiClientCore.HttpContents;

public class FaceModel : IApiParameter
{
    public Bitmap Image1 { get; set; }
    public Bitmap Image2 { get; set; }

    public Task OnRequestAsync(ApiParameterContext context)
    {
        var model = new {
            image1 = GetImageBase64(Image1),
            image2 = GetImageBase64(Image2)
        };
        var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;
        context.HttpContext.RequestMessage.Content = new JsonContent(model, options);
        return Task.CompletedTask;
    }
}
```

---

## 9. 异常处理

### 9.1 异常体系

```
Exception
└── ApiException (抽象基类)
    ├── ApiInvalidConfigException      // 配置无效
    ├── ApiResponseStatusException     // 非 2xx 状态码
    ├── ApiResultNotMatchException     // 重试结果验证失败
    ├── ApiRetryException              // 重试次数耗尽
    └── ApiReturnNotSupportedException // 不支持的 Content-Type
```

### 9.2 异常封装

所有请求异常被封装为 `HttpRequestException`，`InnerException` 为实际异常：

```csharp
using System.Net.Http;
using WebApiClientCore.Exceptions;

catch (HttpRequestException ex) when (ex.InnerException is ApiResponseStatusException statusEx)
{
    var code = (int)statusEx.StatusCode;
    Console.WriteLine($"HTTP {code}: {await statusEx.ResponseMessage.Content.ReadAsStringAsync()}");
}
```

### 9.3 异常处理速查

| 异常 | 触发阶段 | 原因 | 处理 |
|------|---------|------|------|
| `ApiInvalidConfigException` | 请求前 | 配置错误 | 开发期修复 |
| `ApiResponseStatusException` | 响应后 | 非 2xx 状态码 | 按状态码分类处理 |
| `ApiRetryException` | 重试后 | 重试耗尽 | 提示稍后重试 |
| `ApiReturnNotSupportedException` | 响应解析 | Content-Type 不匹配 | 添加对应处理器 |

### 9.4 关闭数据验证

```csharp
using Microsoft.Extensions.DependencyInjection;

services.AddHttpApi<IUserApi>().ConfigureHttpApi(o =>
{
    o.UseParameterPropertyValidate = false;
    o.UseReturnValuePropertyValidate = false;
});
```

---

## 10. ITask 与重试

### 10.1 ITask vs Task

| 场景 | 推荐类型 |
|------|----------|
| 简单请求，无需重试 | `Task<TResult>` |
| 需要条件重试 | `ITask<TResult>` |
| 异常处理返回默认值 | `ITask<TResult>` |

### 10.2 Retry 方法

```csharp
using WebApiClientCore;

// 基本重试
ITask<T> task = api.GetAsync(id);
await task.Retry(3);

// 固定延时
await api.GetAsync(id).Retry(3, TimeSpan.FromSeconds(1));

// 动态延时 (指数退避)
await api.GetAsync(id).Retry(3, i => TimeSpan.FromSeconds(Math.Pow(2, i)));
```

### 10.3 WhenCatch 异常捕获

```csharp
using System.Net.Http;
using WebApiClientCore;

await api.GetAsync(id)
    .Retry(3)
    .WhenCatch<HttpRequestException>()                               // 捕获并重试
    .WhenCatch<HttpRequestException>(ex => logger.LogWarning(ex))    // 捕获 + 记录日志
    .WhenCatch<HttpRequestException>(ex => ex.InnerException is SocketException);  // 条件判断
```

### 10.4 WhenResult 结果条件重试

```csharp
using WebApiClientCore;

await api.GetAsync(id)
    .Retry(3)
    .WhenResult(r => r.Success == false)
    .WhenResultAsync(async r => await cacheService.IsStaleAsync(r.Version));
```

### 10.5 Handle 异常处理

```csharp
using WebApiClientCore;

var user = await api.GetAsync(id).HandleAsDefaultWhenException();

var result = await api.GetAsync(id)
    .Handle()
    .WhenCatch<HttpRequestException>(() => new User { Id = "error" });
```

### 10.6 最佳实践

- 只重试可恢复的异常 (网络/超时)，不重试所有 `Exception`
- `WhenResult` 中避免耗时操作
- HttpClient.Timeout 应大于 重试总时间 = (请求超时 + 重试延时) × 重试次数
- 全局重试建议使用 Polly Resilience 机制

---

## 11. HttpClient 与 HttpMessageHandler 配置

### 11.1 HttpClient 配置

```csharp
using System.Net;
using Microsoft.Extensions.DependencyInjection;

services.AddHttpApi<IUserApi>().ConfigureHttpClient(httpClient =>
{
    httpClient.Timeout = TimeSpan.FromMinutes(1);
    httpClient.DefaultRequestVersion = HttpVersion.Version20;
    httpClient.DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;
    httpClient.DefaultRequestHeaders.Add("X-Custom-Header", "value");
});
```

### 11.2 HttpMessageHandler 配置

**代理**:
```csharp
using System.Net;

.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    UseProxy = true,
    Proxy = new WebProxy("http://proxy.com") { Credentials = new NetworkCredential("user", "pwd") }
});
```

**客户端证书**:
```csharp
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.ClientCertificates.Add(yourCert);
    return handler;
});
```

**维持 CookieContainer**:
```csharp
var cookieContainer = new CookieContainer();
.ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    handler.CookieContainer = cookieContainer;
    return handler;
});
```

**自定义 DelegatingHandler**:
```csharp
using Microsoft.Extensions.DependencyInjection;

services.AddHttpApi<IUserApi>()
    .AddHttpMessageHandler(() => new CustomDelegatingHandler());
```

---

## 12. 动态 HttpHost

### 12.1 UriAttribute 传绝对 Uri

```csharp
using WebApiClientCore;
using WebApiClientCore.Attributes;

[HttpGet]
ITask<User> GetAsync([Uri] string urlString, [PathQuery] string id);
```

### 12.2 自定义 HttpHostBaseAttribute

```csharp
using System;
using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore;
using WebApiClientCore.Attributes;

[ServiceNameHost("baidu")]
public interface IUserApi { ... }

public class ServiceNameHostAttribute : HttpHostBaseAttribute
{
    public override Task OnRequestAsync(ApiRequestContext context)
    {
        var host = context.HttpContext.ServiceProvider
            .GetRequiredService<HostProvider>().ResolveHost(this.ServiceName);
        context.HttpContext.RequestMessage.RequestUri = new Uri(host);
        return Task.CompletedTask;
    }
}
```

### 12.3 Uri 拼接规则

```
baseUri 以 / 结尾:  http://a.com/path1/ + b/c/d = http://a.com/path1/b/c/d
baseUri 不以 / 结尾: http://a.com/path1 + b/c/d = http://a.com/b/c/d  (path1 被替换)
最佳实践: 始终以 / 结尾: [HttpHost("http://api.example.com/")]
```

---

## 13. 数据验证

```csharp
using System.ComponentModel.DataAnnotations;
using WebApiClientCore.Attributes;

public interface IUserApi
{
    [HttpGet("api/users/{email}")]
    Task<User> GetAsync([EmailAddress, Required] string email);

    [HttpPost("api/users")]
    Task<User> PostAsync([Required][JsonContent] User user);
}

public class User
{
    [Required]
    [StringLength(10, MinimumLength = 1)]
    public string Account { get; set; }
}
```

---

## 14. 适配畸形 API

### 14.1 参数别名
```csharp
using WebApiClientCore.Attributes;

Task<string> GetAsync([AliasAs("field-Name")] string fieldName);
```

### 14.2 Form 字段嵌套 (JsonFormField)
```csharp
using WebApiClientCore.Attributes;

Task PostAsync([FormField] string field1, [JsonFormField] Field2 field2);
```

### 14.3 Form 多层嵌套 (KeyValueSerializeOptions)
```csharp
o.KeyValueSerializeOptions.KeyNamingStyle = KeyNamingStyle.FullName;
```

### 14.4 Content-Type 不匹配
```csharp
using WebApiClientCore.Attributes;

[JsonReturn(EnsureMatchAcceptContentType = false)]
```

---

## 15. 自定义内容与扩展

### 15.1 自定义请求内容

```csharp
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

public class ProtobufContentAttribute : HttpContentAttribute
{
    protected override Task SetHttpContentAsync(ApiParameterContext context)
    {
        var stream = new MemoryStream();
        Serializer.NonGeneric.Serialize(stream, context.ParameterValue);
        stream.Position = 0;
        var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
        context.HttpContext.RequestMessage.Content = content;
        return Task.CompletedTask;
    }
}
```

### 15.2 自定义响应解析

```csharp
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WebApiClientCore;
using WebApiClientCore.Attributes;

public class ProtobufReturnAttribute : ApiReturnAttribute
{
    public ProtobufReturnAttribute()
        : base(new MediaTypeWithQualityHeaderValue("application/x-protobuf")) { }

    public override async Task SetResultAsync(ApiResponseContext context)
    {
        var stream = await context.HttpContext.ResponseMessage.Content.ReadAsStreamAsync();
        context.Result = Serializer.NonGeneric.Deserialize(
            context.ApiAction.Return.DataType.Type, stream);
    }
}
```

### 15.3 自定义日志

```csharp
using WebApiClientCore;
using WebApiClientCore.Attributes;

public class DatabaseLoggingAttribute : LoggingFilterAttribute
{
    protected override async Task WriteLogAsync(ApiResponseContext context, LogMessage logMessage)
    {
        // 输出到数据库
    }
}
```

### 15.4 自定义缓存提供者

```csharp
using System;
using System.Threading.Tasks;
using WebApiClientCore;

public class RedisResponseCacheProvider : IResponseCacheProvider
{
    public async Task<ResponseCacheResult> GetAsync(string key) { ... }
    public async Task SetAsync(string key, ResponseCacheEntry entry, TimeSpan expiration) { ... }
}

services.AddWebApiClient().UseRedisResponseCacheProvider();
```

---

## 16. AOT 发布

### 16.1 AOT 优势

| 优势 | 说明 |
|------|------|
| **启动速度快** | 无需 JIT 编译，应用启动时间大幅缩短 |
| **内存占用低** | 去除了 JIT 编译器和 IL 代码，减少内存使用 |
| **部署体积小** | 只包含必要的运行时代码，生成单一可执行文件 |
| **无需 .NET 运行时** | 目标机器不需要安装 .NET Runtime |
| **更好的安全性** | 原生代码比 IL 更难逆向工程 |

### 16.2 WebApiClientCore AOT 支持原理

传统模式依赖运行时反射创建接口代理类，AOT 环境不可行。WebApiClientCore 通过 **Source Generator** 在编译时生成代理类：

```
传统模式：
┌─────────────────┐     反射      ┌──────────────────┐
│   IHttpApi 接口  │ ──────────→  │   运行时代理类    │
└─────────────────┘              └──────────────────┘

AOT 模式：
┌─────────────────┐   Source     ┌──────────────────┐
│   IHttpApi 接口  │ ──────────→  │   编译时代理类    │
└─────────────────┘   Generator  └──────────────────┘
```

### 16.3 Source Generator 工作机制

WebApiClientCore 的 Source Generator 在编译时：
1. **扫描接口** - 查找所有继承自 `IHttpApi` 的接口
2. **生成代理类** - 为每个接口生成一个实现类
3. **注册初始化器** - 使用 `[ModuleInitializer]` 自动注册代理类类型

对于以下接口：
```csharp
using WebApiClientCore;
using WebApiClientCore.Attributes;

public interface IUserApi : IHttpApi
{
    [HttpGet("api/users/{id}")]
    Task<User> GetAsync(string id);
}
```

Source Generator 生成类似以下代理代码：
```csharp
using WebApiClientCore;

partial class HttpApiProxyClass
{
    [HttpApiProxyClass(typeof(IUserApi))]
    sealed partial class IUserApi : IUserApi
    {
        private readonly IHttpApiInterceptor _apiInterceptor;
        private readonly ApiActionInvoker[] _actionInvokers;

        [HttpApiProxyMethod(0, "GetAsync", typeof(IUserApi))]
        Task<User> IUserApi.GetAsync(string p0)
        {
            return (Task<User>)_apiInterceptor.Intercept(_actionInvokers[0], new object[] { p0 });
        }
    }
}
```

### 16.4 项目配置

**前置要求**：
- **AOT 发布**：需要 .NET 8.0 或更高版本
- **JSON 源生成器**：`PrependJsonSerializerContext` 方法仅支持 .NET 8.0+
- **接口必须继承 `IHttpApi`**：Source Generator 以此扫描接口

**完整 .csproj 示例**：
```xml
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net8.0</TargetFramework>
        <Nullable>enable</Nullable>
        <PublishAot>true</PublishAot>
        <PublishTrimmed>true</PublishTrimmed>
        <InvariantGlobalization>true</InvariantGlobalization>
    </PropertyGroup>

    <ItemGroup>
        <PackageReference Include="Microsoft.Extensions.Hosting" Version="8.0.0" />
    </ItemGroup>

    <ItemGroup>
        <PackageReference Include="WebApiClientCore" Version="2.*" />
    </ItemGroup>
</Project>
```

### 16.5 JSON 源生成器

AOT 环境下，`System.Text.Json` 也必须使用源生成器。必须声明**所有**接口中使用的 JSON 类型，包括集合和泛型：

```csharp
using System.Text.Json.Serialization;

[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(User[]))]
[JsonSerializable(typeof(List<User>))]
[JsonSerializable(typeof(ApiResponse<User>))]
[JsonSerializable(typeof(ApiResponse<List<User>>))]
public partial class AppJsonSerializerContext : JsonSerializerContext { }
```

### 16.6 DI 注册

```csharp
using Microsoft.Extensions.DependencyInjection;

services
    .AddWebApiClient()
    .ConfigureHttpApi(options =>
    {
        options.PrependJsonSerializerContext(AppJsonSerializerContext.Default);
    });
services.AddHttpApi<IUserApi>();
```

### 16.7 发布命令

```bash
# 基本发布
dotnet publish -c Release

# 指定目标平台
dotnet publish -c Release -r win-x64
dotnet publish -c Release -r linux-x64
dotnet publish -c Release -r osx-x64

# 完整发布命令（优化体积）
dotnet publish -c Release -r linux-x64 \
    -p:PublishAot=true \
    -p:PublishTrimmed=true \
    -p:InvariantGlobalization=true \
    -p:StripSymbols=true \
    -p:OptimizationPreference=Speed
```

### 16.8 常见问题

| 问题 | 原因 | 解决方案 |
|------|------|----------|
| **找不到代理类** | Source Generator 未正确生成 | 确保接口继承 `IHttpApi`，`dotnet clean && dotnet build` 重新生成 |
| **JSON 序列化失败** | 未声明所需类型 | 在 `JsonSerializerContext` 中添加缺失类型，集合类型需单独声明 |
| **AOT 裁剪警告** | 代码使用了不兼容 AOT 的模式 | 使用 `[DynamicallyAccessedMembers]` 标注，检查动态代码生成 |
| **类型被裁剪** | 裁剪器认为类型未使用 | 使用 `[DynamicDependency]` 或 `[DynamicallyAccessedMembers]` 保持依赖 |
| **发布体积过大** | 包含不必要依赖或调试信息 | 启用 `InvariantGlobalization`、`StripSymbols`、`OptimizationPreference=Size` |

### 16.9 AOT 限制

| 限制 | 说明 | 替代方案 |
|------|------|----------|
| 接口必须继承 `IHttpApi` | Source Generator 以此为扫描条件 | — |
| 不支持动态接口 | 所有接口必须在编译时定义 | 静态引用所有依赖 |
| 不支持动态特性修改 | 特性配置必须在编译时确定 | — |
| JSON 必须源生成 | 必须使用 System.Text.Json 源生成器 | 不可替代 |
| 不支持 Newtonsoft.Json | `WebApiClientCore.Extensions.NewtonsoftJson` 不兼容 AOT | 使用 System.Text.Json |
| 有限反射 | 部分反射操作受限 | 使用源生成或标注 |

### 16.10 最佳实践

1. **开发阶段** - 使用 `dotnet run` 测试，AOT 发布前验证所有 API 调用
2. **发布前检查** - 检查编译警告（ILTrim/ILC），测试发布后应用功能完整性，验证 JSON 序列化
3. **调试技巧** - 使用 `<PublishAot>false</PublishAot>` 临时禁用 AOT 调试，检查 `obj/Release/netX.X/generated/` 下的生成代码
4. **裁剪保留** - 必要时使用 `<TrimmerRootAssembly Include="..." />` 保留特定程序集
