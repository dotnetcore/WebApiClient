## WebApiClient
WebApiClient.JIT will replace the old Laojiu.WebApiClient, and use Emit to create the proxy class of http request interface at runtime.<br/>
WebApiClient.AOT supports all platforms, including the platform that requires AOT, inserting proxy class IL instructions from http request interface to output assembly at compile time.<br/>

### 1 Nuget
PM> `install-package WebApiClient.JIT`
<br/>supports .net framework4.5 netstandard1.3 netcoreapp2.1 

PM> `install-package WebApiClient.AOT` 
<br/>supports .net framework4.5 netstandard1.3 netcoreapp2.1

### 2. Http(s) request
#### 2.1 Interface declaration
```c#
[HttpHost("http://www.webapiclient.com")] 
public interface IMyWebApi : IHttpApi
{
    // GET webapi/user?account=laojiu
    // Return original string
    [HttpGet("/webapi/user")]
    ITask<string> GetUserByAccountAsync(string account);

    // POST webapi/user  
    // Body Account=laojiu&password=123456
    // Return json or xml content
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
 
#### 2.2 Call request
```c#
static async Task TestAsync()
{
    var client = HttpApiClient.Create<IMyWebApi>();
    var user = new UserInfo { Account = "laojiu", Password = "123456" }; 
    var user1 = await client.GetUserByAccountAsync("laojiu");
    var user2 = await client.UpdateUserWithFormAsync(user);
}
``` 
 

#### 3. Documents
* [WebApiClient basis](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80)
* [WebApiClient advance](https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E8%BF%9B%E9%98%B6)
* [WebApiClient senior](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E9%AB%98%E7%BA%A7)
 

#### 4. Features
![](https://raw.githubusercontent.com/dotnetcore/WebApiClient/master/WebApiClient.png)
