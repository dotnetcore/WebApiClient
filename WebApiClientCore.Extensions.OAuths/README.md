## WebApiClientCore.Extensions.OAuths 　　　　　　　　　　　　　　　　　　　
WebApiClientCore的OAuth功能扩展，支持client_credentials与password授权模式
 
### 使用方式
#### 注册Token提供者
```
// 为接口注册与配置token提者选项
services.AddClientCredentialsTokenProvider<IUserApi>(o =>
{
    o.Endpoint = new Uri("http://localhost:6000/api/tokens");
    o.Credentials.Client_id = "clientId";
    o.Credentials.Client_secret = "xxyyzz";
});
```
 
#### 应用对应的Token特性
```
/// <summary>
/// 用户操作接口
/// </summary>
[ClientCredentialsToken]
public interface IUserApi : IHttpApi
{
    ...
}
```