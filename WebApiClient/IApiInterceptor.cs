using System;
using System.Reflection;

namespace WebApiClient
{
    /// <summary>
    /// Define the behavior of the http interface interceptor
    /// </summary>
    public interface IApiInterceptor : IDisposable
    {
        /// <summary>
        /// Intercepting method calls
        /// </summary>
        /// <param name="target">Examples of interfaces</param>
        /// <param name="method">Interface methods</param>
        /// <param name="parameters">Parameter collection of the interface</param>
        /// <returns></returns>
        object Intercept(object target, MethodInfo method, object[] parameters);
    }
}
