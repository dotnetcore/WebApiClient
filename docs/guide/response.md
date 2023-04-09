# 响应处理

## 缺省配置值

缺省配置是[JsonReturn(0.01),XmlReturn(0.01)]，对应的请求accept值是
`Accept: application/json; q=0.01, application/xml; q=0.01`

## Json优先

在Interface或Method上显式地声明`[JsonReturn]`，请求accept变为`Accept: application/json, application/xml; q=0.01`

## 禁用json

在Interface或Method上声明`[JsonReturn(Enable = false)]`，请求变为`Accept: application/xml; q=0.01`

## 原始类型返回值

当接口返回值声明为如下类型时，我们称之为原始类型，会被RawReturnAttribute处理。

返回类型 | 说明
---|---
`Task` | 不关注响应消息
`Task<HttpResponseMessage>` | 原始响应消息类型
`Task<Stream>` | 原始响应流
`Task<byte[]>` | 原始响应二进制数据
`Task<string>` | 原始响应消息文本

## 响应内容缓存

配置CacheAttribute特性的Method会将本次的响应内容缓存起来，下一次如果符合预期条件的话，就不会再请求到远程服务器，而是从IResponseCacheProvider获取缓存内容，开发者可以自己实现ResponseCacheProvider。

### 声明缓存特性

```csharp
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

```csharp
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

### 自定义缓存提供者

默认的缓存提供者为内存缓存，如果希望将缓存保存到其它存储位置，则需要自定义 缓存提者，并注册替换默认的缓存提供者。

```csharp
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
```

```csharp
public static IWebApiClientBuilder UseRedisResponseCacheProvider(this IWebApiClientBuilder builder)
{
    builder.Services.AddSingleton<IResponseCacheProvider, RedisResponseCacheProvider>();
    return builder;
}
```
