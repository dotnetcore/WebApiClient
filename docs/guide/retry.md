# 请求条件性重试

使用ITask<>异步声明，就有Retry的扩展，Retry的条件可以为捕获到某种Exception或响应模型符合某种条件。

```csharp
public interface IUserApi
{
    [HttpGet("api/users/{id}")]
    ITask<User> GetAsync(string id);
}

var result = await userApi.GetAsync(id: "id001")
    .Retry(maxCount: 3)
    .WhenCatch<HttpRequestException>()
    .WhenResult(r => r.Age <= 0);
```
