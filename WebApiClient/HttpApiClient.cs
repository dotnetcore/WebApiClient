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
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi客户端
    /// 提供获取Http接口的实例
    /// </summary>
    public class HttpApiClient : IInterceptor
    {
        /// <summary>
        /// 代理生成器
        /// </summary>
        private static readonly ProxyGenerator generator = new ProxyGenerator();

        /// <summary>
        /// 获取配置项
        /// </summary>
        public HttpApiClientConfig Config { get; private set; }

        /// <summary>
        /// HttpApi客户端
        /// </summary>
        public HttpApiClient()
        {
            this.Config = new HttpApiClientConfig();
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TApiInterface">请求接口</typeparam>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public TApiInterface Implement<TApiInterface>() where TApiInterface : class
        {
            return HttpApiClient.GeneratoProxy<TApiInterface>(null, this);
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TApiInterface">请求接口</typeparam>
        /// <param name="host">服务跟路径，效果与HttpHostAttribute一致</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <returns></returns>
        public TApiInterface Implement<TApiInterface>(string host) where TApiInterface : class
        {
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException();
            }

            if (typeof(TApiInterface).IsInterface == false)
            {
                throw new ArgumentException(typeof(TApiInterface).Name + "不是接口类型");
            }

            return HttpApiClient.GeneratoProxy<TApiInterface>(host, this);
        }

        /// <summary>
        /// 获取请求接口的实现对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <param name="host">服务跟路径</param>
        /// <param name="interceptor">拦截器</param>
        /// <returns></returns>
        private static TInterface GeneratoProxy<TInterface>(string host, IInterceptor interceptor) where TInterface : class
        {
            var option = new ProxyGenerationOptions();
            if (string.IsNullOrEmpty(host) == false)
            {
                var ctor = typeof(HttpHostAttribute).GetConstructors().FirstOrDefault();
                var hostAttribute = new CustomAttributeInfo(ctor, new object[] { host });
                option.AdditionalAttributes.Add(hostAttribute);
            }
            return HttpApiClient.generator.CreateInterfaceProxyWithoutTarget<TInterface>(option, interceptor);
        }


        /// <summary>
        /// 方法拦截
        /// </summary>
        /// <param name="invocation">拦截内容</param>
        void IInterceptor.Intercept(IInvocation invocation)
        {
            var context = CastleContext.From(invocation);
            var actionDescripter = context.ApiActionDescriptor.Clone() as ApiActionDescriptor;
            for (var i = 0; i < actionDescripter.Parameters.Length; i++)
            {
                actionDescripter.Parameters[i].Value = invocation.GetArgumentValue(i);
            }

            var actionContext = new ApiActionContext
            {
                HttpApiClientConfig = this.Config,
                RequestMessage = new HttpRequestMessage(),
                ResponseMessage = null,
                HttpClientContext = null,
                HostAttribute = context.HostAttribute,
                ApiReturnAttribute = context.ApiReturnAttribute,
                ApiActionFilterAttributes = context.ApiActionFilterAttributes,
                ApiActionDescriptor = actionDescripter
            };

            invocation.ReturnValue = context.ApiActionDescriptor.Execute(actionContext);
        }
    }
}
