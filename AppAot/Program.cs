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
                        .UseSourceGeneratorHttpApiActivator() // SG 激活器
                        .ConfigureHttpApi(options => // json SG生成器配置
                        {
                            var jsonContext = AppJsonSerializerContext.Default;
                            options.JsonSerializeOptions.TypeInfoResolverChain.Insert(0, jsonContext);
                            options.JsonDeserializeOptions.TypeInfoResolverChain.Insert(0, jsonContext);
                            options.KeyValueSerializeOptions.GetJsonSerializerOptions().TypeInfoResolverChain.Insert(0, jsonContext);
                        });

                    services.AddHttpApi<ICloudflareApi>();
                    services.AddHostedService<AppHostedService>();
                })
                .Build()
                .Run();
        }
    }
}
