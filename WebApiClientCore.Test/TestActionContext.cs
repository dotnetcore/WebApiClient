using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace WebApiClientCore.Test
{
    public class TestActionContext : ApiActionContext
    {
        /// <summary>
        /// 请求Api的上下文
        /// </summary>
        /// <param name="httpApi">httpApi代理类实例</param>
        /// <param name="httpApiConfig">关联的HttpApiConfig</param>
        /// <param name="apiActionDescriptor">关联的ApiActionDescriptor</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TestActionContext(ApiActionDescriptor apiActionDescriptor, params object[] args)
            : base(new TestApi(), new HttpClient(), GetServiceProvider(), new HttpApiOptions(), apiActionDescriptor, args)
        {
            this.ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        private static IServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddHttpApi<ITestApi>();
            return services.BuildServiceProvider();
        }

        public interface ITestApi : IHttpApi
        {
        }

        private class TestApi : IHttpApi { }
    }
}
