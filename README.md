## WebApiClientCore 　　　　　　　　　　　　　　　　　　　
[WebApiClient.JIT](https://github.com/dotnetcore/WebApiClient/tree/WebApiClient.JITAOT)的.netcore版本，目前尚属于alpha阶段，计划只支持.netcore平台，并紧密与.netcore新特性紧密结合。
 
### 项目原因
 
1. WebApiClient很优秀，它将不同框架不同平台都实现了统一的api
2. WebApiClient不够优秀，它在.netcore下完全可以更好，但它不得不兼容.net45开始所有框架而有所牺牲


### 相对变化
1. 使用System.Text.Json替换Json.net
2. 提升内置的多个HttpContent性能
3. 移除HttpApiFactory和HttApiConfig功能，紧密结合DependencyInjection和HttpClientFactory
4. 移除AOT功能，目前依赖于Emit
5. 高效的ActionInvoker
6. 所有特性都基于中间件思想开发
7. 基于管道编排各个特性中间件，执行逻辑清晰
8. 良好设计的HttpContext、ApiRequestContext、ApiParameterContext和ApiResponseContext


### PackageReference
> WebApiClientCore

    <PackageReference Include="WebApiClientCore" Version="1.0.0-alpha9" /> 
