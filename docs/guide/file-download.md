# 文件下载

```csharp
public interface IUserApi
{
    [HttpGet("/files/{fileName}"]
    Task<HttpResponseMessage> DownloadAsync(string fileName);
}
```

```csharp
using System.Net.Http

var response = await userApi.DownloadAsync('123.zip');
using var fileStream = File.OpenWrite("123.zip");
await response.SaveAsAsync(fileStream);
```
