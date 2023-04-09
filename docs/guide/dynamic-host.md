
# 动态Host

针对大家经常提问的动态Host,提供以下简单的示例供参阅；实现的方式不仅限于示例中提及的，**原则上在请求还没有发出去之前的任何环节，都可以修改请求消息的RequestUri来实现动态目标的目的**

```csharp
    

[LoggingFilter]
    public interface IDynamicHostDemo
    {
        //直接传入绝对目标的方式
        [HttpGet]
        ITask<HttpResponseMessage> ByUrlString([Uri] string urlString);
        
        //通过Filter的形式
        [HttpGet]
        [UriFilter]
        ITask<HttpResponseMessage> ByFilter();
        //通过Attribute修饰的方式
        [HttpGet]
        [ServiceName("baiduService")]
        ITask<HttpResponseMessage> ByAttribute();
    }

    /*通过Attribute修饰的方式*/

    /// <summary>
    /// 表示对应的服务名
    /// </summary>
    public class ServiceNameAttribute : ApiActionAttribute
    {
        public ServiceNameAttribute(string name)
        {
            Name = name;
            OrderIndex = int.MinValue;
        }

        public string Name { get; set; }

        public override async Task OnRequestAsync(ApiRequestContext context)
        {
            await Task.CompletedTask;
            IServiceProvider sp = context.HttpContext.ServiceProvider;
            HostProvider hostProvider = sp.GetRequiredService<HostProvider>();
            //服务名也可以在接口配置时挂在Properties中
            string host = hostProvider.ResolveService(this.Name);
            HttpApiRequestMessage requestMessage = context.HttpContext.RequestMessage;
            //和原有的Uri组合并覆盖原有Uri
            //并非一定要这样实现，只要覆盖了RequestUri,即完成了替换
            requestMessage.RequestUri = requestMessage.MakeRequestUri(new Uri(host));
        }

    }

    /*通过Filter修饰的方式*/

    /// <summary>
    ///用来处理动态Uri的拦截器 
    /// </summary>
    public class UriFilterAttribute : ApiFilterAttribute
    {
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            var options = context.HttpContext.HttpApiOptions;
            //获取注册时为服务配置的服务名
            options.Properties.TryGetValue("serviceName", out object serviceNameObj);
            string serviceName = serviceNameObj as string;
            IServiceProvider sp = context.HttpContext.ServiceProvider;
            HostProvider hostProvider = sp.GetRequiredService<HostProvider>();
            string host = hostProvider.ResolveService(serviceName);
            HttpApiRequestMessage requestMessage = context.HttpContext.RequestMessage;
            //和原有的Uri组合并覆盖原有Uri
            //并非一定要这样实现，只要覆盖了RequestUri,即完成了替换
            requestMessage.RequestUri = requestMessage.MakeRequestUri(new Uri(host));
            return Task.CompletedTask;
        }

        public override Task OnResponseAsync(ApiResponseContext context)
        {
            //不处理响应的信息
            return Task.CompletedTask;
        }
    }

    //以上代码中涉及的依赖项
    public interface IDBProvider
    {
        string SelectServiceUri(string serviceName);
    }
    public class DBProvider : IDBProvider
    {
        public string SelectServiceUri(string serviceName)
        {
            if (serviceName == "baiduService") return "https://www.baidu.com";
            if (serviceName == "microsoftService") return "https://www.microsoft.com";
            return string.Empty;
        }
    }

    public class HostProvider
    {
        private readonly IDBProvider dbProvider;

        public HostProvider(IDBProvider dbProvider)
        {
            this.dbProvider = dbProvider;
            //将HostProvider放到依赖注入容器中，即可从容器获取其它服务来实现动态的服务地址查询
        }

        internal string ResolveService(string name)
        {
            //如何获取动态的服务地址由你自己决定，此处仅以简单的接口定义简要说明
            return dbProvider.SelectServiceUri(name);
        }
    }
```
