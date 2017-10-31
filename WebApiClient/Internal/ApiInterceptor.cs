using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示Api的拦截器
    /// </summary>
    class ApiInterceptor : IApiInterceptor
    {
        /// <summary>
        /// httpApi配置
        /// </summary>
        private readonly HttpApiConfig httpApiConfig;

        /// <summary>
        /// dispose方法
        /// </summary>
        private static readonly MethodInfo disposeMethod = typeof(IDisposable).GetMethods().FirstOrDefault();

        /// <summary>
        /// Api的拦截器
        /// </summary>
        /// <param name="apiConfig">httpApi配置</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiInterceptor(HttpApiConfig apiConfig)
        {
            if (apiConfig == null)
            {
                throw new ArgumentNullException("apiConfig");
            }
            apiConfig.SetNullPropertyAsDefault();
            this.httpApiConfig = apiConfig;
        }

        /// <summary>
        /// 拦截方法的调用
        /// </summary>
        /// <param name="target">接口的实例</param>
        /// <param name="method">接口的方法</param>
        /// <param name="parameters">接口的参数集合</param>
        /// <returns></returns>
        public object Intercept(object target, MethodInfo method, object[] parameters)
        {
            if (method.Equals(disposeMethod) == true)
            {
                this.httpApiConfig.Dispose();
                return null;
            }

            var apiActionDescripter = this.GetApiActionDescriptor(method, parameters);
            var apiTask = ApiTask.CreateInstance(this.httpApiConfig, apiActionDescripter);

            if (apiActionDescripter.Return.GenericType == typeof(ITask<>))
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
        private ApiActionDescriptor GetApiActionDescriptor(MethodInfo method, object[] parameters)
        {
            var cache = ApiDescriptorCache.GetApiActionDescriptor(method);
            var actionDescripter = cache.Clone() as ApiActionDescriptor;

            for (var i = 0; i < actionDescripter.Parameters.Length; i++)
            {
                actionDescripter.Parameters[i].Value = parameters[i];
            }
            return actionDescripter;
        }
    }
}
