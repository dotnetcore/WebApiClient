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
* HttpContent类型的参数值，直接作为请求的内容体，如StringContent、FormUrlEncodedContent等
* IApiParameterable或其集合类型的参数值，不需要添加特性修饰，如MulitpartFile、Url和Proxy类
* 无任何特性修饰的且不属于以上两种类型的参数值，将自动关联[PathQueryAttribute]

### 扩展
* 派生IApiActionAttribute，实现Api请求前的逻辑处理
* 派生IApiActionFilterAttribute，实现Api请求前或请求后的逻辑处理
* 派生IApiParameterAttribute或IApiParameterable，实现Api参数的逻辑处理
* 派生IApiReturnAttribute，实现更多的回复内容处理的功能

### 执行顺序
IApiActionAttribute > IApiParameterAttribute> IApiReturnAttribute
