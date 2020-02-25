#if NETCOREAPP2_1
using System;

namespace WebApiClient
{
    /// <summary>
    /// Represents the HttpApi option
    /// </summary>
    public class HttpApiOptions
    {
        /// <summary>
        /// Gets or sets whether to perform input validity verification on the attribute value of the parameter
        /// </summary>
        public bool? UseParameterPropertyValidate { get; set; }

        /// <summary>
        /// Gets or sets whether input validity validation is performed on the attribute value of the returned value
        /// </summary>
        public bool? UseReturnValuePropertyValidate { get; set; }


        /// <summary>
        /// Get or set the full host domain name of the HTTP service
        /// E.g. http://www.webapiclient.com
        /// HttpHost value is set, HttpHostAttribute will be invalid  
        /// </summary>
        public Uri HttpHost { get; set; }

        /// <summary>
        /// Gets or sets the default format used by serialization when requesting   
        /// Affects serialization of JsonFormatter or KeyValueFormatter   
        /// </summary>
        public FormatOptions FormatOptions { get; set; }


        /// <summary>
        /// Get or set Xml formatting tool
        /// </summary>
        public IXmlFormatter XmlFormatter { get; set; }


        /// <summary>
        /// Get or set Json formatting tool
        /// </summary>
        public IJsonFormatter JsonFormatter { get; set; }


        /// <summary>
        /// Gets or sets the KeyValue formatting tool
        /// </summary>
        public IKeyValueFormatter KeyValueFormatter { get; set; }


        /// <summary>
        /// Gets or sets the cache provider of the API
        /// </summary>
        public IResponseCacheProvider ResponseCacheProvider { get; set; }
    }

    /// <summary>
    /// Represents the HttpApi option
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class HttpApiOptions<THttpApi> : HttpApiOptions where THttpApi : class, IHttpApi
    {
    }
}
#endif