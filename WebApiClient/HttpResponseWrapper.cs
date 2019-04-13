using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// 表示http响应包装器抽象类
    /// 其子类可以声明为接口的返回类型
    /// </summary>
    public abstract class HttpResponseWrapper
    {
        /// <summary>
        /// 获取响应消息
        /// </summary>
        protected HttpResponseMessage HttpResponse { get; }

        /// <summary>
        /// http响应包装器抽象类
        /// </summary>
        /// <param name="httpResponse">响应消息</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpResponseWrapper(HttpResponseMessage httpResponse)
        {
            this.HttpResponse = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));
        }
    }
}
