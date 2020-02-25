using System;
using System.Net.Http;

namespace WebApiClient
{
    /// <summary>
    /// Define the interface of HttpApiFactory
    /// </summary>
    public interface IHttpApiFactory
    {
        /// <summary>
        /// Get interface type
        /// </summary>
        Type InterfaceType { get; }

        /// <summary>
        /// Create a proxy instance of the interface
        /// </summary>
        /// <returns></returns>
        HttpApi CreateHttpApi();

        /// <summary>
        /// Configure HttpApiConfig
        /// </summary>
        /// <param name="options">Configuration delegation</param>
        void ConfigureHttpApiConfig(Action<HttpApiConfig> options);

        /// <summary>
        /// Configure the creation of HttpMessageHandler
        /// </summary>
        /// <param name="factory">Create delegate</param>
        void ConfigureHttpMessageHandler(Func<HttpMessageHandler> factory);
    }

    /// <summary>
    /// Defines the interface of the HttpApi factory
    /// </summary>
    /// <typeparam name="TInterface"></typeparam>
    public interface IHttpApiFactory<TInterface> : IHttpApiFactory where TInterface : class, IHttpApi
    {
        /// <summary>
        /// Create a proxy instance of the interface
        /// </summary>
        /// <returns></returns>
        new TInterface CreateHttpApi();
    }
}
