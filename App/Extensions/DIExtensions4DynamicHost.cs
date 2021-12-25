using App.Clients;
using App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace App.Extensions
{
    internal static class DIExtensions4DynamicHost
    {
        /// <summary>
        /// 动态Host的Demo相关服务注册
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddDynamicHostSupport(this IServiceCollection services)
        {
            services.AddSingleton<HostProvider>();
            services.AddSingleton<IDBProvider, DBProvider>();
            services.AddHttpApi<IDynamicHostDemo>().ConfigureHttpApi(options => {
                options.Properties.Add("serviceName", "microsoftService");            
            });
            services.AddScoped<DynamicHostDemoService>();
            services.AddHostedService<DynamicHostDemoHostedService>();
            return services;
        }
    }
}
