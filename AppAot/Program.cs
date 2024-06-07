using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AppAot
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
                    services
                        .AddWebApiClient()
                        .ConfigureHttpApi(options => // json SG生成器配置
                        {
                            options.PrependJsonSerializerContext(AppJsonSerializerContext.Default);
                        });

                    services.AddHttpApi<ICloudflareApi>();
                    services.AddHostedService<AppHostedService>();
                })
                .Build()
                .Run();
        }
    }
}
