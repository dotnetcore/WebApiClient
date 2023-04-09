# 3、自定义特性

WebApiClient内置很多特性，包含接口级、方法级、参数级的，他们分别是实现了`IApiActionAttribute`接口、`IApiActionFilterAttribute`接口、`IApiParameterAttribute`接口、`IApiParameterable`接口和`IApiReturnAttribute`接口的一个或多个接口。

## 3.1 自定义IApiParameterAttribute

例如服务端要求使用`x-www-form-urlencoded`提交，由于接口设计不合理，目前要求是提交`：fieldX= {X}`的json文本&fieldY={Y}的json文本 这里`{X}`和`{Y}`都是一个多字段的Model，我们对应的接口是这样设计的：

```csharp
[HttpHost("/upload")]
ITask<bool> UploadAsync(
      [FormField][AliasAs("fieldX")] string xJson,
      [FormField][AliasAs("fieldY")] string yJson);
```

显然，我们接口参数为`string`类型的范围太广，没有约束性，我们希望是这样子

```csharp
[HttpHost("/upload")]
ITask<bool> UploadAsync([FormFieldJson] X fieldX, [FormFieldJson] Y fieldY);
```

`[FormFieldJson]`将参数值序列化为Json并做为表单的一个字段内容

```csharp
[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
class FormFieldJson: Attribute, IApiParameterAttribute
{
    public async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
    {
        var options = context.HttpApiConfig.FormatOptions;
        var json = context.HttpApiConfig.JsonFormatter.Serialize(parameter.Value, options);
        var fieldName = parameter.Name;
        await context.RequestMessage.AddFormFieldAsync(fieldName, json);
    }
}
```
