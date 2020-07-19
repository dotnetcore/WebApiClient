## WebApiClientCore.Extensions.JsonRpc　　　　　　　　　　　　　　　
WebApiClientCore的JsonRpc调用扩展

### 使用方式
使用[JsonRpcMethod]修饰JsonRpc方法方法，使用[JsonRpcParam]修复JsonRpc参数

```c#
[HttpHost("http://localhost:5000/jsonrpc")]
public interface IUserApi 
{
    [JsonRpcMethod("add")]
    ITask<JsonRpcResult<User>> AddAsync([JsonRpcParam] string name, [JsonRpcParam] int age);
}
```