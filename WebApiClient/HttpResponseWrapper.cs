using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// Represents the HTTP response wrapper abstract class
    /// Its subclasses can be declared as the return type of the interface
    /// </summary>
    public abstract class HttpResponseWrapper
    {
        /// <summary>
        /// Get response message
        /// </summary>
        protected HttpResponseMessage HttpResponse { get; }

        /// <summary>
        /// http response wrapper abstract class
        /// </summary>
        /// <param name="httpResponse">Response message</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpResponseWrapper(HttpResponseMessage httpResponse)
        {
            this.HttpResponse = httpResponse ?? throw new ArgumentNullException(nameof(httpResponse));
        }
    }
}
