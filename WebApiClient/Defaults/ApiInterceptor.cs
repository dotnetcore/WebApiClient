using System;
using System.Reflection;
using WebApiClient.Contexts;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示http接口调用的拦截器
    /// </summary>
    public class ApiInterceptor : IApiInterceptor
    {
        /// <summary>
        /// ApiActionDescriptor缓存
        /// </summary>
        private static readonly ConcurrentCache<MethodInfo, ApiActionDescriptor> descriptorCache;

        /// <summary>
        /// 获取相关的配置
        /// </summary>
        public HttpApiConfig HttpApiConfig { get; private set; }


        /// <summary>
        /// http接口调用的拦截器
        /// </summary>
        static ApiInterceptor()
        {
            descriptorCache = new ConcurrentCache<MethodInfo, ApiActionDescriptor>();
        }

        /// <summary>
        /// http接口调用的拦截器
        /// </summary>
        /// <param name="httpApiConfig">httpApi配置</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiInterceptor(HttpApiConfig httpApiConfig)
        {
            this.HttpApiConfig = httpApiConfig ?? throw new ArgumentNullException(nameof(httpApiConfig));
        }

        /// <summary>
        /// 拦截方法的调用
        /// </summary>
        /// <param name="target">接口的实例</param>
        /// <param name="method">接口的方法</param>
        /// <param name="parameters">接口的参数集合</param>
        /// <returns></returns>
        public virtual object Intercept(object target, MethodInfo method, object[] parameters)
        {
            var descriptor = this.GetApiActionDescriptor(method, parameters);
            var apiTask = descriptor.Return.DataType.ITaskFactory.Invoke() as ApiTask;

            apiTask.HttpApi = target as IHttpApi;
            apiTask.HttpApiConfig = this.HttpApiConfig;
            apiTask.ApiActionDescriptor = descriptor;

            if (descriptor.Return.IsITaskDefinition == false)
            {
                return apiTask.Execute();
            }
            else
            {
                return apiTask;
            }
        }

        /// <summary>
        /// 获取api的描述
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="parameters">参数值集合</param>
        /// <returns></returns>
        protected virtual ApiActionDescriptor GetApiActionDescriptor(MethodInfo method, object[] parameters)
        {
            return descriptorCache.GetOrAdd(method, m => ApiActionDescriptor.Create(m)).Clone(parameters);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            this.HttpApiConfig.Dispose();
        }
    }
}
