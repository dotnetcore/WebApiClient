## WebApiClientCore 　　　　　　　　　　　　　　　　　　　
[WebApiClient.JIT](https://github.com/dotnetcore/WebApiClient/tree/WebApiClient.JITAOT)的netcoreapp版本，集高性能高可扩展性于一体的声明式http客户端库，特别适用于微服务的restful资源请求，也适用于各种畸形http接口请求。

### PackageReference

    <PackageReference Include="WebApiClientCore" Version="1.0.0-beta*" />
 

### Benchmark
使用[MockResponseHandler](https://github.com/dotnetcore/WebApiClient/tree/master/WebApiClientCore.Benchmarks/Requests)消除真实http请求，原生HttpClient、WebApiClientCore和[Refit](https://github.com/reactiveui/refit)的性能参考：

``` ini
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18362.836 (1903/May2019Update/19H1)
Intel Core i3-4150 CPU 3.50GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=3.1.202
  [Host]     : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
  DefaultJob : .NET Core 3.1.4 (CoreCLR 4.700.20.20201, CoreFX 4.700.20.22101), X64 RyuJIT
```
|                    Method |      Mean |     Error |    StdDev |
|-------------------------- |----------:|----------:|----------:|
|       HttpClient_GetAsync |  3.945 μs | 0.2050 μs | 0.5850 μs |
| WebApiClientCore_GetAsync | 13.320 μs | 0.2604 μs | 0.3199 μs |
|            Refit_GetAsync | 43.503 μs | 0.8489 μs | 1.0426 μs |

|                     Method |      Mean |     Error |    StdDev |
|--------------------------- |----------:|----------:|----------:|
|       HttpClient_PostAsync |  4.876 μs | 0.0972 μs | 0.2092 μs |
| WebApiClientCore_PostAsync | 14.018 μs | 0.1829 μs | 0.2246 μs |
|            Refit_PostAsync | 46.512 μs | 0.7885 μs | 0.7376 μs |



### 声明式接口定义
* 支持Task、Task<>和ITask<>三种异步返回
* 支持模型自动转换为Xml、Json、Form、和FormData共4种请求格式的内容
* 支持HttpResponseMessage、byte[]、string和Stream原生类型返回内容
* 支持原生HttpContent(比如StringContent)类型直接做为请求参数
* 内置丰富的能满足各种环境的常用特性(ActionAttribute和ParameterAttribute)
* 内置常用的FormDataFile等参数类型，同时支持自定义IApiParameter参数类型作为参数值
* 支持用户自定义IApiActionAttribute、IApiParameterAttribue、IApiReturnAttribute和IApiFilterAttribute

#### 1 Petstore接口例子
这个OpenApi文档在[petstore.swagger.io](https://petstore.swagger.io/)，代码为使用WebApiClientCore.OpenApi.SourceGenerator工具将其OpenApi文档反向生成得到

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
        Task UpdatePetWithFormAsync([Required] long petId, [FormFiled] string name, [FormFiled] string status);

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
        ITask<ApiResponse> UploadFileAsync([Required] long petId, [FormDataText] string additionalMetadata, FormDataFile file);
    }
}
```
####  2 IOAuthClient接口例子
这个接口是在WebApiClientCore.Extensions.OAuths.IOAuthClient.cs代码中声明

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
WebApiClientCore.Analyzers提供编码时语法分析与提示。

比如[Header]特性，可以声明在Interface、Method和Parameter三个地方，但是必须使用正确的构造器，否则运行时会抛出异常。有了语法分析功能，在声明接口时就不会使用不当的语法。如果想让语法分析生效，你的接口必须继承空方法的IHttpApi接口。

```
/// <summary>
/// 你的接口，记得要实现IHttpApi
/// </summary>
public interface IYourApi : IHttpApi
{
    ...
}
```


### 接口注册与实例获取
#### 1 接口服务注册

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

#### 2 接口实例获取

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

### 接口选项与配置
每个接口的选项对应为`HttpApiOptions<THttpApi>`，除了Action配置，我们也可以使用Configuration配置结合一起使用，这部分内容为Microsoft.Extensions.Options范畴。

#### Action配置

```
services
    .ConfigureHttpApi<IpetApi>(Configuration.GetSection(nameof(IpetApi)))
    .ConfigureHttpApi<IpetApi>(o =>
    {
        // 符合国情的不标准时间格式，有些接口就是这么要求必须不标准
        o.JsonSerializeOptions.Converters.Add(new JsonLocalDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
    });
```

#### appsettings.json的文件配置

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

### Uri拼接规则
所有的Uri拼接都是通过Uri(Uri baseUri, Uri relativeUri)这个构造器生成。

#### 带`/`结尾的baseUri

* `http://a.com/` + `b/c/d` = `http://a.com/b/c/d`
* `http://a.com/path1/` + `b/c/d` = `http://a.com/path1/b/c/d`
* `http://a.com/path1/path2/` + `b/c/d` = `http://a.com/path1/path2/b/c/d`

#### 不带`/`结尾的baseUri

* `http://a.com` + `b/c/d` = `http://a.com/b/c/d`
* `http://a.com/path1` + `b/c/d` = `http://a.com/b/c/d`
* `http://a.com/path1/path2` + `b/c/d` = `http://a.com/path1/b/c/d`

事实上`http://a.com`与`http://a.com/`是完全一样的，他们的path都是`/`，所以才会表现一样。为了避免低级错误的出现，请使用的标准baseUri书写方式，即使用`/`作为baseUri的结尾的第一种方式。


### 表单集合处理
按照OpenApi，一个集合在Uri的Query或表单中支持5种表述方式，分别是：
* Csv // 逗号分隔
* Ssv // 空格分隔
* Tsv // 反斜杠分隔
* Pipes // 竖线分隔
* Multi // 多个同名键的键值对

对于 id = new string []{"001","002"} 这样的值，处理后分别是
* id=001,002
* id=001 002
* id=001\002
* id=001|002
* id=001&id=002

默认的，PathQuryAttribute与FormContentAttribute使用了Multi处理方式，可以设置其CollectionFormat属性为其它值，比如：`[FormContent(CollectionFormat = CollectionFormat.Csv)]`

### Accpet ContentType
这个用于控制客户端希望服务器返回什么样的内容格式，比如json或xml。

#### 缺省配置值

缺省配置是[JsonReturn(0.01),XmlReturn(0.01)]，对应的请求accept值是
`Accept: application/json; q=0.01, application/xml; q=0.01`

#### Json优先

在Interface或Method上显式地声明`[JsonReturn]`，请求accept变为`Accept: application/json, application/xml; q=0.01`

#### 禁用json

在Interface或Method上声明`[JsonReturn(Enable = false)]`，请求变为`Accept: application/xml; q=0.01`


### 请求和响应日志
在整个Interface或某个Method上声明`[LoggingFilter]`，即可把请求和响应的内容输出到LoggingFactory中。如果要排除某个Method不打印日志，在该Method上声明`[LoggingFilter(Enable = false)]`，即可将本Method排除。

#### 默认日志

```
[LoggingFilter]   
public interface IUserApi : IHttpApi
{
    [HttpGet("api/users/{account}")]
    ITask<HttpResponseMessage> GetAsync([Required]string account);  

    // 禁用日志
    [LoggingFilter(Enable =false)]
    [HttpPost("api/users/body")]
    Task<User> PostByJsonAsync([Required, JsonContent]User user, CancellationToken token = default);
}
```

#### 自定义日志输出目标
```
class MyLogging : LoggingFilterAttribute
{
    protected override Task WriteLogAsync(ApiResponseContext context, LogMessage logMessage)
    {
        xxlogger.Log(logMessage.ToIndentedString(spaceCount: 4));
        return Task.CompletedTask;
    }
}
```

### 请求条件性重试
使用ITask<>异步声明，就有Retry的扩展，Retry的条件可以为捕获到某种Exception或响应模型符合某种条件。

```
var result = await youApi.GetModelAsync(id: "id001")
    .Retry(maxCount: 3)
    .WhenCatch<HttpRequestException>()
    .WhenResult(r => r.ErrorCode > 0);
```
### 异常和异常处理
请求一个接口，不管出现何种异常，最终都抛出HttpRequestException，HttpRequestException的内部异常为实际具体异常，之所以设计为内部异常，是为了完好的保存内部异常的堆栈信息。

WebApiClient内部的很多异常都基于ApiException这个抽象异常，也就是很多情况下，抛出的异常都是内为某个ApiException的HttpRequestException。

```
try
{
    var model = await api.GetAsync();
}
catch (HttpRequestException ex) when (ex.InnerException is ApiInvalidConfigException configException)
{
    // 请求配置异常
}
catch (HttpRequestException ex) when (ex.InnerException is ApiResponseStatusException statusException)
{
    // 响应状态码异常
}
catch (HttpRequestException ex) when (ex.InnerException is ApiException apiException)
{
    // 抽象的api异常
}
catch (HttpRequestException ex) when (ex.InnerException is SocketException socketException)
{
    // socket连接层异常
}
catch (HttpRequestException ex)
{
    // 请求异常
}
catch (Exception ex)
{
    // 异常
}
```
### 响应内容缓存
配置CacheAttribute特性的Method会将本次的响应内容缓存起来，下一次如果符合预期条件的话，就不会再请求到远程服务器，而是从IResponseCacheProvider获取缓存内容，开发者可以自己实现ResponseCacheProvider。

#### 声明缓存特性
```
// 缓存一分钟
[Cache(60 * 1000)]
[HttpGet("api/users/{account}")]
ITask<HttpResponseMessage> GetAsync([Required]string account);
```

#### 自定义缓存提供者
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

### 非模型请求
有时候我们未必需要强模型，假设我们已经有原始的form文本内容，或原始的json文本内容，甚至是System.Net.Http.HttpContent对象，只需要把这些原始内请求到远程远程器。

#### 原始文本

```
[HttpPost]
Task PostAsync([RawStringContent("txt/plain")] string text);

[HttpPost]
Task PostAsync(StringContent text);
```


#### 原始json
```
[HttpPost]
Task PostAsync([RawJsonContent] string json);
```

#### 原始xml

```
[HttpPost]
Task PostAsync([RawXmlContent] string xml);
```

#### 原始表单内容

```
[HttpPost]
Task PostAsync([RawFormContent] string form);
```


### 自定义无特性的参数类型
在某些极限情况下，比如人脸比对的接口，我们输入模型与传输模型未必是对等的：

#### 服务端要求的json模型
```
{
    "image1" : "图片1的base64",
    "image2" : "图片2的base64"
}
```

#### 客户端期望的业务模型
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

### 自定义请求内容与响应内容解析
除了常见的xml或json响应内容要反序列化为强类型结果模型，你可能会遇到其它的二进制协议响应内容，比如google的ProtoBuf二进制内容。

#### 1 编写相关自定义特性

##### 自定义请求内容处理特性
```
public class ProtobufContentAttribute : HttpContentAttribute
{
    public string ContentType { get; set; } = "application/x-protobuf";

    protected override Task SetHttpContentAsync(ApiParameterContext context)
    {
        var stream = new MemoryStream();
        if (context.ParameterValue != null)
        {
            Serializer.NonGeneric.Serialize(stream, context.ParameterValue);
            stream.Position = 0L;
        }

        var content = new StreamContent(stream);
        content.Headers.ContentType = new MediaTypeHeaderValue(this.ContentType);
        context.HttpContext.RequestMessage.Content = content;
        return Task.CompletedTask;
    }
}
```
##### 自定义响应内容解析特性

```
public class ProtobufReturnAttribute : ApiReturnAttribute
{
    public ProtobufReturnAttribute(string acceptContentType = "application/x-protobuf")
        : base(new MediaTypeWithQualityHeaderValue(acceptContentType))
    {
    }

    public override async Task SetResultAsync(ApiResponseContext context)
    {
        if (context.ApiAction.Return.DataType.IsRawType == false)
        {
            var stream = await context.HttpContext.ResponseMessage.Content.ReadAsStreamAsync();
            context.Result = Serializer.NonGeneric.Deserialize(context.ApiAction.Return.DataType.Type, stream);
        }
    }
}
```

#### 2 应用相关自定义特性
```
[ProtobufReturn]
public interface IProtobufApi
{
    [HttpPut("/users/{id}")]
    Task<User> UpdateAsync([Required, PathQuery] string id, [ProtobufContent] User user);
}
```

### 适配畸形接口
在实际应用场景中，常常会遇到一些设计不标准的畸形接口，主要是早期还没有restful概念时期的接口，我们要区分分析这些接口，包装为好友的客户端调用接口。

#### 不好友的参数名别名
例如服务器要求一个Query参数的名字为`field-Name`，这个是c#关键字或变量命名不允许的，我们可以使用`[AliasAsAttribute]`来达到这个要求：

```
[HttpGet("api/users/{account}")]
ITask<HttpResponseMessage> GetAsync([Required]string account, [AliasAs("field-Name")] string fieldName);
```

然后最终请求uri变为api/users/`account1`?field-name=`fileName1`

#### Form的某个字段为json文本

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

依托于`JsonString<>`这个类型，现在只要我们把Field2结构声明为强类型模型，然后包装为`JsonString<>`类型，最后为HttpApiOptions添加JsonStringTypeConverter即可。

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


// 添加转换器 
services
    .AddHttpApi<IMyApi>(o =>
    {
        o.HttpHost = new Uri("http://localhost:6000/");
        o.KeyValueSerializeOptions.Converters.Add(JsonStringTypeConverter.Default);
    }); 
``` 

#### 响应未指明ContentType
明明响应的内容肉眼看上是json内容，但服务响应头里没有ContentType告诉客户端这内容是json，这好比客户端使用Form或json提交时就不在请求头告诉服务器内容格式是什么，而是让服务器猜测一样的道理。

解决办法是在Interface或Method声明`[JsonReturn]`特性，并设置其EnsureMatchAcceptContentType属性为false，表示ContentType不是期望值匹配也要处理。

```
[JsonReturn(EnsureMatchAcceptContentType = false)] 
public interface IJsonResponseApi : IHttpApi
{
}
```
#### 类签名参数或token参数
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


### HttpMessageHandler配置

#### Http代理配置

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

#### 客户端证书配置

有些服务器为了限制客户端的连接，开启了https双向验证，只允许它执有它颁发的证书的客户端进行连接
```
services
    .AddHttpApi<IMyApi>(o =>
    {
        o.HttpHost = new Uri("http://localhost:6000/");
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(yourCert);
        return handler;
    });
```

#### 维持CookieContainer不变

如果请求的接口不幸使用了Cookie保存身份信息机制，那么就要考虑维持CookieContainer实例不要跟随HttpMessageHandler的生命周期，默认的HttpMessageHandler最短只有2分钟的生命周期。

```
var cookieContainer = new CookieContainer();
services
    .AddHttpApi<IMyApi>(o =>
    {
        o.HttpHost = new Uri("http://localhost:6000/");
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        handler.CookieContainer = cookieContainer;
        return handler;
    });
```

### OAuths&Token
使用WebApiClientCore.Extensions.OAuths扩展，轻松支持token的获取、刷新与应用

#### 1 注册相应类型的TokenProvider

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
Microsoft.Extensions.Http支持收入各种第三方的HttpMessageHandler来build出一种安全的HttpClient，同时支持将此HttpClient实例包装为强类型服务的目标服务类型注册功能。

#### Polly
Microsoft.Extensions.Http.Polly项目依托于Polly，将Polly策略实现到System.Net.Http.DelegatingHandler，其handler可以为HttpClient提供重试、降级和断路等功能。

#### WebApiClientCore
WebApiClientCore可以将Microsoft.Extensions.Http创建出来的HttpClient实例包装为声明式接口的代理实例，使开发者从面向命令式的编程模式直达声明式的AOP编程。

