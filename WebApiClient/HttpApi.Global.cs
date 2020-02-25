using System;
using System.Linq;
using System.Net;

namespace WebApiClient
{
    /// <summary>
    /// Provides creation, registration, and parsing of HttpApi  
    /// </summary>
    public partial class HttpApi
    {
        /// <summary>
        /// Limit of default connections within a site
        /// </summary>
        private static int maxConnections = 128;

        /// <summary>
        /// Gets or sets the default maximum number of connections within a site
        /// This value is used when initializing the HttpClientHandler
        /// The default value is 128
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static int MaxConnections
        {
            get
            {
                return maxConnections;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaxConnections));
                }
                maxConnections = value;
            }
        }

#if NET45 || NET46
        /// <summary>
        /// Gets or sets the security protocol version
        /// </summary>
        public static SecurityProtocolType SecurityProtocol
        {
            get => ServicePointManager.SecurityProtocol;
            set => ServicePointManager.SecurityProtocol = value;
        }

        /// <summary>
        /// Static constructor
        /// </summary>
        static HttpApi()
        {
            ServicePointManager.SecurityProtocol = Enum
                .GetValues(typeof(SecurityProtocolType))
                .Cast<SecurityProtocolType>()
                .Aggregate((cur, next) => cur | next);
        }
#endif



        /// <summary>
        /// Debug view
        /// </summary>
        private partial class DebugView
        {
            /// <summary>
            /// Gets or sets the default maximum number of connections within a site      
            /// </summary>
            public static int MaxConnections => HttpApi.MaxConnections;

#if NET45 || NET46
            /// <summary>
            /// Gets or sets the security protocol version
            /// </summary>
            public static SecurityProtocolType SecurityProtocol => HttpApi.SecurityProtocol;
#endif             
        }
    }
}
