## WebApiClient 　　　　　　　　　　　　　　　　　　　　[English](https://github.com/dotnetcore/WebApiClient/blob/master/README.en.md)
### 1 Nuget包
PM> `install-package WebApiClient.JIT`
<br/>支持 .net framework4.5 netstandard1.3 netcoreapp2.1 

PM> `install-package WebApiClient.AOT` 
<br/>支持 .net framework4.5 netstandard1.3 netcoreapp2.1

#### 1.1 WebApiClient.JIT
在运行时使用Emit创建Http请求接口的代理类，HttpApiClient.Create<TInterface>()时，在新的程序集创建了TInterface的代理类，类名与TInterface相同，命名空间也相同，由于代理类和TInterface接口不在同一程序集，所以要求TInterface为public。

* 可以在项目中直接引用WebApiClient.JIT.dll就能使用；
* 不适用于不支持JIT技术的平台(IOS、UWP)；
* 接口要求为public；

#### 1.2 WebApiClient.AOT
在编译过程中使用Mono.Cecil修改编译得到的程序集，向其插入Http请求接口的代理类IL指令，这一步是在AOT编译阶段之前完成。代理类型所在的程序集、模块、命名空间与接口类型的一样，其名称为$前缀的接口类型名称，使用反编译工具查看项目编译后的程序集可以看到这些代理类。

* 项目必须使用nuget安装WebApiClient.AOT才能正常使用；
* 没有JIT，支持的平台广泛；
* 接口不要求为public，可以嵌套在类里面；

### 2. Http(s)请求
#### 2.1 接口的声明
```c#
[HttpHost("http://www.webapiclient.com")] 
public interface IMyWebApi : IHttpApi
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
 
#### 2.2 接口的调用
```c#
static async Task TestAsync()
{
    var client = HttpApiClient.Create<IMyWebApi>();
    var user = new UserInfo { Account = "laojiu", Password = "123456" }; 
    var user1 = await client.GetUserByAccountAsync("laojiu");
    var user2 = await client.UpdateUserWithFormAsync(user);
}
``` 

#### 3. 功能特性
* 面向切面编程方式
* 内置丰富的接口、方法和参数特性，支持使用自定义特性
* 适应个性化需求的多个DataAnnotations特性
* 灵活的ApiAcitonFilter、GobalFilter和IParameterable
* 支持与外部HttpMessageHandler实例无缝衔接
* 独一无二的请求异常条件重试(Retry)和异常处理(Handle)链式语法功能

#### 4. 详细文档
* [WebApiClient基础](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E5%9F%BA%E7%A1%80)
* [WebApiClient进阶](https://github.com/dotnetcore/WebApiClient/wiki/WebApiClient%E8%BF%9B%E9%98%B6)
* [WebApiClient高级](https://github.com/xljiulang/WebApiClient/wiki/WebApiClient%E9%AB%98%E7%BA%A7)

#### 5. 联系方式
* 加群439800853 注明WeApiClient
* 366193849@qq.com，不重要的尽量不要发

#### 6. 功能视图
![功能脑图](https://raw.githubusercontent.com/dotnetcore/WebApiClient/master/WebApiClient.png)
