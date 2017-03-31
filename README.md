# WebApiClient
一种类似Retrofit声明接口即可实现调用的WebApi客户端框架

### Api声明
```
namespace Demo
{
    [JsonReturn]
    [HttpHost("http://www.mywebapi.com")]
    public interface MyWebApi
    {
        [HttpGet("/webapi/{type}/about")] // GET webapi/typeValue/about
        Task<ApiResult<string>> GetAboutAsync(string type);


        [HttpGet("/webapi/user")]  // GET webapi/user?userName=aa&nickName=bb&BeginTime=cc&EndTime=dd
        Task<ApiResult<UserInfo>> GetUserAsync(string userName, string nickName, TimeFilter timeFilter);


        [HttpPut("/webapi/user")] // PUT webapi/user
        Task<ApiResult<bool>> UpdateUserAsync([JsonContent] UserInfo loginInfo);


        [HttpDelete("/webapi/user")] // DELETE  webapi/user?id=idValue
        Task<ApiResult<bool>> DeleteUserAsync(string id);


        [HttpDelete("/webapi/user/{id}")] // DELETE  webapi/user/idValue
        Task<ApiResult<bool>> DeleteUser2Async(string id);
    }
}
```
 
 ### Api调用
 ```
namespace Demo
{
    class Program
    {
        static async void Test()
        {
            var myWebApi = new WebApiClient.HttpApiClient().GetHttpApi<MyWebApi>();

            await myWebApi.GetAboutAsync("typeValue");
            await myWebApi.UpdateUserAsync(new UserInfo { UserName = "abc", Password = "123456" });
            await myWebApi.DeleteUser2Async(id: "id001");
        }
    }
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

### 执行顺序
ApiActionAttribute > ApiParameterAttribute > ApiReturnAttribute
