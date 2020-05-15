using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace WebApiClientCore.Test
{
    public class TestActionContext : ApiRequestContext
    {
        /// <summary>
        /// 请求Api的上下文
        /// </summary>
        /// <param name="httpApi">httpApi代理类实例</param>
        /// <param name="httpApiConfig">关联的HttpApiConfig</param>
        /// <param name="apiActionDescriptor">关联的ApiActionDescriptor</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TestActionContext(ApiActionDescriptor apiActionDescriptor, params object[] args)
            : base(GetHttpContext(), apiActionDescriptor, args)
        {
            this.HttpContext.ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        private static HttpContext GetHttpContext()
        {
            var services = new ServiceCollection();
            services.AddHttpApi<ITestApi>();
            var requestServices = services.BuildServiceProvider();
            return new HttpContext(new HttpClient(), requestServices, new HttpApiOptions());
        }

        public interface ITestApi : IHttpApi
        {
        }

        private class TestApi : IHttpApi { }
    }
}
