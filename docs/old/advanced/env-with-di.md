# 有依赖注入的环境

## Asp.net core 2.1+

接口声明

```csharp
public interface IMyWebApi : IHttpApi
{
    [HttpGet("user/{id}")]
    ITask<UserInfo> GetUserAsync(string id);
}
```

Startup.cs配置依赖注入

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddHttpApi<IMyWebApi>();
    services.ConfigureHttpApi<IMyWebApi>(o=>
    {
        o.HttpHost = new Uri("http://localhost:9999/");
        ...
    });
}
```

Controller代码

```csharp
public class HomeController : Controller
{
    public async Task<UserInfo> Index([FromServices]IMyWebApi myWebApi)
    {
        return await myWebApi.GetUserAsync("id001");
    }
}
```

## Asp.net MVC + Autofac

接口声明

```csharp
public interface IMyWebApi : IHttpApi
{
    [HttpGet("user/{id}")]
    ITask<UserInfo> GetUserAsync(string id);
}
```

Global.asax.cs Application_Start

```csharp
var builder = new ContainerBuilder();
builder.RegisterControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();

builder.Register(_ => new HttpApiFactory<IMyWebApi>()
    .ConfigureHttpApiConfig(c =>
    {
        c.HttpHost = new Uri("http://localhost:9999/");
        c.FormatOptions.DateTimeFormat = DateTimeFormats.ISO8601_WithMillisecond;
    }))
    .As<IHttpApiFactory<IMyWebApi>>()
    .SingleInstance();

builder.Register(c => c.Resolve<IHttpApiFactory<IMyWebApi>>().CreateHttpApi())
    .As<IMyWebApi>()
    .InstancePerHttpRequest();

DependencyResolver.SetResolver(new AutofacDependencyResolver(builder.Build()));
```

Controller

```csharp
public class HomeController : Controller
{
    public IMyWebApi MyWebApi { get; set; }

    public async Task<ActionResult> Index()
    {
        var user = await this.MyWebApi.GetUserAsync("id001");
        return View(user);
    }
}
```
