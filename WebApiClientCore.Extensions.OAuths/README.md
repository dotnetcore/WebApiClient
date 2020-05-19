## WebApiClientCore.Extensions.OAuths 　　　　　　　　　　　　　　　　　　　
WebApiClientCore的OAuth功能扩展，支持client_credentials与password授权模式
 
### 使用方式
#### 注册Token提供者
```
// 注册与配置token提者选项
services.AddClientCredentialsTokenProvider<IUserApi>(o =>
{
    o.Endpoint = new Uri("http://localhost:6000/api/tokens");
    o.Credentials.Client_id = "clientId";
    o.Credentials.Client_secret = "xxyyzz";
});
```

#### 实现OAuthTokenAttribute的自定义子类
```
/// <summary>
/// token获取与应用过滤器
/// </summary>
class UserTokenAttribute : OAuthTokenAttribute
{
    /// <summary>
    /// 获取token提供者
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    protected override TokenProvider GetTokenProvider(ApiRequestContext context)
    {
        return context.HttpContext.Services.GetRequiredService<ClientCredentialsTokenProvider<IUserApi>>();
    } 
}
```


#### 应用自定义子类
```
/// <summary>
/// 用户操作接口
/// </summary>
[UserToken] 
public interface IUserApi : IHttpApi
{
    ...
}
```