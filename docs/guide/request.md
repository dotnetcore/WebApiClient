# 请求声明

## 表单集合处理

按照OpenApi，一个集合在Uri的Query或表单中支持5种表述方式，分别是：

* Csv // 逗号分隔
* Ssv // 空格分隔
* Tsv // 反斜杠分隔
* Pipes // 竖线分隔
* Multi // 多个同名键的键值对

对于 id = new string []{"001","002"} 这样的值，在PathQueryAttribute与FormContentAttribute处理后分别是：

CollectionFormat | Data
---|---
[PathQuery(CollectionFormat = CollectionFormat.Csv)] | `id=001,002`
[PathQuery(CollectionFormat = CollectionFormat.Ssv)] | `id=001 002`
[PathQuery(CollectionFormat = CollectionFormat.Tsv)] | `id=001\002`
[PathQuery(CollectionFormat = CollectionFormat.Pipes)] | `id=001\|002`
[PathQuery(CollectionFormat = CollectionFormat.Multi)] | `id=001&id=002`

## CancellationToken参数

每个接口都支持声明一个或多个CancellationToken类型的参数，用于支持取消请求操作。CancellationToken.None表示永不取消，创建一个CancellationTokenSource，可以提供一个CancellationToken。

```csharp
[HttpGet("api/users/{id}")]
ITask<User> GetAsync([Required]string id, CancellationToken token = default);
```

## ContentType CharSet

对于非表单的body内容，默认或缺省时的charset值，对应的是UTF8编码，可以根据服务器要求调整编码。

Attribute | ContentType
---|---
[JsonContent] | Content-Type: application/json; charset=utf-8
[JsonContent(CharSet ="utf-8")] | Content-Type: application/json; charset=utf-8
[JsonContent(CharSet ="unicode")] | Content-Type: application/json; charset=utf-16

## Accpet ContentType

这个用于控制客户端希望服务器返回什么样的内容格式，比如json或xml。

## PATCH请求

json patch是为客户端能够局部更新服务端已存在的资源而设计的一种标准交互，在RFC6902里有详细的介绍json patch，通俗来讲有以下几个要点：

1. 使用HTTP PATCH请求方法；
2. 请求body为描述多个opration的数据json内容；
3. 请求的Content-Type为application/json-patch+json；

### 声明Patch方法

```csharp
public interface IUserApi
{
    [HttpPatch("api/users/{id}")]
    Task<UserInfo> PatchAsync(string id, JsonPatchDocument<User> doc);
}
```

### 实例化JsonPatchDocument

```csharp
var doc = new JsonPatchDocument<User>();
doc.Replace(item => item.Account, "laojiu");
doc.Replace(item => item.Email, "laojiu@qq.com");
```

### 请求内容

```csharp
PATCH /api/users/id001 HTTP/1.1
Host: localhost:6000
User-Agent: WebApiClientCore/1.0.0.0
Accept: application/json; q=0.01, application/xml; q=0.01
Content-Type: application/json-patch+json

[{"op":"replace","path":"/account","value":"laojiu"},{"op":"replace","path":"/email","value":"laojiu@qq.com"}]
```

## 非模型请求

有时候我们未必需要强模型，假设我们已经有原始的form文本内容，或原始的json文本内容，甚至是System.Net.Http.HttpContent对象，只需要把这些原始内请求到远程远程器。

### 原始文本

```csharp
[HttpPost]
Task PostAsync([RawStringContent("txt/plain")] string text);

[HttpPost]
Task PostAsync(StringContent text);
```

### 原始json

```csharp
[HttpPost]
Task PostAsync([RawJsonContent] string json);
```

### 原始xml

```csharp
[HttpPost]
Task PostAsync([RawXmlContent] string xml);
```

### 原始表单内容

```csharp
[HttpPost]
Task PostAsync([RawFormContent] string form);
```

## 自定义自解释的参数类型

在某些极限情况下，比如人脸比对的接口，我们输入模型与传输模型未必是对等的，例如：

服务端要求的json模型

```json
{
    "image1" : "图片1的base64",
    "image2" : "图片2的base64"
}
```

客户端期望的业务模型

```csharp
class FaceModel
{
    public Bitmap Image1 {get; set;}
    public Bitmap Image2 {get; set;}
}
```

我们希望构造模型实例时传入Bitmap对象，但传输的时候变成Bitmap的base64值，所以我们要改造FaceModel，让它实现IApiParameter接口：

```csharp
class FaceModel : IApiParameter
{
    public Bitmap Image1 { get; set; }

    public Bitmap Image2 { get; set; }


    public Task OnRequestAsync(ApiParameterContext context)
    {
        var image1 = GetImageBase64(this.Image1);
        var image2 = GetImageBase64(this.Image2);
        var model = new { image1, image2 };
        
        var options = context.HttpContext.HttpApiOptions.JsonSerializeOptions;
        context.HttpContext.RequestMessage.Content = new JsonContent(model,options);
    }

    private static string GetImageBase64(Bitmap image)
    {
        using var stream = new MemoryStream();
        image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
        return Convert.ToBase64String(stream.ToArray());
    }
}
```

最后，我们在使用改进后的FaceModel来请求

```csharp
public interface IFaceApi
{
    [HttpPost("/somePath")]
    Task<HttpResponseMessage> PostAsync(FaceModel faces);
}
```
