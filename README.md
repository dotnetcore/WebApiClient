## WebApiClientCore 　　　　　　　　　　　　　　　　　　　
[WebApiClient.JIT/AOT](https://github.com/dotnetcore/WebApiClient/tree/WebApiClient.JITAOT)的netcore版本，集高性能高可扩展性于一体的声明式http客户端库，特别适用于微服务的restful资源请求，也适用于各种畸形http接口请求。


### Nuget

| 包名 | 描述 | Nuget |
---|---|--|
| WebApiClientCore | 基础包 | [![NuGet](https://buildstats.info/nuget/WebApiClientCore)](https://www.nuget.org/packages/WebApiClientCore) |
| WebApiClientCore.Extensions.OAuths | OAuth扩展包 | [![NuGet](https://buildstats.info/nuget/WebApiClientCore.Extensions.OAuths)](https://www.nuget.org/packages/WebApiClientCore.Extensions.OAuths) |
| WebApiClientCore.Extensions.NewtonsoftJson | Json.Net扩展包 | [![NuGet](https://buildstats.info/nuget/WebApiClientCore.Extensions.NewtonsoftJson)](https://www.nuget.org/packages/WebApiClientCore.Extensions.NewtonsoftJson) |
| WebApiClientCore.Extensions.JsonRpc | JsonRpc调用扩展包 | [![NuGet](https://buildstats.info/nuget/WebApiClientCore.Extensions.JsonRpc)](https://www.nuget.org/packages/WebApiClientCore.Extensions.JsonRpc) |
| WebApiClientCore.OpenApi.SourceGenerator | 将本地或远程OpenApi文档解析生成WebApiClientCore接口代码的dotnet tool | [![NuGet](https://buildstats.info/nuget/WebApiClientCore.OpenApi.SourceGenerator)](https://www.nuget.org/packages/WebApiClientCore.OpenApi.SourceGenerator) |

### 如何使用

```
[HttpHost("http://localhost:5000/")]
public interface IUserApi
{
    [HttpGet("api/users/{id}")]
    Task<User> GetAsync(string id);
    
    [HttpPost("api/users")]
    Task<User> PostAsync([JsonContent] User user);
}
```
```
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpApi<IUserApi>();
}
```

```
public class MyService
{
    private readonly IUserApi userApi;
    public MyService(IUserApi userApi)
    {
        this.userApi = userApi;
    }
}
```
    
### QQ群协助
> [825135345](https://shang.qq.com/wpa/qunwpa?idkey=c6df21787c9a774ca7504a954402c9f62b6595d1e63120eabebd6b2b93007410)

进群时请注明**WebApiClient**，在咨询问题之前，请先认真阅读以下剩余的文档，避免消耗作者不必要的重复解答时间。
 
### 编译时语法分析
WebApiClientCore.Analyzers提供编码时语法分析与提示，声明的接口继承了空方法的IHttpApi接口，语法分析将生效，建议开发者开启这个功能。

例如[Header]特性，可以声明在Interface、Method和Parameter三个地方，但是必须使用正确的构造器，否则运行时会抛出异常。有了语法分析功能，在声明接口时就不会使用不当的语法。

```
/// <summary>
/// 记得要实现IHttpApi
/// </summary>
public interface IUserApi : IHttpApi
{
    ...
}
```

### 接口配置与选项
每个接口的选项对应为`HttpApiOptions`，选项名称为接口的完整名称，也可以通过HttpApi.GetName()方法获取得到。

#### 在IHttpClientBuilder配置
```
services
    .AddHttpApi<IUserApi>()
    .ConfigureHttpApi(Configuration.GetSection(nameof(IUserApi)))
    .ConfigureHttpApi(o =>
    {
        // 符合国情的不标准时间格式，有些接口就是这么要求必须不标准
        o.JsonSerializeOptions.Converters.Add(new JsonLocalDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
    });
```

配置文件的json
```
{
  "IUserApi": {
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

#### 在IServiceCollection配置
```
services
    .ConfigureHttpApi<IUserApi>(Configuration.GetSection(nameof(IUserApi)))
    .ConfigureHttpApi<IUserApi>(o =>
    {
        // 符合国情的不标准时间格式，有些接口就是这么要求必须不标准
        o.JsonSerializeOptions.Converters.Add(new JsonLocalDateTimeConverter("yyyy-MM-dd HH:mm:ss"));
    });
```

### 数据验证
#### 参数值验证
对于参数值，支持ValidationAttribute特性修饰来验证值。
```
public interface IUserApi
{
    [HttpGet("api/users/{email}")]
    Task<User> GetAsync([EmailAddress, Required] string email);
}
```

#### 参数或返回模型属性验证
```
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

### 常用内置特性
内置特性指框架内提供的一些特性，拿来即用就能满足一般情况下的各种应用。当然，开发者也可以在实际应用中，编写满足特定场景需求的特性，然后将自定义特性修饰到接口、方法或参数即可。

#### Return特性

特性名称 | 功能描述 | 备注
---|---|---|
RawReturnAttribute | 处理原始类型返回值 | 缺省也生效
JsonReturnAttribute | 处理Json模型返回值 | 缺省也生效
XmlReturnAttribute | 处理Xml模型返回值 | 缺省也生效

#### 常用Action特性

特性名称 | 功能描述 | 备注
---|---|---|
HttpHostAttribute | 请求服务http绝对完整主机域名| 优先级比Options配置低
HttpGetAttribute | 声明Get请求方法与路径| 支持null、绝对或相对路径
HttpPostAttribute | 声明Post请求方法与路径| 支持null、绝对或相对路径
HttpPutAttribute | 声明Put请求方法与路径| 支持null、绝对或相对路径
HttpDeleteAttribute | 声明Delete请求方法与路径| 支持null、绝对或相对路径
*HeaderAttribute* | 声明请求头 | 常量值
*TimeoutAttribute* | 声明超时时间 | 常量值
*FormFieldAttribute* | 声明Form表单字段与值 | 常量键和值
*FormDataTextAttribute* | 声明FormData表单字段与值 | 常量键和值

#### 常用Parameter特性

特性名称 | 功能描述 | 备注
---|---|---|
PathQueryAttribute | 参数值的键值对作为url路径参数或query参数的特性 | 缺省特性的参数默认为该特性
FormContentAttribute | 参数值的键值对作为x-www-form-urlencoded表单 | 
FormDataContentAttribute | 参数值的键值对作为multipart/form-data表单 | 
JsonContentAttribute | 参数值序列化为请求的json内容 |
XmlContentAttribute | 参数值序列化为请求的xml内容 |
UriAttribute | 参数值作为请求uri | 只能修饰第一个参数
ParameterAttribute | 聚合性的请求参数声明 | 不支持细颗粒配置
*HeaderAttribute* | 参数值作为请求头 | 
*TimeoutAttribute* | 参数值作为超时时间 | 值不能大于HttpClient的Timeout属性
*FormFieldAttribute* | 参数值作为Form表单字段与值 | 只支持简单类型参数
*FormDataTextAttribute* | 参数值作为FormData表单字段与值 | 只支持简单类型参数

#### Filter特性

特性名称 | 功能描述| 备注
---|---|---|
ApiFilterAttribute | Filter特性抽象类 | 
LoggingFilterAttribute | 请求和响应内容的输出为日志的过滤器 |

#### 自解释参数类型

类型名称 | 功能描述 | 备注
---|---|---|
FormDataFile | form-data的一个文件项 | 无需特性修饰，等效于FileInfo类型
JsonPatchDocument | 表示将JsonPatch请求文档 | 无需特性修饰


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

对于 id = new string []{"001","002"} 这样的值，在PathQueryAttribute与FormContentAttribute处理后分别是：

CollectionFormat | Data
---|---
[PathQuery(CollectionFormat = CollectionFormat.Csv)] | `id=001,002`
[PathQuery(CollectionFormat = CollectionFormat.Ssv)] | `id=001 002`
[PathQuery(CollectionFormat = CollectionFormat.Tsv)] | `id=001\002`
[PathQuery(CollectionFormat = CollectionFormat.Pipes)] | `id=001|002`
[PathQuery(CollectionFormat = CollectionFormat.Multi)] | `id=001&id=002`
 



### CancellationToken参数
每个接口都支持声明一个或多个CancellationToken类型的参数，用于支持取消请求操作。CancellationToken.None表示永不取消，创建一个CancellationTokenSource，可以提供一个CancellationToken。

```
[HttpGet("api/users/{id}")]
ITask<User> GetAsync([Required]string id, CancellationToken token = default);
```

### ContentType CharSet
对于非表单的body内容，默认或缺省时的charset值，对应的是UTF8编码，可以根据服务器要求调整编码。


Attribute | ContentType
---|---
[JsonContent] | Content-Type: application/json; charset=utf-8
[JsonContent(CharSet ="utf-8")] | Content-Type: application/json; charset=utf-8
[JsonContent(CharSet ="unicode")] | Content-Type: application/json; charset=utf-16



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
public interface IUserApi
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
class MyLoggingAttribute : LoggingFilterAttribute
{
    protected override Task WriteLogAsync(ApiResponseContext context, LogMessage logMessage)
    {
        xxlogger.Log(logMessage.ToIndentedString(spaceCount: 4));
        return Task.CompletedTask;
    }
}

[MyLogging]   
public interface IUserApi
{
}
```

### 原始类型返回值

当接口返回值声明为如下类型时，我们称之为原始类型，会被RawReturnAttribute处理。

返回类型 | 说明
---|---
`Task` | 不关注响应消息
`Task<HttpResponseMessage>` | 原始响应消息类型
`Task<Stream>` | 原始响应流
`Task<byte[]>` | 原始响应二进制数据
`Task<string>` | 原始响应消息文本

### 接口声明示例
#### Petstore接口
这个OpenApi文档在[petstore.swagger.io](https://petstore.swagger.io/)，代码为使用WebApiClientCore.OpenApi.SourceGenerator工具将其OpenApi文档反向生成得到

```
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
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [HttpPost("pet")]
    Task AddPetAsync([Required] [JsonContent] Pet body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing pet
    /// </summary>
    /// <param name="body">Pet object that needs to be added to the store</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [HttpPut("pet")]
    Task UpdatePetAsync([Required] [JsonContent] Pet body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds Pets by status
    /// </summary>
    /// <param name="status">Status values that need to be considered for filter</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>successful operation</returns>
    [HttpGet("pet/findByStatus")]
    ITask<List<Pet>> FindPetsByStatusAsync([Required] IEnumerable<Anonymous> status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds Pets by tags
    /// </summary>
    /// <param name="tags">Tags to filter by</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>successful operation</returns>
    [Obsolete]
    [HttpGet("pet/findByTags")]
    ITask<List<Pet>> FindPetsByTagsAsync([Required] IEnumerable<string> tags, CancellationToken cancellationToken = default);

    /// <summary>
    /// Find pet by ID
    /// </summary>
    /// <param name="petId">ID of pet to return</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>successful operation</returns>
    [HttpGet("pet/{petId}")]
    ITask<Pet> GetPetByIdAsync([Required] long petId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a pet in the store with form data
    /// </summary>
    /// <param name="petId">ID of pet that needs to be updated</param>
    /// <param name="name">Updated name of the pet</param>
    /// <param name="status">Updated status of the pet</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [HttpPost("pet/{petId}")]
    Task UpdatePetWithFormAsync([Required] long petId, [FormField] string name, [FormField] string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a pet
    /// </summary>
    /// <param name="api_key"></param>
    /// <param name="petId">Pet id to delete</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [HttpDelete("pet/{petId}")]
    Task DeletePetAsync([Header("api_key")] string api_key, [Required] long petId, CancellationToken cancellationToken = default);

    /// <summary>
    /// uploads an image
    /// </summary>
    /// <param name="petId">ID of pet to update</param>
    /// <param name="additionalMetadata">Additional data to pass to server</param>
    /// <param name="file">file to upload</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>successful operation</returns>
    [HttpPost("pet/{petId}/uploadImage")]
    ITask<ApiResponse> UploadFileAsync([Required] long petId, [FormDataText] string additionalMetadata, FormDataFile file, CancellationToken cancellationToken = default);
}
```
#### IOAuthClient接口
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

### 请求条件性重试
使用ITask<>异步声明，就有Retry的扩展，Retry的条件可以为捕获到某种Exception或响应模型符合某种条件。

```
public interface IUserApi
{
    [HttpGet("api/users/{id}")]
    ITask<User> GetAsync(string id);
}

var result = await userApi.GetAsync(id: "id001")
    .Retry(maxCount: 3)
    .WhenCatch<HttpRequestException>()
    .WhenResult(r => r.Age <= 0);
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

### PATCH请求
json patch是为客户端能够局部更新服务端已存在的资源而设计的一种标准交互，在RFC6902里有详细的介绍json patch，通俗来讲有以下几个要点：

1. 使用HTTP PATCH请求方法；
2. 请求body为描述多个opration的数据json内容；
3. 请求的Content-Type为application/json-patch+json；

#### 声明Patch方法
```
public interface IUserApi
{
    [HttpPatch("api/users/{id}")]
    Task<UserInfo> PatchAsync(string id, JsonPatchDocument<User> doc);
}
```

#### 实例化JsonPatchDocument
```
var doc = new JsonPatchDocument<User>();
doc.Replace(item => item.Account, "laojiu");
doc.Replace(item => item.Email, "laojiu@qq.com");
```

#### 请求内容
```
PATCH /api/users/id001 HTTP/1.1
Host: localhost:6000
User-Agent: WebApiClientCore/1.0.0.0
Accept: application/json; q=0.01, application/xml; q=0.01
Content-Type: application/json-patch+json

[{"op":"replace","path":"/account","value":"laojiu"},{"op":"replace","path":"/email","value":"laojiu@qq.com"}]
```


### 响应内容缓存
配置CacheAttribute特性的Method会将本次的响应内容缓存起来，下一次如果符合预期条件的话，就不会再请求到远程服务器，而是从IResponseCacheProvider获取缓存内容，开发者可以自己实现ResponseCacheProvider。

#### 声明缓存特性
```
public interface IUserApi
{
    // 缓存一分钟
    [Cache(60 * 1000)]
    [HttpGet("api/users/{account}")]
    ITask<HttpResponseMessage> GetAsync([Required]string account);
}
```
默认缓存条件：URL(如`http://abc.com/a`)和指定的请求Header一致。
如果需要类似`[CacheByPath]`这样的功能，可直接继承`ApiCacheAttribute`来实现:

```
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class CacheByAbsolutePathAttribute : ApiCacheAttribute
    {
        public CacheByPathAttribute(double expiration) : base(expiration)
        {
        }

        public override Task<string> GetCacheKeyAsync(ApiRequestContext context)
        {
            return Task.FromResult(context.HttpContext.RequestMessage.RequestUri.AbsolutePath);
        }
    }
```


#### 自定义缓存提供者
默认的缓存提供者为内存缓存，如果希望将缓存保存到其它存储位置，则需要自定义 缓存提者，并注册替换默认的缓存提供者。

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


### 自定义自解释的参数类型
在某些极限情况下，比如人脸比对的接口，我们输入模型与传输模型未必是对等的，例如：

**服务端要求的json模型**
```
{
    "image1" : "图片1的base64",
    "image2" : "图片2的base64"
}
```

**客户端期望的业务模型**
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

        var jsonContent = new JsonContent();
        context.HttpContext.RequestMessage.Content = jsonContent;

        var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;
        var serializer = context.HttpContext.ServiceProvider.GetJsonSerializer();
        serializer.Serialize(jsonContent, model, options);
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
在实际应用场景中，常常会遇到一些设计不标准的畸形接口，主要是早期还没有restful概念时期的接口，我们要区分分析这些接口，包装为友好的客户端调用接口。

#### 不友好的参数名别名
例如服务器要求一个Query参数的名字为`field-Name`，这个是c#关键字或变量命名不允许的，我们可以使用`[AliasAsAttribute]`来达到这个要求：

```
public interface IDeformedApi
{
    [HttpGet("api/users")]
    ITask<string> GetAsync([AliasAs("field-Name")] string fieldName);  
}
```

然后最终请求uri变为api/users/?field-name=`fileNameValue`

#### Form的某个字段为json文本

字段 | 值
---|---
field1 | someValue
field2 | {"name":"sb","age":18}

对应强类型模型是
```
class Field2
{
    public string Name {get; set;}
    
    public int Age {get; set;}
}
```
常规下我们得把field2的实例json序列化得到json文本，然后赋值给field2这个string属性，使用[JsonFormField]特性可以轻松帮我们自动完成Field2类型的json序列化并将结果字符串作为表单的一个字段。
 

```
public interface IDeformedApi
{
    Task PostAsync([FormField] string field1, [JsonFormField] Field2 field2)
}
``` 

#### Form提交嵌套的模型

字段 | 值
---|---|
|filed1 |someValue|
|field2.name | sb|
|field2.age | 18|

其对应的json格式为
```
{
    "field1" : "someValue",
    "filed2" : {
        "name" : "sb",
        "age" : 18
    }
}
```
合理情况下，对于复杂嵌套结构的数据模型，应当使用applicaiton/json，但接口要求必须使用Form提交，我可以配置KeyValueSerializeOptions来达到这个格式要求：

```
services.AddHttpApi<IDeformedApi>(o =>
{
    o.KeyValueSerializeOptions.KeyNamingStyle = KeyNamingStyle.FullName;
});
```

#### 响应未指明ContentType
明明响应的内容肉眼看上是json内容，但服务响应头里没有ContentType告诉客户端这内容是json，这好比客户端使用Form或json提交时就不在请求头告诉服务器内容格式是什么，而是让服务器猜测一样的道理。

解决办法是在Interface或Method声明`[JsonReturn]`特性，并设置其EnsureMatchAcceptContentType属性为false，表示ContentType不是期望值匹配也要处理。

```
[JsonReturn(EnsureMatchAcceptContentType = false)] 
public interface IDeformedApi 
{
}
```

#### 类签名参数或apikey参数
例如每个请求的url额外的动态添加一个叫sign的参数，这个sign可能和请求参数值有关联，每次都需要计算。

我们可以自定义ApiFilterAttribute来实现自己的sign功能，然后把自定义Filter声明到Interface或Method即可

```
class SignFilterAttribute : ApiFilterAttribute
{
    public override Task OnRequestAsync(ApiRequestContext context)
    {
        var signService = context.HttpContext.ServiceProvider.GetService<SignService>();
        var sign = signService.SignValue(DateTime.Now);
        context.HttpContext.RequestMessage.AddUrlQuery("sign", sign);
        return Task.CompletedTask;
    }
}

[SignFilter]
public interface IDeformedApi 
{
    ...
}
```


### HttpMessageHandler配置

#### Http代理配置

```
services
    .AddHttpApi<IUserApi>(o =>
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
    .AddHttpApi<IUserApi>(o =>
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
    .AddHttpApi<IUserApi>(o =>
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
使用WebApiClientCore.Extensions.OAuths扩展，轻松支持token的获取、刷新与应用。

#### 对象与概念

对象 | 用途
---|---
ITokenProviderFactory | tokenProvider的创建工厂，提供通过HttpApi接口类型创建tokenProvider
ITokenProvider | token提供者，用于获取token，在token的过期后的头一次请求里触发重新请求或刷新token
OAuthTokenAttribute | token的应用特性，使用ITokenProviderFactory创建ITokenProvider，然后使用ITokenProvider获取token，最后将token应用到请求消息中
OAuthTokenHandler | 属于http消息处理器，功能与OAuthTokenAttribute一样，除此之外，如果因为意外的原因导致服务器仍然返回未授权(401状态码)，其还会丢弃旧token，申请新token来重试一次请求。

#### OAuth的Client模式

##### 1 为接口注册tokenProvider
```
// 为接口注册与配置Client模式的tokenProvider
services.AddClientCredentialsTokenProvider<IUserApi>(o =>
{
    o.Endpoint = new Uri("http://localhost:6000/api/tokens");
    o.Credentials.Client_id = "clientId";
    o.Credentials.Client_secret = "xxyyzz";
});
```
 
##### 2 token的应用

###### 2.1 使用OAuthToken特性
OAuthTokenAttribute属于WebApiClientCore框架层，很容易操控请求内容和响应模型，比如将token作为表单字段添加到既有请求表单中，或者读取响应消息反序列化之后对应的业务模型都非常方便，但它不能在请求内部实现重试请求的效果。在服务器颁发token之后，如果服务器的token丢失了，使用OAuthTokenAttribute会得到一次失败的请求，本次失败的请求无法避免。

```
/// <summary>
/// 用户操作接口
/// </summary>
[OAuthToken]
public interface IUserApi
{
    ...
}
```

OAuthTokenAttribute默认实现将token放到Authorization请求头，如果你的接口需要请token放到其它地方比如uri的query，需要重写OAuthTokenAttribute：

```
class UriQueryTokenAttribute : OAuthTokenAttribute
{
    protected override void UseTokenResult(ApiRequestContext context, TokenResult tokenResult)
    {
        context.HttpContext.RequestMessage.AddUrlQuery("mytoken", tokenResult.Access_token);
    }
}

[UriQueryToken]
public interface IUserApi
{
    ...
}
```

###### 2.1 使用OAuthTokenHandler
OAuthTokenHandler的强项是支持在一个请求内部里进行多次尝试，在服务器颁发token之后，如果服务器的token丢失了，OAuthTokenHandler在收到401状态码之后，会在本请求内部丢弃和重新请求token，并使用新token重试请求，从而表现为一次正常的请求。但OAuthTokenHandler不属于WebApiClientCore框架层的对象，在里面只能访问原始的HttpRequestMessage与HttpResponseMessage，如果需要将token追加到HttpRequestMessage的Content里，这是非常困难的，同理，如果不是根据http状态码(401等)作为token无效的依据，而是使用HttpResponseMessage的Content对应的业务模型的某个标记字段，也是非常棘手的活。

```
// 注册接口时添加OAuthTokenHandler
services
    .AddHttpApi<IUserApi>()
    .AddOAuthTokenHandler();
```

OAuthTokenHandler默认实现将token放到Authorization请求头，如果你的接口需要请token放到其它地方比如uri的query，需要重写OAuthTokenHandler：

```
class UriQueryOAuthTokenHandler : OAuthTokenHandler
{
    /// <summary>
    /// token应用的http消息处理程序
    /// </summary>
    /// <param name="tokenProvider">token提供者</param> 
    public UriQueryOAuthTokenHandler(ITokenProvider tokenProvider)
        : base(tokenProvider)
    {
    }

    /// <summary>
    /// 应用token
    /// </summary>
    /// <param name="request"></param>
    /// <param name="tokenResult"></param>
    protected override void UseTokenResult(HttpRequestMessage request, TokenResult tokenResult)
    {
        var builder = new UriBuilder(request.RequestUri);
        builder.Query += "mytoken=" + Uri.EscapeDataString(tokenResult.Access_token);
        request.RequestUri = builder.Uri;
    }
}


// 注册接口时添加UriQueryOAuthTokenHandler
services
    .AddHttpApi<IUserApi>()
    .AddOAuthTokenHandler((s, tp) => new UriQueryOAuthTokenHandler(tp));
```

#### 多接口共享的TokenProvider
可以给http接口设置基础接口，然后为基础接口配置TokenProvider，例如下面的xxx和yyy接口，都属于IBaidu，只需要给IBaidu配置TokenProvider。
```
public interface IBaidu
{
}

[OAuthToken]
public interface IBaidu_XXX_Api : IBaidu
{
    [HttpGet]
    Task xxxAsync();
}

[OAuthToken]
public interface IBaidu_YYY_Api : IBaidu
{
    [HttpGet]
    Task yyyAsync();
}
```

```
// 注册与配置password模式的token提者选项
services.AddPasswordCredentialsTokenProvider<IBaidu>(o =>
{
    o.Endpoint = new Uri("http://localhost:5000/api/tokens");
    o.Credentials.Client_id = "clientId";
    o.Credentials.Client_secret = "xxyyzz";
    o.Credentials.Username = "username";
    o.Credentials.Password = "password";
});
```

#### 自定义TokenProvider
扩展包已经内置了OAuth的Client和Password模式两种标准token请求，但是仍然还有很多接口提供方在实现上仅仅体现了它的精神，这时候就需要自定义TokenProvider，假设接口提供方的获取token的接口如下：
```
public interface ITokenApi
{
    [HttpPost("http://xxx.com/token")]
    Task<TokenResult> RequestTokenAsync([Parameter(Kind.Form)] string clientId, [Parameter(Kind.Form)] string clientSecret);
}
```

##### 委托TokenProvider
委托TokenProvider是一种最简单的实现方式，它将请求token的委托作为自定义TokenProvider的实现逻辑：

```
// 为接口注册自定义tokenProvider
services.AddTokeProvider<IUserApi>(s =>
{
    return s.GetService<ITokenApi>().RequestTokenAsync("id", "secret");
});
```

##### 完整实现的TokenProvider

```
// 为接口注册CustomTokenProvider
services.AddTokeProvider<IUserApi, CustomTokenProvider>();
```

```
class CustomTokenProvider : TokenProvider
{
    public CustomTokenProvider(IServiceProvider serviceProvider)
        : base(serviceProvider)
    {
    }

    protected override Task<TokenResult> RequestTokenAsync(IServiceProvider serviceProvider)
    {
        return serviceProvider.GetService<ITokenApi>().RequestTokenAsync("id", "secret");
    }

    protected override Task<TokenResult> RefreshTokenAsync(IServiceProvider serviceProvider, string refresh_token)
    {
        return this.RequestTokenAsync(serviceProvider);
    }
}
```

##### 自定义TokenProvider的选项
每个TokenProvider都有一个Name属性，与service.AddTokeProvider()返回的ITokenProviderBuilder的Name是同一个值。读取Options值可以使用TokenProvider的GetOptionsValue()方法，配置Options则通过ITokenProviderBuilder的Name来配置。


### NewtonsoftJson处理json
不可否认，System.Text.Json由于性能的优势，会越来越得到广泛使用，但NewtonsoftJson也不会因此而退出舞台。

System.Text.Json在默认情况下十分严格，避免代表调用方进行任何猜测或解释，强调确定性行为，该库是为了实现性能和安全性而特意这样设计的。Newtonsoft.Json默认情况下十分灵活，默认的配置下，你几乎不会遇到反序列化的种种问题，虽然这些问题很多情况下是由于不严谨的json结构或类型声明造成的。

  
#### 扩展包

默认的基础包是不包含NewtonsoftJson功能的，需要额外引用WebApiClientCore.Extensions.NewtonsoftJson这个扩展包。

#### 配置[可选]
```
// ConfigureNewtonsoftJson
services.AddHttpApi<IUserApi>().ConfigureNewtonsoftJson(o =>
{
    o.JsonSerializeOptions.NullValueHandling = NullValueHandling.Ignore;
});
```
 
#### 声明特性
使用[JsonNetReturn]替换内置的[JsonReturn]，[JsonNetContent]替换内置[JsonContent]
```
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


### JsonRpc调用
在极少数场景中，开发者可能遇到JsonRpc调用的接口，由于该协议不是很流行，WebApiClientCore将该功能的支持作为WebApiClientCore.Extensions.JsonRpc扩展包提供。使用[JsonRpcMethod]修饰Rpc方法，使用[JsonRpcParam]修饰Rpc参数
即可。

#### JsonRpc声明
```c#
[HttpHost("http://localhost:5000/jsonrpc")]
public interface IUserApi 
{
    [JsonRpcMethod("add")]
    ITask<JsonRpcResult<User>> AddAsync([JsonRpcParam] string name, [JsonRpcParam] int age, CancellationToken token = default);
}
```

#### JsonRpc数据包
```
POST /jsonrpc HTTP/1.1
Host: localhost:5000
User-Agent: WebApiClientCore/1.0.6.0
Accept: application/json; q=0.01, application/xml; q=0.01
Content-Type: application/json-rpc

{"jsonrpc":"2.0","method":"add","params":["laojiu",18],"id":1}
```