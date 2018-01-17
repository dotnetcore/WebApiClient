## 1. [Nuget](https://www.nuget.org/packages/Laojiu.WebApiClient/)
PM> `install-package Laojiu.WebApiClient`

## 2. WebApiClient是什么
### 2.1 接口的声明
```c#
[HttpHost("http://www.webapiclient.com")] 
public interface MyWebApi : IDisposable
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
    var myWebApi = HttpApiClient.Create<MyWebApi>();
    var user = new UserInfo { Account = "laojiu", Password = "123456" }; 
    var user1 = await myWebApi.GetUserByAccountAsync("laojiu");
    var user2 = await myWebApi.UpdateUserWithFormAsync(user);
}
``` 

### 3. 详细文档
* [WebApiClient基础](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80)
* [WebApiClient中级](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E4%B8%AD%E7%BA%A7)
* [WebApiClient高级](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E9%AB%98%E7%BA%A7)

### 4. 怎么联系我
* 加群439800853 注明WeApiClient
* 366193849@qq.com，不重要的尽量不要发

