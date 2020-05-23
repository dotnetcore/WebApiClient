## WebApiClientCore 　　　　　　　　　　　　　　　　　　　
[WebApiClient.JIT](https://github.com/dotnetcore/WebApiClient/tree/WebApiClient.JITAOT)的`.netcore`版本，基于`HttpClient`集高性能高可扩展性于一体的声明式http客户端库，特别适用于微服务的restful资源请求，也适用于各种非标准的http接口请求。

### PackageReference

    <PackageReference Include="WebApiClientCore" Version="1.0.0-beta*" />
 

### Benchmark
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18362.778 (1903/May2019Update/19H1)
Intel Core i3-4150 CPU 3.50GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.1.202
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT


|                     Method |       Mean |      Error |     StdDev |
|--------------------------- |-----------:|-----------:|-----------:|
|      WebApiClient_GetAsync | 279.479 us | 22.5466 us | 64.3268 us |
|  WebApiClientCore_GetAsync |  25.298 us |  0.4953 us |  0.7999 us |
|        HttpClient_GetAsync |   2.849 us |  0.0568 us |  0.1393 us |
|     WebApiClient_PostAsync |  25.942 us |  0.3817 us |  0.3188 us |
| WebApiClientCore_PostAsync |  13.462 us |  0.2551 us |  0.6258 us |
|       HttpClient_PostAsync |   4.515 us |  0.0866 us |  0.0926 us |





### 声明式接口定义
* 支持`Task`、`Task<>`和`ITask<>`三种异步返回
* 支持模型自动转换为`Xml`、`Json`、`Form`、和`FormData`共4种请求格式的内容
* 支持`HttpResponseMessage`、`byte[]`、`string`和`Stream`原生类型返回内容
* 支持原生`HttpContent`(比如`StringContent`)类型直接做为请求参数
* 内置丰富的能满足各种环境的常用特性(`ActionAttribute`和`ParameterAttribute`)
* 内置常用的`FormDataFile`等参数类型，同时支持自定义`IApiParameter`参数类型作为参数值
* 支持用户自定义`IApiActionAttribute`、`IApiParameterAttribue`、`IApiReturnAttribute`和`IApiFilterAttribute`

#### 1 Petstore接口例子
这个OpenApi文档在[petstore.swagger.io](https://petstore.swagger.io/)，代码为使用`WebApiClientCore.OpenApi.SourceGenerator`工具将其OpenApi文档反向生成得到

```
namespace Petstore
{
    /// <summary>
    /// Everything about your Pets
    /// </summary>
    [LoggingFilter]
    [HttpHost("https://petstore.swagger.io/v2/")]
    public interface IPetApi : IHttpApi
    {
        /// <summary>
        /// Add a new pet to the store
        /// </summary>
        /// <param name="body">Pet object that needs to be added to the store</param>
        [HttpPost("pet")]
        Task AddPetAsync([Required] [JsonContent] Pet body, CancellationToken token = default);

        /// <summary>
        /// Update an existing pet
        /// </summary>
        /// <param name="body">Pet object that needs to be added to the store</param>
        [HttpPut("pet")]
        Task<HttpResponseMessage> UpdatePetAsync([Required] [JsonContent] Pet body, CancellationToken token = default);

        /// <summary>
        /// Finds Pets by status
        /// </summary>
        /// <param name="status">Status values that need to be considered for filter</param>
        /// <returns>successful operation</returns>
        [HttpGet("pet/findByStatus")]
        ITask<List<Pet>> FindPetsByStatusAsync([Required] IEnumerable<Anonymous> status);

        /// <summary>
        /// Finds Pets by tags
        /// </summary>
        /// <param name="tags">Tags to filter by</param>
        /// <returns>successful operation</returns>
        [Obsolete]
        [HttpGet("pet/findByTags")]
        ITask<List<Pet>> FindPetsByTagsAsync([Required] [PathQuery] IEnumerable<string> tags);

        /// <summary>
        /// Find pet by ID
        /// </summary>
        /// <param name="petId">ID of pet to return</param>
        /// <returns>successful operation</returns>
        [HttpGet("pet/{petId}")]
        ITask<Pet> GetPetByIdAsync([Required] long petId);

        /// <summary>
        /// Updates a pet in the store with form data
        /// </summary>
        /// <param name="petId">ID of pet that needs to be updated</param>
        /// <param name="name">Updated name of the pet</param>
        /// <param name="status">Updated status of the pet</param>
        [HttpPost("pet/{petId}")]
        Task UpdatePetWithFormAsync([Required] long petId, [FormContent] string name, [FormContent] string status);

        /// <summary>
        /// Deletes a pet
        /// </summary>
        /// <param name="api_key"></param>
        /// <param name="petId">Pet id to delete</param>
        [HttpDelete("pet/{petId}")]
        Task DeletePetAsync([Header("api_key")] string api_key, [Required] long petId);

        /// <summary>
        /// uploads an image
        /// </summary>
        /// <param name="petId">ID of pet to update</param>
        /// <param name="additionalMetadata">Additional data to pass to server</param>
        /// <param name="file">file to upload</param>
        /// <returns>successful operation</returns>
        [LoggingFilter(Enable = false)]
        [HttpPost("pet/{petId}/uploadImage")]
        ITask<ApiResponse> UploadFileAsync([Required] long petId, [FormDataContent] string additionalMetadata, FormDataFile file);
    }
}
```
####  2 IOAuthClient接口例子
这个接口是在`WebApiClientCore.Extensions.OAuths.IOAuthClient.cs`代码中声明

```
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using WebApiClientCore.Attributes;

namespace WebApiClientCore.Extensions.OAuths
{
    /// <summary>
    /// 定义Token客户端的接口
    /// </summary>
    [LoggingFilter]
    [XmlReturn(Enable = false)]
    [JsonReturn(EnsureMatchAcceptContentType = false, EnsureSuccessStatusCode = false)]
    public interface IOAuthClient
    {
        /// <summary>
        /// 以client_credentials授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "client_credentials")]
        Task<TokenResult> RequestTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] ClientCredentials credentials);

        /// <summary>
        /// 以password授权方式获取token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "password")]
        Task<TokenResult> RequestTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] PasswordCredentials credentials);

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="endpoint">token请求地址</param>
        /// <param name="credentials">身份信息</param>
        /// <returns></returns>
        [HttpPost]
        [FormField("grant_type", "refresh_token")]
        Task<TokenResult> RefreshTokenAsync([Required, Uri] Uri endpoint, [Required, FormContent] RefreshTokenCredentials credentials);
    }
}
```
### 编译时语法分析
`WebApiClientCore.Analyzers`项目为`WebApiClientCore`提供编码时语法分析与提示。

比如`[Header]`特性，可以声明在Interface、Method和Parameter三个地方，但是必须使用正确的构造器，否则运行时会抛出异常。有了语法分析功能，在声明接口时就不会使用不当的语法。如果想让语法分析生效，你的接口必须继承空方法的`IHttpApi`接口。

```
/// <summary>
/// 你的接口，记得要实现IHttpApi
/// </summary>
public interface IYourApi : IHttpApi
{
    ...
}
```


### 服务注册与获取
#### 1 服务注册

```
var services = new ServiceCollection();

services.AddHttpApi<IPetApi>(o =>
{
    o.UseParameterPropertyValidate = true;
    o.UseReturnValuePropertyValidate = false;
    o.KeyValueSerializeOptions.IgnoreNullValues = true;
    o.HttpHost = new Uri("http://localhost:6000/");
});
```

#### 2 服务获取

```
public class MyService
{
    private readonly IpetApi petApi;
    
    // 构造器注入IpetApi
    public MyService(IpetApi petApi)
    {
        tihs.petApi = petApi;
    }
}
```

### `HttpApiOptions<THttpApi>`选项
每个接口的选项对应为`HttpApiOptions<THttpApi>`，除了Action配置，我们也可以使用Configuration配置结合一起使用，这部分内容为`Microsoft.Extensions.Options`范畴。

服务配置
```
services
    .ConfigureHttpApi<IpetApi>(Configuration.GetSection(nameof(IUserApi)))
    .ConfigureHttpApi<IpetApi>(o =>
    {
        o.JsonSerializeOptions.Converters.Add(new MyJsonConverter());
    });
```

appsettings.json的文件配置
```
{
  "IpetApi": {
    "HttpHost": "http://www.webappiclient.com/",
    "UseParameterPropertyValidate": false,
    "UseReturnValuePropertyValidate": false,
    "JsonSerializeOptions": {
      "IgnoreNullValues": true,
      "WriteIndented": false
    }
  }
}
```

### 请求和响应日志
在整个Interface或某个Method上声明`[LoggingFilter]`，即可把请求和响应的内容输出到`LoggingFactory`中。

如果要排除某个Method不打印日志（比如大流量传输接口），在该Method上声明`[LoggingFilter(Enable = false)]`，即可将本Method排除。

### Accpet ContentType
这个用于控制客户端希望服务器返回什么样的内容格式，比如json或xml，默认的配置值是`Accept: application/json; q=0.01, application/xml; q=0.01`

如果想json优先，可以在Interface或Method上声明`[JsonReturn]`，请求变为`Accept: application/json, application/xml; q=0.01`

如果想禁用其中一种，比如禁用xml，可以在Interface或Method上声明`[XmlReturn(Enable = false)]`，请求变为`Accept: application/json; q=0.01`


### 请求条件重试
使用ITask<>异步声明，就有Retry的扩展，Retry的条件可以为捕获到某种Exception或响应模型符合某种条件。

```
var result = await youApi.GetModelAsync(id: "id001")
    .Retry(maxCount: 3)
    .WhenCatch<Exception>()
    .WhenResult(r => r.ErrorCode > 0);
```

### 响应内容缓存
`ApiCacheAttribute`与`CacheAttribute`做为缓存应用的配置，配置了这个特性的Method将本次的响应内容缓存起来，下一次如果符合预期条件的话，就不会再请求到远程服务器，而是从`IResponseCacheProvider`获取缓存内容。你可以重写`CacheAttribute`或实现自定义`ResponseCacheProvider`来到达你自定义的要求。

```
public class RedisResponseCacheProvider : IResponseCacheProvider
{
    public string Name => nameof(RedisResponseCacheProvider);

    public Task<ResponseCacheResult> GetAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task SetAsync(string key, ResponseCacheEntry entry, TimeSpan expiration)
    {
        throw new NotImplementedException();
    }
}

// 注册RedisResponseCacheProvider
var services = new ServiceCollection();
services.AddSingleton<IResponseCacheProvider, RedisResponseCacheProvider>();
```



### Http代理
Http代理属于HttpMessageHandler层，所以应该在`Microsoft.Extensions.Http`的HttpClientBuilder里配置

```
services
    .AddHttpApi<IMyApi>(o =>
    {
        o.HttpHost = new Uri("http://localhost:6000/");
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        UseProxy = true,
        Proxy = new WebProxy
        {
            Address = new Uri("http://proxy.com"),
            Credentials = new NetworkCredential
            {
                UserName = "useranme",
                Password = "pasword"
            }
        }
    });
```

### 非强类型模型请求
有时候我们未必需要强模型，假设我们有原始的form文本内容，或原始的json文本内容，或者是System.Net.Http.HttpContent对象，只需要把这些原始内请求到远程远程器。

#### 1 原始文本
```
[HttpPost]
Task PostAsync([RawStringContent("txt/plain")] string text);

[HttpPost]
Task PostAsync(StringContent text);
```

#### 2 原始json
```
[HttpPost]
Task PostAsync([RawJsonContent] string json);
```

#### 3 原始xml
```
[HttpPost]
Task PostAsync([RawXmlContent] string json);
```

#### 4 原始表单内容
```
[HttpPost]
Task PostAsync([RawFormContent] string form);
```


### 自定义参数类型
在某些极限情况下，我们输入模型与传输模型未必是对等的。比如人脸比对的接口，其要求如下的json请求格式：

> 传输模型
```
{
    "image1" : "图片1的base64",
    "image2" : "图片2的base64"
}
```

而我们的最方便的业务模型是这样子：
```
class FaceModel
{
    public Bitmap Image1 {get; set;}
    public Bitmap Image2 {get; set;}
}
```

我们希望构造模型实例时传入Bitmap对象，但传输的时候变成Bitmap的base64值，所以我们要改造FaceModel，让它实现IApiParameter接口：

```
class FaceModel : IApiParameter
{
    public Bitmap Image1 { get; set; }

    public Bitmap Image2 { get; set; }


    public Task OnRequestAsync(ApiParameterContext context)
    {
        var image1 = GetImageBase64(this.Image1);
        var image2 = GetImageBase64(this.Image2);
        var model = new { image1, image2 };

        var options = context.HttpContext.Options.JsonSerializeOptions;
        var json = System.Text.Json.JsonSerializer.Serialize(model, options);
        context.HttpContext.RequestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");
        return Task.CompletedTask;
    }

    private static string GetImageBase64(Bitmap image)
    {
        using var stream = new MemoryStream();
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
        return Convert.ToBase64String(stream.ToArray());
    }
}
```

最后，我们在使用改进后的FaceModel来请求
```
public interface IFaceApi
{
    [HttpPost("/somePath")]
    Task<HttpResponseMessage> PostAsync(FaceModel faces);
}
```

### 适应非标准接口
在实际环境中，有些平台未能提供标准的接口，主要早期还没有restful概念时期的接口，形形色色的各种，我们要区别对待。

#### 1 参数别名`[Alias]`
例如服务器要求一个Query参数的名字为`field-Name`，这个是c#关键字或变量命名不允许的，我们可以使用`[AliasAsAttribute]`来达到这个要求：

```
[HttpGet("api/users/{account}")]
ITask<HttpResponseMessage> GetAsync([Required]string account, [AliasAs("field-Name")] string fieldName);
```

然后最终请求uri变为api/users/`account1`?field-name=`fileName1`

#### 2 Form的某个字段为json描述的实体

字段 | 值
---|---
field1 | abc
field2 | {"name":"jName","data":{"data1":"jData1"}}

对应强类型模型是
```
class Model
{
    public string Filed1 {get; set;}
    public string Field2 {get; set;}
}

```
我们在构建这个Model的实例时，不得不使用json序列化将field2的实例得到json文本，然后赋值给field2这个string属性，工作量大而且没有约束性。

依托于`JsonString<>`这个类型，现在只要我们把Field2结构声明为强类型模型，然后包装为`JsonString<>`类型即可。

```
class Model
{
    public string Filed1 {get; set;}
    public JsonString<Field2Info> Field2 {get; set;}
}

class Field2Info
{
    public string Name {get; set;}
    public Field2Data data {get; set;}
}

class Field2Data
{
    public string data1 {get; set;}
}
```

#### 3 响应没有指明ContentType
明明响应的内容是肉眼看上是json内容，但服务响应头里没有ContentType告诉客户端这内容是json，这好比客户端使用Form或json提交时就不在请求头告诉服务器内容格式是什么，而是让服务器猜测一样的道理。

解决办法是在Interface或Method声明`[JsonReturn]`特性，并设置其`EnsureMatchAcceptContentType`属性为false，表示ContentType不是期望值匹配也要处理。

```
[JsonReturn(EnsureMatchAcceptContentType = false)] 
public interface IJsonResponseApi : IHttpApi
{
}
```
#### 4 形形色色的各种签名
例如每个请求的url额外的动态添加一个叫sign的参数，这个sign可能和请求参数值有关联，每次都需要计算。

我们可以自定义ApiFilterAttribute来实现自己的sign功能，然后把自定义Filter声明到Interface或Method即可

```
class SignFilterAttribute : ApiFilterAttribute
{
    public override Task OnRequestAsync(ApiRequestContext context)
    {
        var sign = DateTime.Now.Ticks.ToString();
        context.HttpContext.RequestMessage.AddUrlQuery("sign", sign);
        return Task.CompletedTask;
    }
}

[SignFilter]
public interface ISignedApi 
{
    ...
}
```


### OAuths&Token
使用`WebApiClientCore.Extensions.OAuths`扩展，轻松支持token的获取、刷新与应用

### 1 注册相应类型的TokenProvider

```
// 为接口注册与配置token提者选项
services.AddClientCredentialsTokenProvider<IMyApi>(o =>
{
    o.Endpoint = new Uri("http://localhost:6000/api/tokens");
    o.Credentials.Client_id = "clientId";
    o.Credentials.Client_secret = "xxyyzz";
});
```
 
#### 2 声明对应的Token特性

```
/// <summary>
/// 用户操作接口
/// </summary>
[ClientCredentialsToken]
public interface IMyApi
{
    ...
}
```

#### 3 其它操作
> 清空Token，未过期的token也强制刷新

```
var providers = serviceProvider.GetServices<ITokenProvider>();
foreach(var item in providers)
{
    // 强制清除token以支持下次获取到新的token
    item.ClearToken();
}
```

> 自定义Token应用，得到token值，怎么用自己说了算

```
class MyTokenAttribute : ClientCredentialsTokenAttribute
{
    protected override void UseTokenResult(ApiRequestContext context, TokenResult tokenResult)
    {
        context.HttpContext.RequestMessage.Headers.TryAddWithoutValidation("xxx-header", tokenResult.Access_token);
    }
}

/// <summary>
/// 用户操作接口
/// </summary>
[MyToken]
public interface IMyApi
{
    ...
}
```

### 生态融合
`Microsoft.Extensions.Http`支持收入各种第三方的`HttpMessageHandler`来build出一种安全的`HttpClient`，同时支持将此`HttpClient`实例包装为强类型服务的目标服务类型注册功能。

### 1 Polly
`Microsoft.Extensions.Http.Polly`项目依托于Polly，将Polly策略实现到`System.Net.Http.DelegatingHandler`，其handler可以为`HttpClient`提供重试、降级和断路等功能。

### 2 WebApiClientCore
`WebApiClientCore`可以将`Microsoft.Extensions.Http`创建出来的`HttpClient`实例包装为声明式接口的代理实例，使开发者从面向命令式的编程模式直达声明式的AOP编程。

