using Castle.DynamicProxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示web api请求客户端
    /// </summary>
    public class HttpApiClient : IInterceptor, IDisposable
    {
        /// <summary>
        /// 代理生成器
        /// </summary>
        private readonly ProxyGenerator generator = new ProxyGenerator();

        /// <summary>
        /// 获取或设置http客户端
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// 获取或设置json解析工具
        /// </summary>
        public IJsonFormatter JsonFormatter { get; set; }

        /// <summary>
        /// web api请求客户端
        /// </summary>
        public HttpApiClient()
            : this(null)
        {
        }

        /// <summary>
        /// web api请求客户端
        /// </summary>
        /// <param name="httpClient">关联的http客户端</param>
        public HttpApiClient(HttpClient httpClient)
        {
            if (httpClient == null)
            {
                httpClient = new HttpClient();
            }

            this.HttpClient = httpClient;
            this.JsonFormatter = new DefaultJsonFormatter();
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <returns></returns>
        public TInterface GetHttpApi<TInterface>() where TInterface : class
        {
            return this.generator.CreateInterfaceProxyWithoutTarget<TInterface>(this);
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <param name="host">服务跟路径，效果与HttpHostAttribute一致</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public TInterface GetHttpApi<TInterface>(string host) where TInterface : class
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException();
            }

            if (typeof(TInterface).IsInterface == false)
            {
                throw new ArgumentException(typeof(TInterface).Name + "不是接口类型");
            }

            var option = new ProxyGenerationOptions();
            var ctor = typeof(HttpHostAttribute).GetConstructors().FirstOrDefault();
            var hostAttribute = new CustomAttributeInfo(ctor, new object[] { host });
            option.AdditionalAttributes.Add(hostAttribute);
            return this.generator.CreateInterfaceProxyWithoutTarget<TInterface>(option, this);
        }

        /// <summary>
        /// 方法拦截
        /// </summary>
        /// <param name="invocation">拦截内容</param>
        void IInterceptor.Intercept(IInvocation invocation)
        {
            var context = CastleContext.From(invocation);
            var actionContext = new ApiActionContext
            {
                HttpApiClient = this,
                RequestMessage = new HttpRequestMessage(),
                HostAttribute = context.HostAttribute,
                ApiReturnAttribute = context.ApiReturnAttribute,
                ApiActionFilterAttributes = context.ApiActionFilterAttributes,
                ApiActionDescriptor = context.ApiActionDescriptor.Clone() as ApiActionDescriptor
            };

            var parameters = actionContext.ApiActionDescriptor.Parameters;
            for (var i = 0; i < parameters.Length; i++)
            {
                parameters[i].Value = invocation.Arguments[i];
            }

            var apiAction = context.ApiActionDescriptor;
            invocation.ReturnValue = apiAction.Execute(actionContext);
        }

        /// <summary>
        /// 释放相关资源
        /// </summary>
        public void Dispose()
        {
            this.HttpClient.Dispose();
        }
    }
}
