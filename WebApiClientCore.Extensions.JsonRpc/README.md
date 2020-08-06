## WebApiClientCore.Extensions.JsonRpc　　　　　　　　　　　　　　　
WebApiClientCore的JsonRpc调用扩展

### 使用方式
使用[JsonRpcMethod]修饰Rpc方法，使用[JsonRpcParam]修饰Rpc参数

```c#
[HttpHost("http://localhost:5000/jsonrpc")]
public interface IUserApi 
{
    [JsonRpcMethod("add")]
    ITask<JsonRpcResult<User>> AddAsync([JsonRpcParam] string name, [JsonRpcParam] int age, CancellationToken token = default);
}
```

```
POST /jsonrpc HTTP/1.1
Host: localhost:5000
User-Agent: WebApiClientCore/1.0.6.0
Accept: application/json; q=0.01, application/xml; q=0.01
Content-Type: application/json-rpc

{"jsonrpc":"2.0","method":"add","params":["laojiu",18],"id":1}
```