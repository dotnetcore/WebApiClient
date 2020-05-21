## WebApiClientCore.Analyzers　　　　　　　　　　　　　　　　　　
WebApiClientCore声明接口的语法分析，编译前就分析接口是否符合要求，需要接口现实IHttpApi接口才触发分析
 
### 使用方式
```
/// <summary>
/// 你的接口，记得要实现IHttpApi
/// </summary>
public interface IYourApi : IHttpApi
{
    ...
}
```