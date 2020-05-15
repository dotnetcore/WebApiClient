using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using WebApiClientCore.Defaults;

namespace WebApiClientCore.Test
{
    public class TestRequestContext : ApiRequestContext
    {
        /// <summary>
        /// 请求Api的上下文
        /// </summary>
        /// <param name="httpApi">httpApi代理类实例</param>
        /// <param name="httpApiConfig">关联的HttpApiConfig</param>
        /// <param name="apiActionDescriptor">关联的ApiActionDescriptor</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TestRequestContext(ApiActionDescriptor apiActionDescriptor, params object[] args)
            : base(GetHttpContext(), apiActionDescriptor, args)
        {
            this.HttpContext.ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        private static HttpContext GetHttpContext()
        {
            var services = new ServiceCollection();
            services.TryAddSingleton<IXmlFormatter, XmlFormatter>();
            services.TryAddSingleton<IJsonFormatter, JsonFormatter>();
            services.TryAddSingleton<IKeyValueFormatter, KeyValueFormatter>();
            services.TryAddSingleton<IResponseCacheProvider, ResponseCacheProvider>();
            services.TryAddSingleton<IApiActionDescriptorProvider, ApiActionDescriptorProvider>();

            var requestServices = services.BuildServiceProvider();
            return new HttpContext(new HttpClient(), requestServices, new HttpApiOptions());
        }
    }
}
