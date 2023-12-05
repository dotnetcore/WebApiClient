
# JsonRpc调用

在极少数场景中，开发者可能遇到JsonRpc调用的接口，由于该协议不是很流行，WebApiClientCore将该功能的支持作为WebApiClientCore.Extensions.JsonRpc扩展包提供。使用[JsonRpcMethod]修饰Rpc方法，使用[JsonRpcParam]修饰Rpc参数
即可。

## JsonRpc声明

```csharp
[HttpHost("http://localhost:5000/jsonrpc")]
public interface IUserApi 
{
    [JsonRpcMethod("add")]
    ITask<JsonRpcResult<User>> AddAsync([JsonRpcParam] string name, [JsonRpcParam] int age, CancellationToken token = default);
}
```

## JsonRpc数据包

```log

POST /jsonrpc HTTP/1.1
Host: localhost:5000
User-Agent: WebApiClientCore/1.0.6.0
Accept: application/json; q=0.01, application/xml; q=0.01
Content-Type: application/json-rpc

{"jsonrpc":"2.0","method":"add","params":["laojiu",18],"id":1}
```
