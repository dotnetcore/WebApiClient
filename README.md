# WebApiClient
基于.Net45的HttpClient，只需定义http api的接口并打上特性[Attribute]，即可以异步调用http api的框架

## 如何使用
### Api声明
```
[Logger] // 记录请求日志
[HttpHost("http://www.mywebapi.com")] // 可以在Implement传Url覆盖
public interface MyWebApi
{
    // GET webapi/user?account=laojiu
    // Return 原始string内容
    [HttpGet("/webapi/user")]
    Task<string> GetUserByAccountAsync(string account);

    // POST webapi/user  
    // Body Account=laojiu&Password=123456
    // Return json或xml内容
    [HttpPost("/webapi/user")]
    Task<UserInfo> UpdateUserWithFormAsync([FormContent] UserInfo user);
}
```
 
### Api调用
 ```
static async Task TestAsync()
{
    var webApiClient = new HttpApiClient();
    var myWebApi = webApiClient.Implement<MyWebApi>();
    var user = new UserInfo { Account = "laojiu", Password = "123456" }; 
    var user1 = await myWebApi.GetUserByAccountAsync("laojiu");
    var user2 = await myWebApi.UpdateUserWithFormAsync(user);
}
```

## 功能列表 
### 方法或接口级特性
* 绝对主机域名：[HttpHost]
* 请求方式与路径：[HttpGet]、[HttpPost]、[HttpDelete]、[HttpPut]、[HttpHead]和[HttpOptions]
* 代理：[Proxy]
* 请求头：[Header]
* 返回值：[AutoReturn]、[JsonReturn]、[XmlReturn]
* 自定义IApiActionAttribute特性或IApiReturnAttribute特性

### 参数级特性
* 路径或query：[PathQuery]、[Url]
* 请求头：[Header]
* 请求Body：[HttpContent]、[JsonContent]、[XmlContent]、[FormContent]、[MulitpartConten]
* 自定义IApiParameterAttribute特性

### 特殊参数类型</br>
* MulitpartFile类(表单文件)</br>
* Url类(请求地址)</br>
* Proxy类 (请求代理)</br>
* 自定义IApiParameterable类 </br>

## 配置与扩展
### 配置
* HttpApiClient.Config.UseXmlFormatter(your formatter)
* HttpApiClient.Config.UseJsonFormatter(your formatter)
* HttpApiClient.Config.UseHttpClientContextProvider(your provider)

### 扩展
* 派生IApiActionAttribute，实现Api请求前的逻辑处理
* 派生IApiActionFilterAttribute，实现Api请求前或请求后的逻辑处理
* 派生IApiParameterAttribute或IApiParameterable，实现Api参数的逻辑处理
* 派生IApiReturnAttribute，实现更多的回复内容处理的功能

### 执行顺序
IApiActionAttribute > IApiParameterAttribute、IApiParameterable > IApiActionFilterAttribute > IApiReturnAttribute

