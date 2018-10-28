using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using WebApiClient.Contexts;

namespace WebApiClient.Test
{
    class TestActionContext : Contexts.ApiActionContext
    {
        /// <summary>
        /// 请求Api的上下文
        /// </summary>
        /// <param name="httpApi">httpApi代理类实例</param>
        /// <param name="httpApiConfig">关联的HttpApiConfig</param>
        /// <param name="apiActionDescriptor">关联的ApiActionDescriptor</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TestActionContext(IHttpApi httpApi, HttpApiConfig httpApiConfig, ApiActionDescriptor apiActionDescriptor)
            : base(httpApi, httpApiConfig, apiActionDescriptor)
        {
            this.ResponseMessage = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }
    }
}
