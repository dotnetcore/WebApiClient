## WebApiClientCore.Extensions.NewtonsoftJson　　　　　　　　　　　　　　　　　　
WebApiClientCore的NewtonsoftJson序列化扩展

### 适用场景
* 熟悉NewtonsoftJson，不想使用System.Text.Json的项目；
* 喜欢NewtonsoftJson的动态类型，且性能不敏感的项目；
 
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
使用[JsonNetReturn]替换内置的[JsonReturn]，[JsonNetContent]替换内置[JsonContent]
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