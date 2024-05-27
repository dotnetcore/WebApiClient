using App.Clients;
using App.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace App
{
    /// <summary>
    /// 启动页
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 获取配置
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// 环境
        /// </summary>
        public IWebHostEnvironment Environment { get; }

        /// <summary>
        /// 构造器
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>     
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }


        /// <summary>
        /// 添加服务
        /// </summary>
        /// <param name="services"></param>  
        public void ConfigureServices(IServiceCollection services)
        {
            // 添加控制器
            services.AddControllers().AddXmlSerializerFormatters();

            // 应用编译时生成接口的代理类型代码
            services
                .AddWebApiClient()
                .UseJsonFirstApiActionDescriptor()
                .UseSourceGeneratorHttpApiActivator()
                .AddDynamicDependencyApp();

            // 注册userApi
            services.AddHttpApi(typeof(IUserApi), o =>
            {
                o.UseLogging = Environment.IsDevelopment();
                o.HttpHost = new Uri("http://localhost:5000/");
            });

            // 注册与配置clientId模式的token提者选项
            services.AddClientCredentialsTokenProvider<IUserApi>(o =>
            {
                o.Endpoint = new Uri("http://localhost:5000/api/tokens");
                o.Credentials.Client_id = "clientId";
                o.Credentials.Client_secret = "xxyyzz";
            });

            // userApi客户端后台服务
            services.AddScoped<UserService>().AddHostedService<UserHostedService>();
            services.AddDynamicHostSupport();
        }

        /// <summary>
        /// 配置中间件
        /// </summary>
        /// <param name="app"></param>    
        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
