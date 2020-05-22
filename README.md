## WebApiClientCore 　　　　　　　　　　　　　　　　　　　
[WebApiClient.JIT](https://github.com/dotnetcore/WebApiClient/tree/WebApiClient.JITAOT)的.netcore版本，基于HttpClient的高性能与高可扩展性于一体的声明式Http客户端库，特别适用于微服务的restful资源请求，也适用于各种非标准的http接口请求。

### PackageReference

    <PackageReference Include="WebApiClientCore" Version="1.0.0-beta1" />

 
### 项目原因
 
1. WebApiClient很优秀，它将不同框架不同平台都实现了统一的api
2. WebApiClient不够优秀，它在.netcore下完全可以更好，但它不得不兼容.net45开始所有框架而有所牺牲


### 相对变化
* 使用System.Text.Json替换Json.net，提升序列化性能
* 移除HttpApiFactory和HttApiConfig功能，使用Microsoft.Extensions.Http的HttpClientFactory
* 移除AOT功能，仅保留依赖于Emit的运行时代理
* 高效的ActionInvoker，对返回Task<>和ITask<>作不同处理
* 所有特性都都变成中间件，基于管道编排各个特性并生成Action执行委托
* 良好设计的HttpContext、ApiRequestContext、ApiParameterContext和ApiResponseContext

### Benchmark
> WebApiClientCore、WebApiClient.JIT与原生HttpClient的性能比较相比原生HttpClient，WebApiClientCore几乎没有性能损耗。

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
* 支持Task、Task<>和ITask<>三种异步返回
* 支持HttpResponseMessage、byte[]、string和Stream原生类型返回内容
* 支持模型自动转换为Xml、Json、Form、和FormData共4种请求格式的内容
* 支持原生HttpContent(比如StringContent)类型直接做为请求参数
* 支持自定义IApiParameter类型直接作为就参数
* 内置丰富的能满足各种环境的常用特性，支持用户自定义特性

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

### 服务注册与获取
> 服务注册

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

> 服务获取

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

### 请求和响应日志
在整个Interface或某个Method上声明[LoggingFilter]，即可把请求和响应的内容输出到LoggingFactory中。

如果要排除某个Method不打印日志（比如大流量传输接口），在方法上声明[LoggingFilter(Enable = false)]，即可将本Method排除。

### Accpet ContentType
这个用于控制客户端希望服务器返回什么样的内容格式，比如json或xml，默认的配置值是Accept: application/json; q=0.01, application/xml; q=0.01

如果想json优先，可以在Interface或Method上声明[JsonReturn]，请求变为Accept: application/json, application/xml; q=0.01

如果想禁用其中一种，比如禁用xml，可以在Interface或Method上声明[XmlReturn(Enable = false)]，请求变为Accept: application/json; q=0.01


### 请求条件重试
使用ITask<>异步声明，就有Retry的扩展，Retry的条件可以为捕获到某种Exception或响应模型符合某种条件。

```
var result = await youApi.GetModelAsync(id: "id001")
    .Retry(maxCount: 3)
    .WhenCatch<Exception>()
    .WhenResult(r => r.ErrorCode > 0);
```



### OAuths&Token
使用`WebApiClientCore.Extensions.OAuths`扩展，轻松支持token的获取、刷新与应用

### 1 注册相应类型的TokenProvider

```
// 为接口注册与配置token提者选项
services.AddClientCredentialsTokenProvider<IpetApi>(o =>
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
public interface IpetApi
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
public interface IpetApi
{
    ...
}
```

