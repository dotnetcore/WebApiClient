
# 异常和异常处理

请求一个接口，不管出现何种异常，最终都抛出HttpRequestException，HttpRequestException的内部异常为实际具体异常，之所以设计为内部异常，是为了完好的保存内部异常的堆栈信息。

WebApiClient内部的很多异常都基于ApiException这个抽象异常，也就是很多情况下，抛出的异常都是内为某个ApiException的HttpRequestException。

```csharp
try
{
    var model = await api.GetAsync();
}
catch (HttpRequestException ex) when (ex.InnerException is ApiInvalidConfigException configException)
{
    // 请求配置异常
}
catch (HttpRequestException ex) when (ex.InnerException is ApiResponseStatusException statusException)
{
    // 响应状态码异常
}
catch (HttpRequestException ex) when (ex.InnerException is ApiException apiException)
{
    // 抽象的api异常
}
catch (HttpRequestException ex) when (ex.InnerException is SocketException socketException)
{
    // socket连接层异常
}
catch (HttpRequestException ex)
{
    // 请求异常
}
catch (Exception ex)
{
    // 异常
}
```
