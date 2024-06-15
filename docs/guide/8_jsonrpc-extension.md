# JsonRpc 扩展

在极少数场景中，开发者可能遇到 JsonRpc 调用的接口，由于该协议不是很流行，WebApiClientCore 将该功能的支持作为 WebApiClientCore.Extensions.JsonRpc 扩展包提供。使用[JsonRpcMethod]修饰 Rpc 方法，使用[JsonRpcParam]修饰 Rpc 参数
即可。

## JsonRpc 声明

```csharp
[HttpHost("http://localhost:5000/jsonrpc")]
public interface IUserApi
{
    [JsonRpcMethod("add")]
    ITask<JsonRpcResult<User>> AddAsync([JsonRpcParam] string name, [JsonRpcParam] int age, CancellationToken token = default);
}
```

## JsonRpc 数据包

```log

POST /jsonrpc HTTP/1.1
Host: localhost:5000
User-Agent: WebApiClientCore/1.0.6.0
Accept: application/json; q=0.01, application/xml; q=0.01
Content-Type: application/json-rpc

{"jsonrpc":"2.0","method":"add","params":["laojiu",18],"id":1}
```
