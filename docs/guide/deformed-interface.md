# 适配畸形接口

在实际应用场景中，常常会遇到一些设计不标准的畸形接口，主要是早期还没有restful概念时期的接口，我们要区分分析这些接口，包装为友好的客户端调用接口。

## 不友好的参数名别名

例如服务器要求一个Query参数的名字为`field-Name`，这个是c#关键字或变量命名不允许的，我们可以使用`[AliasAsAttribute]`来达到这个要求：

```csharp
public interface IDeformedApi
{
    [HttpGet("api/users")]
    ITask<string> GetAsync([AliasAs("field-Name")] string fieldName);  
}
```

然后最终请求uri变为api/users/?field-name=`fileNameValue`

## Form的某个字段为json文本

字段 | 值
---|---
field1 | someValue
field2 | {"name":"sb","age":18}

对应强类型模型是

```csharp
class Field2
{
    public string Name {get; set;}
    
    public int Age {get; set;}
}
```

常规下我们得把field2的实例json序列化得到json文本，然后赋值给field2这个string属性，使用[JsonFormField]特性可以轻松帮我们自动完成Field2类型的json序列化并将结果字符串作为表单的一个字段。

```csharp
public interface IDeformedApi
{
    Task PostAsync([FormField] string field1, [JsonFormField] Field2 field2)
}
```

## Form提交嵌套的模型

字段 | 值
---|---|
|filed1 |someValue|
|field2.name | sb|
|field2.age | 18|

其对应的json格式为

```json
{
    "field1" : "someValue",
    "filed2" : {
        "name" : "sb",
        "age" : 18
    }
}
```

合理情况下，对于复杂嵌套结构的数据模型，应当使用applicaiton/json，但接口要求必须使用Form提交，我可以配置KeyValueSerializeOptions来达到这个格式要求：

```csharp
services.AddHttpApi<IDeformedApi>(o =>
{
    o.KeyValueSerializeOptions.KeyNamingStyle = KeyNamingStyle.FullName;
});
```

## 响应未指明ContentType

明明响应的内容肉眼看上是json内容，但服务响应头里没有ContentType告诉客户端这内容是json，这好比客户端使用Form或json提交时就不在请求头告诉服务器内容格式是什么，而是让服务器猜测一样的道理。

解决办法是在Interface或Method声明`[JsonReturn]`特性，并设置其EnsureMatchAcceptContentType属性为false，表示ContentType不是期望值匹配也要处理。

```csharp
[JsonReturn(EnsureMatchAcceptContentType = false)] 
public interface IDeformedApi 
{
}
```

## 类签名参数或apikey参数

例如每个请求的url额外的动态添加一个叫sign的参数，这个sign可能和请求参数值有关联，每次都需要计算。

我们可以自定义ApiFilterAttribute来实现自己的sign功能，然后把自定义Filter声明到Interface或Method即可

```csharp
class SignFilterAttribute : ApiFilterAttribute
{
    public override Task OnRequestAsync(ApiRequestContext context)
    {
        var signService = context.HttpContext.ServiceProvider.GetService<SignService>();
        var sign = signService.SignValue(DateTime.Now);
        context.HttpContext.RequestMessage.AddUrlQuery("sign", sign);
        return Task.CompletedTask;
    }
}

[SignFilter]
public interface IDeformedApi 
{
    ...
}
```

## 表单字段排序

不知道是哪门公司起的所谓的“签名算法”，往往要字段排序等。

```csharp
class SortedFormContentAttribute : FormContentAttribute
{
    protected override IEnumerable<KeyValue> SerializeToKeyValues(ApiParameterContext context)
    {
        这里可以排序、加上其它衍生字段等
        return base.SerializeToKeyValues(context).OrderBy(item => item.Key);
    }
}

public interface IDeformedApi
{
    [HttpGet("/path")]
    Task<HttpResponseMessage> PostAsync([SortedFormContent] Model model);
}
```
