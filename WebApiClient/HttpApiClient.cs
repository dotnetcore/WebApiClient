
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi客户端
    /// 提供获取Http接口的实例
    /// </summary>
    public abstract class HttpApiClient : IHttpApiClient
    {
        /// <summary>
        /// 获取相关配置
        /// </summary>
        public HttpApiConfig ApiConfig { get; private set; }

        /// <summary>
        /// 获取拦截器
        /// </summary>
        public IApiInterceptor ApiInterceptor { get; private set; }

        /// <summary>
        /// http客户端的基类
        /// </summary>
        /// <param name="interceptor">拦截器</param>
        public HttpApiClient(IApiInterceptor interceptor)
        {
            if (interceptor != null)
            {
                this.ApiInterceptor = interceptor;
                this.ApiConfig = interceptor.ApiConfig;
            }
        }

        /// <summary>
        /// 获取或设置一个站点内的连接数限制
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int ConnectionLimit
        {
            get
            {
                return ServicePointManager.DefaultConnectionLimit;
            }
            set
            {
                ServicePointManager.DefaultConnectionLimit = value;
            }
        }

        /// <summary>
        /// 创建请求接口的实例
        /// 关联新建的HttpApiConfig对象
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static TInterface Create<TInterface>() where TInterface : class, IDisposable
        {
            return Create<TInterface>(null);
        }

        /// <summary>
        /// 创建请求接口的实例
        /// </summary>
        /// <typeparam name="TInterface">请求接口</typeparam>
        /// <param name="httpApiConfig">接口配置</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static TInterface Create<TInterface>(HttpApiConfig httpApiConfig) where TInterface : class
        {
            if (httpApiConfig == null)
            {
                httpApiConfig = new HttpApiConfig();
            }

            var interceptor = new ApiInterceptor(httpApiConfig);
            return ProxyGenerator.CreateInterfaceProxyWithoutTarget<TInterface>(interceptor);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (this.ApiConfig != null && this.ApiConfig.IsDisposed == false)
            {
                this.ApiConfig.Dispose();
            }
        }
    }
}
