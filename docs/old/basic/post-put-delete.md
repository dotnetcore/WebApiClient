# 3、POST/PUT/DELETE请求

## 3.1 使用Json或Xml提交

使用`[XmlContent]`或`[Parameter(Kind.XmlBody)]`修饰强类型模型参数，表示提交xml
使用`[JsonContent]`或`[Parameter(Kind.JsonBody)]`修饰强类型模型参数，表示提交json

```csharp
// POST webapi/user  
// Body user的json文本
[HttpPost("webapi/user")]
ITask<UserInfo> AddUserWithJsonAsync([JsonContent] UserInfo user);

// PUT webapi/user  
// Body user的xml文本
[HttpPut("webapi/user")]
ITask<UserInfo> UpdateUserWithXmlAsync([XmlContent] UserInfo user);
```

## 3.2 使用x-www-form-urlencoded提交

使用`[FormContent]`或`[Parameter(Kind.Form)]`修饰强类型模型参数
使用`[FormField]`或`[Parameter(Kind.Form)]`修饰简单类型参数

```csharp
// POST webapi/user  
// Body Account=laojiu&Password=123456
[HttpPost("webapi/user")]
ITask<UserInfo> UpdateUserWithFormAsync(
    [FormContent] UserInfo user);

// POST webapi/user  
// Body Account=laojiu&Password=123456&fieldX=xxx
[HttpPost("webapi/user")]
ITask<UserInfo> UpdateUserWithFormAsync(
    [FormContent] UserInfo user,
    [FormField] string fieldX);

// POST webapi/user  
// Body Account=laojiu&Password=123456&fieldX=xxx
[HttpPost("webapi/user")]
ITask<UserInfo> UpdateUserAsync(
    [Parameter(Kind.Form)] UserInfo user,
    [Parameter(Kind.Form)] string fieldX);
```

## 3.3 使用multipart/form-data提交

使用`[MulitpartContent]`或`[Parameter(Kind.FormData)]`修饰强类型模型参数
使用`[MulitpartText]`或`[Parameter(Kind.FormData)]`修饰简单类型参数
使用`MulitpartFile`类型作为提交的文件

```csharp
// POST webapi/user  
[HttpPost("webapi/user")]
ITask<UserInfo> UpdateUserWithMulitpartAsync(
    [MulitpartContent] UserInfo user);

// POST webapi/user  
[HttpPost("webapi/user")]
ITask<UserInfo> UpdateUserWithMulitpartAsync(
    [MulitpartContent] UserInfo user,
    [MulitpartText] string nickName,
    MulitpartFile file);

// POST webapi/user  
[HttpPost("webapi/user")]
ITask<UserInfo> UpdateUserWithMulitpartAsync(
    [Parameter(Kind.FormData)] UserInfo user,
    [Parameter(Kind.FormData)] string nickName,
    MulitpartFile file);
```

## 3.4 使用具体的HttpContent类型提交

```csharp
// POST webapi/user  
// Body Account=laojiu&Password=123456
[HttpPost("webapi/user")]
ITask<UserInfo> UpdateUserWithFormAsync(
    FormUrlEncodedContent user);

// POST webapi/user  
// Body Account=laojiu&Password=123456&age=18
[HttpPost("webapi/user")]
ITask<UserInfo> UpdateUserWithFormAsync(
    FormUrlEncodedContent user,
    [FormField] int age);
```

如果参数是类型是`HttpContent`类型的子类，如`StringContent`、`ByteArrayContent`、`StreamContent`、`FormUrlEncodedContent`等等，则可以直接做为参数，**但是必须放在其它参数的前面**
