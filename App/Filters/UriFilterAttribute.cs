using System;
using System.Threading.Tasks;
using App.Services;
using Microsoft.Extensions.DependencyInjection;
using WebApiClientCore;
using WebApiClientCore.Attributes;

namespace App.Filters
{
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
}
