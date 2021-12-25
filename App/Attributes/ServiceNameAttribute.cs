using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System.Threading;
using System;
using WebApiClientCore.Attributes;
using WebApiClientCore;
using App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace App.Attributes
{
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
}
