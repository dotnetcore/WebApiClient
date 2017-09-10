# WebApiClient
一种类似Retrofit声明接口即可实现调用的WebApi客户端框架

### Api声明
```
[Logger] // 记录请求日志
[JsonReturn] // 默认返回内容为Json
[HttpHost("http://www.mywebapi.com")] // 可以在Implement传Url覆盖
public interface MyWebApi
{
    // GET webapi/user/id001
    // Return json内容
    [HttpGet("/webapi/user/{id}")]
    Task<HttpResponseMessage> GetUserByIdAsync(string id);

    // GET webapi/user?account=laojiu
    // Return json内容
    [HttpGet("/webapi/user")]
    Task<string> GetUserByAccountAsync(string account);


    // POST webapi/user  
    // Body:Account=laojiu&Password=123456
    // Return json内容
    [HttpPost("/webapi/user")]
    Task<UserInfo> UpdateUserWithFormAsync([FormContent] UserInfo user);

    // POST webapi/user   
    // Body:{"Account":"laojiu","Password":"123456"}
    // Return json内容
    [HttpPost("/webapi/user")]
    Task<UserInfo> UpdateUserWithJsonAsync([JsonContent] UserInfo user);

    // POST webapi/user   
    // Body: xml内容
    // Return xml内容
    [XmlReturn]
    [HttpPost("/webapi/user")]
    Task<UserInfo> UpdateUserWithXmlAsync([XmlContent] UserInfo user);
}
```
 
 ### Api调用
 ```
static async Task TestAsync()
{
    var webApiClient = new WebApiClient.HttpApiClient();
    var myWebApi = webApiClient.Implement<MyWebApi>();
    var user = new UserInfo { Account = "laojiu", Password = "123456" };

    var user1 = await myWebApi.GetUserByIdAsync("id001");
    var user2 = await myWebApi.GetUserByIdAsync("laojiu");

    await myWebApi.UpdateUserWithFormAsync(user);
    await myWebApi.UpdateUserWithJsonAsync(user);
    await myWebApi.UpdateUserWithXmlAsync(user);
}
```

### 说明
* 派生HttpContent类型的参数，都自动当作请求的内容
* JsonContentAttribute表示将参数体作为application/json请求
* FormContentAttribute表示将参数体作为x-www-form-urlencoded请求
* PathQueryAttribute表示Url路径参数或query参数，不需要显示声明
* HttpHostAttribute可以不声明，而是从GetHttpApi<ApiInterface>(host)里传
* 可以使用DefaultReturnAttribute替换JsonReturnAttribute，自己接管回复内容

### 扩展
* 派生ApiReturnAttribute扩展回复内容处理
* 派生ApiParameterAttribute扩展参数处理
* 派生ApiActionAttribute扩展方法逻辑处理
* 派生ApiActionFilterAttribute扩展方法请求前后拦截
### 执行顺序
ApiActionAttribute > ApiParameterAttribute > ApiReturnAttribute
