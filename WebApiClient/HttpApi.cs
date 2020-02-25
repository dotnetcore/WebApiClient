using System;

namespace WebApiClient
{
    /// <summary>
    /// Provides creation, registration, and parsing of HttpApi   
    /// </summary> 
    public abstract partial class HttpApi
    {
        /// <summary>
        /// Get the interceptor
        /// </summary>
        public IApiInterceptor ApiInterceptor { get; }

        /// <summary>
        /// Base class of http interface proxy class
        /// </summary>
        /// <param name="apiInterceptor">Interceptor</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpApi(IApiInterceptor apiInterceptor)
        {
            this.ApiInterceptor = apiInterceptor ?? throw new ArgumentNullException(nameof(apiInterceptor));
        }
    }
}
