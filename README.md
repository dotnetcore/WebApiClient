# WebApiClient
一种类似Retrofit声明接口即可实现调用的WebApi客户端框架

### Api声明
```
[Logger] // 记录请求日志
[HttpHost("http://www.mywebapi.com")] // 可以在Implement传Url覆盖
public interface MyWebApi
{
    // GET webapi/user/id001
    // Return HttpResponseMessage
    [HttpGet("/webapi/user/{id}")]
    Task<HttpResponseMessage> GetUserByIdAsync(string id);

    // GET webapi/user?account=laojiu
    // Return 原始string内容
    [HttpGet("/webapi/user")]
    Task<string> GetUserByAccountAsync(string account);


    // POST webapi/user  
    // Body Account=laojiu&Password=123456
    // Return json或xml内容
    [HttpPost("/webapi/user")]
    Task<UserInfo> UpdateUserWithFormAsync([FormContent] UserInfo user);

    // POST webapi/user   
    // Body {"Account":"laojiu","Password":"123456"}
    // Return json或xml内容
    [HttpPost("/webapi/user")]
    Task<UserInfo> UpdateUserWithJsonAsync([JsonContent] UserInfo user);

    // POST webapi/user   
    // Body <?xml version="1.0" encoding="utf-8"?><UserInfo><Account>laojiu</Account><Password>123456</Password></UserInfo>
    // Return xml内容
    [XmlReturn]
    [HttpPost("/webapi/user")]
    Task<UserInfo> UpdateUserWithXmlAsync([XmlContent] UserInfo user);

    // POST webapi/user   
    // Body multipart/form-data
    // Return json或xml内容
    [HttpPost("/webapi/user")]
    Task<UserInfo> UpdateUserWithMulitpartAsync([MulitpartContent] UserInfo user, params MulitpartFile[] files);
}
```
 
 ### Api调用
 ```
static async Task TestAsync()
{
    var webApiClient = new HttpApiClient();
    var myWebApi = webApiClient.Implement<MyWebApi>();
    var user = new UserInfo { Account = "laojiu", Password = "123456" };
    var file = new MulitpartFile("head.jpg");

    var user1 = await myWebApi.GetUserByIdAsync("id001");
    var user2 = await myWebApi.GetUserByAccountAsync("laojiu");

    await myWebApi.UpdateUserWithFormAsync(user);
    await myWebApi.UpdateUserWithJsonAsync(user);
    await myWebApi.UpdateUserWithXmlAsync(user);
    await myWebApi.UpdateUserWithMulitpartAsync(user, file);
}
```

### 说明
* 派生HttpContent类型的参数值，都自动当作请求的内容
* PathQueryAttribute表示Url路径参数或query参数，不需要显示声明

### 扩展
* 派生ApiParameterAttribute，实现更多参数处理的功能
* 派生ApiReturnAttribute，实现更多的回复内容处理的功能
* 派生ApiActionAttribute，实现更多的请求前后逻辑处理的功能
* 派生ApiActionFilterAttribute，实现请求前后拦截的功能

### 执行顺序
ApiActionAttribute > ApiParameterAttribute > ApiReturnAttribute
