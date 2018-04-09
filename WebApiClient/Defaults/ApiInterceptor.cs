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
        /// 获取相关的配置
        /// </summary>
        public HttpApiConfig ApiConfig { get; private set; }

        /// <summary>
        /// http接口调用的拦截器
        /// </summary>
        /// <param name="apiConfig">httpApi配置</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiInterceptor(HttpApiConfig apiConfig)
        {
            this.ApiConfig = apiConfig ?? throw new ArgumentNullException(nameof(apiConfig));
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
            var apiActionDescripter = this.GetApiActionDescriptor(method, parameters);
            var apiTask = ApiTask.CreateInstance(this.ApiConfig, apiActionDescripter);

            if (apiActionDescripter.Return.IsITaskDefinition == true)
            {
                return apiTask;
            }
            else
            {
                return apiTask.InvokeAsync();
            }
        }

        /// <summary>
        /// 获取api的描述
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="parameters">参数集合</param>
        /// <returns></returns>
        protected virtual ApiActionDescriptor GetApiActionDescriptor(MethodInfo method, object[] parameters)
        {
            var actionDescripter = ApiDescriptorCache.GetApiActionDescriptor(method).Clone();
            for (var i = 0; i < actionDescripter.Parameters.Length; i++)
            {
                actionDescripter.Parameters[i].Value = parameters[i];
            }
            return actionDescripter;
        }
    }
}
