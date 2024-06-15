# PATCH请求

json patch是为客户端能够局部更新服务端已存在的资源而设计的一种标准交互，在RFC6902里有详细的介绍json patch，通俗来讲有以下几个要点：

使用HTTP PATCH请求方法；
请求body为描述多个opration的数据json内容；
请求的Content-Type为application/json-patch+json；

## WebApiClient例子

```csharp
public interface IMyWebApi : IHttpApi
{
    [HttpPatch("webapi/user")]
    Task<UserInfo> PatchAsync(string id, JsonPatchDocument<UserInfo> doc);
}

var doc = new JsonPatchDocument<UserInfo>();
doc.Replace(item => item.Account, "laojiu");
doc.Replace(item => item.Email, "laojiu@qq.com");
var api = HttpApi.Create<IMyWebApi>();
await api.PatchAsync("id001", doc);
```

## Asp.net 服务端例子

```csharp
[HttpPatch]
public async Task<UserInfo> Patch(string id, [FromBody] JsonPatchDocument<UserInfo> doc)
{
    // 此处user是从db查询获得
    var user = await GetUserInfoFromDbAsync(id);
    doc.ApplyTo(user);
    return user;
}
```
