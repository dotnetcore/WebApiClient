## WebApiClientCore.Extensions.NewtonsoftJson　　　　　　　　　　　　　　　　　　
WebApiClientCore的NewtonsoftJson序列化扩展
 
### 使用方式
#### 配置json.net[可选]
```
// ConfigureNewtonsoftJson
services.AddHttpApi<IUserApi>().ConfigureNewtonsoftJson(o =>
{
    o.JsonSerializeOptions.NullValueHandling = NullValueHandling.Ignore;
});
```
 
#### 声明特性
```
/// <summary>
/// 用户操作接口
/// </summary>
[JsonNetReturn]
public interface IUserApi : IHttpApi
{
    [HttpPost("/users")]
    Task PostAsync([JsonNetContent] User user);
}
```