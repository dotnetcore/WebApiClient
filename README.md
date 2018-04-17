## 1. [Nuget](https://www.nuget.org/packages/Laojiu.WebApiClient/)
PM> `install-package Laojiu.WebApiClient`
<br/>支持 `.net framework4.5` `netstandard2.0` `netcoreapp2.0` `netcoreapp2.1`

## 2. Http(s)请求
### 2.1 接口的声明
```c#
[HttpHost("http://www.webapiclient.com")] 
public interface IMyWebApi : IHttpApiClient
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
 
### 2.2 接口的调用
```c#
static async Task TestAsync()
{
    var client = HttpApiClient.Create<IMyWebApi>();
    var user = new UserInfo { Account = "laojiu", Password = "123456" }; 
    var user1 = await client.GetUserByAccountAsync("laojiu");
    var user2 = await client.UpdateUserWithFormAsync(user);
}
``` 

### 3. 功能特性
* 面向切面编程方式
* 内置丰富的接口、方法和参数特性，支持使用自定义特性
* 适应个性化需求的多个DataAnnotations特性
* 灵活的ApiAcitonFilter、GobalFilter和IParameterable
* 支持与外部HttpMessageHandler实例无缝衔接
* 独一无二的请求异常条件重试(Retry)和异常处理(Handle)链式语法功能

### 4. 详细文档
* [WebApiClient基础](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80)
* [WebApiClient中级](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E4%B8%AD%E7%BA%A7)
* [WebApiClient高级](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E9%AB%98%E7%BA%A7)

### 5. 联系方式
* 加群439800853 注明WeApiClient
* 366193849@qq.com，不重要的尽量不要发



## License
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2Fdotnetcore%2FWebApiClient.svg?type=shield)](https://app.fossa.io/projects/git%2Bgithub.com%2Fdotnetcore%2FWebApiClient?ref=badge_shield)

[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2Fdotnetcore%2FWebApiClient.svg?type=large)](https://app.fossa.io/projects/git%2Bgithub.com%2Fdotnetcore%2FWebApiClient?ref=badge_large)
