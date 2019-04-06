## WebApiClient 　　　　　　　　　　　　　　　　　　　
### 1 Nuget包

> WebApiClient.JIT

    PM> install-package WebApiClient.JIT
* 可以在项目中直接引用WebApiClient.JIT.dll就能使用；
* 不适用于不支持JIT技术的平台(IOS、UWP)；
* 接口要求为public；


> WebApiClient.AOT

    PM> install-package WebApiClient.AOT
* 项目必须使用nuget安装WebApiClient.AOT才能正常使用；
* 没有JIT，支持的平台广泛；
* 接口不要求为public，可以嵌套在类里面；



### 2. Http请求
> 接口的声明

```c#
[HttpHost("http://www.webapiclient.com")] 
public interface IMyWebApi : IHttpApi
{
    // GET webapi/user?account=laojiu
    // Return 原始string内容
    [HttpGet("/webapi/user")]
    ITask<string> GetUserByAccountAsync(string account);

    // POST webapi/user  
    // Body Account=laojiu&password=123456
    // Return json或xml内容
    [HttpPost("/webapi/user")]
    ITask<UserInfo> UpdateUserWithFormAsync([FormContent] UserInfo user);
}

public class UserInfo
{
    public string Account { get; set; }

    [AliasAs("password")]
    public string Password { get; set; }

    [IgnoreSerialized]
    public string Email { get; set; }
}
```
 
> 接口的调用

```c#
var api = HttpApi.Create<IMyWebApi>();
var user = new UserInfo { Account = "laojiu", Password = "123456" }; 
var user1 = await api.GetUserByAccountAsync("laojiu");
var user2 = await api.UpdateUserWithFormAsync(user);
``` 

#### 3. Api变化
> 相对于[v0.3.6](https://github.com/dotnetcore/WebApiClient/tree/v0.3.6)或以前版本，Api有如下变化 

* ~~HttpApiClient.Create()~~ -> HttpApi.Create()
* ~~HttpApiFactory.Add()~~ -> HttpApi.Register()
* ~~HttpApiFactory.Create()~~ -> HttpApi.Resolve()


#### 4. Wiki文档
1. [WebApiClient基础](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80)
2. [WebApiClient进阶](https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E8%BF%9B%E9%98%B6)
3. [WebApiClient高级](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E9%AB%98%E7%BA%A7)
4. [WebApiClient.Extensions](https://github.com/xljiulang/WebApiClient.Extensions)
5. [WebApiClient.Tools.Swagger](https://github.com/xljiulang/WebApiClient.Tools)

#### 5. 联系方式
1. 加Q群825135345 注明WeApiClient
2. 邮箱366193849@qq.com，不重要的尽量不要发

