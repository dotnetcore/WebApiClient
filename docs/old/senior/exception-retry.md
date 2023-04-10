# 4、异常处理和重试策略

## 4.1 try catch异常处理

```csharp
try
{
    var user = await userApi.GetByIdAsync("id001");
    ...
}
catch (HttpStatusFailureException ex)
{
    var error = ex.ReadAsAsync<ErrorModel>();
    ...
}
catch (HttpApiException ex)
{
    ...
}
```

## 4.2 Retry重试策略

```csharp
try
{
    var user1 = await userApi
        .GetByIdAsync("id001")
        .Retry(3, i => TimeSpan.FromSeconds(i))
        .WhenCatch<HttpStatusFailureException>();
    ...
}
catch (HttpStatusFailureException ex)
{
    var error = ex.ReadAsAsync<ErrorModel>();
    ...
}
catch (HttpApiException ex)
{
    ...
}
catch(Exception ex)
{
    ...
}
```

## 4.3 RX扩展

在一些场景中，你可能不需要使用async/await异步编程方式，WebApiClient提供了Task对象转换为IObservable对象的扩展，使用方式如下：

```csharp
var unSubscriber = userApi.GetByIdAsync("id001")
    .Retry(3, i => TimeSpan.FromSeconds(i))
    .WhenCatch<HttpStatusFailureException>();
    .ToObservable().Subscribe(result =>
    {
        ...
    }, ex =>
    {
         ...
    });
```
