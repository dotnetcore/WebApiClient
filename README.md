## Nuget
PM> `install-package Laojiu.WebApiClient`

## WebApiClient是什么
### 1. 接口的声明
```
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
 
### 2. 接口的调用
```
static async Task TestAsync()
{
    var myWebApi = HttpApiClient.Create<MyWebApi>();
    var user = new UserInfo { Account = "laojiu", Password = "123456" }; 
    var user1 = await myWebApi.GetUserByAccountAsync("laojiu");
    var user2 = await myWebApi.UpdateUserWithFormAsync(user);
}
``` 

### 详细文档
* [WebApiClient基础](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80)
* [WebApiClient中级](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E4%B8%AD%E7%BA%A7)
