# WebApiClient
基于.Net45的HttpClient，只需定义http api的接口并打上特性[Attribute]，即可以异步调用http api的框架

## 如何使用
### Api声明
```
[Logger] // 记录请求日志
[HttpHost("http://www.mywebapi.com")] // 可以在Config设置
public interface MyWebApi : IDisposable
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
    var myWebApi = HttpApiClient.Create<UserApi>();
    var user = new UserInfo { Account = "laojiu", Password = "123456" }; 
    var user1 = await myWebApi.GetUserByAccountAsync("laojiu");
    var user2 = await myWebApi.UpdateUserWithFormAsync(user);
}
```

## 支持与约束
* 支持接口继承或多继承
* 支持泛型接口、泛型方法
* 支持非泛型接口、非泛型方法
* 约束接口只能定义方法
* 约束接口的参数不能为ref/out
* 约束接口的返回类型必须是TaskOf(TResult)

## 功能列表 
### 接口级特性
* 绝对主机域名：[HttpHost]
* 请求头：[Header]
* 返回值：[AutoReturn]、[JsonReturn]、[XmlReturn]
* 代理：[Proxy]
* 自定义IApiActionAttribute特性或IApiReturnAttribute特性

### 方法级特性
* 绝对主机域名：[HttpHost]
* 请求头：[Header]
* 返回值：[AutoReturn]、[JsonReturn]、[XmlReturn]
* 请求方式+路径：[HttpGet]、[HttpPost]、[HttpDelete]、[HttpPut]、[HttpHead]和[HttpOptions]
* 自定义IApiActionAttribute特性或IApiReturnAttribute特性

### 参数级特性
* 请求头：[Header]
* Path/Query：[PathQuery]
* 请求URL：[Url]
* 请求Body：[HttpContent]、[JsonContent]、[XmlContent]、[FormContent]、[MulitpartContent]
* 自定义IApiParameterAttribute特性

### 特殊参数类型
* MulitpartFile类(表单文件)
* 自定义IApiParameterable类

## 配置与扩展
### 配置
```
var config = new HttpApiConfig
{
    HttpHost = ...                
    JsonFormatter = ...
    XmlFormatter = ...
    ...
};
var yourApi = HttpApiClient.Create<YourApi>(config);
```

### 扩展
* 派生IApiActionAttribute或ApiActionAttribute，实现Api请求前的逻辑处理
* 派生IApiParameterAttribute或IApiParameterable，实现Api参数的逻辑处理
* 派生IApiActionFilterAttribute或ApiActionFilterAttribute，实现Api请求前或请求后的逻辑处理
* 派生IApiReturnAttribute或ApiReturnAttribute，实现更多的回复内容处理的功能
