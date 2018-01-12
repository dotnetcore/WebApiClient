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
    ITask<string> GetUserByAccountAsync(string account);

    // POST webapi/user  
    // Body Account=laojiu&Password=123456
    // Return json或xml内容
    [HttpPost("/webapi/user")]
    ITask<UserInfo> UpdateUserWithFormAsync([FormContent] UserInfo user);
}

public class UserInfo
{
    public string Account { get; set; }

    [AliasAs("password")]
    public string Password { get; set; }

    [DateTimeFormat("yyyy-MM-dd")]
    public DateTime? BirthDay { get; set; }

    [IgnoreSerialized]
    public string Email { get; set; }

    public override string ToString()
    {
        return string.Format("{{Account:{0}, Password:{1}, BirthDay:{2}}}", this.Account, this.Password, this.BirthDay);
    }
}
```
 
### Api调用
```
static async Task TestAsync()
{
    var myWebApi = HttpApiClient.Create<MyWebApi>();
    var user = new UserInfo { Account = "laojiu", Password = "123456" }; 
    var user1 = await myWebApi.GetUserByAccountAsync("laojiu");
    var user2 = await myWebApi.UpdateUserWithFormAsync(user);
}
```

## Nuget
PM> `install-package Laojiu.WebApiClient`

## 支持与约束
* 支持接口继承或多继承
* 支持泛型接口
* 约束接口只能定义方法
* 约束接口的参数不能为ref/out
* 约束接口的返回类型必须是TaskOf(TResult)或ITaskOf(TResult)

## 功能列表 
### 接口级特性
* 绝对主机域名：[HttpHost]
* 请求头：[Header]
* 返回值：[AutoReturn]、[JsonReturn]、[XmlReturn]
* 代理：[Proxy]
* 请求Body：[FormField]、[MulitpartText]
* 自定义IApiActionAttribute特性或IApiReturnAttribute特性

### 方法级特性
* 绝对主机域名：[HttpHost]
* 请求头：[Header]
* 返回值：[AutoReturn]、[JsonReturn]、[XmlReturn]
* 请求方式+路径：[HttpGet]、[HttpPost]、[HttpDelete]、[HttpPut]、[HttpHead]和[HttpOptions]
* 请求Body：[FormField]、[MulitpartText]
* 自定义IApiActionAttribute特性或IApiReturnAttribute特性

### 参数级特性
* 请求头：[Header]
* Path/Query：[PathQuery]
* 请求URL：[Url]
* 请求Body：[HttpContent]、[JsonContent]、[XmlContent]、[FormContent]、[MulitpartContent]、[FormField]、[MulitpartText]
* 自定义IApiParameterAttribute特性

### 特殊参数类型
* FormField(表单字段)
* MulitpartFile类(表单文件)
* MulitpartText类(表单文本)
* BasicAuth(基本身份)
* 自定义IApiParameterable类

## 异常的Retry与Handle
* ITask类型支持Retry功能，当某种异常时，请求重试
* ITask类型支持Handle功能，当某种异常时，将结果处理为目标值
```
var user = await myWebApi.UpdateWithFormAsync(user, nickName: "老九", age: 18)
    .Retry(3, i => TimeSpan.FromSeconds(i))
    .WhenCatch<TimeoutException>()
    .Handle()
    .WhenCatch<RetryException>(ex => new UserInfo { Account = "RetryException" })
    .WhenCatch<Exception>(ex => new UserInfo { Account = "Exception" });
```

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
