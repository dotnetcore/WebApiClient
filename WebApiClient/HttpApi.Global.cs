using System;
using System.Linq;
using System.Net;

namespace WebApiClient
{
    /// <summary>
    /// 提供HttpApi的创建、注册和解析   
    /// </summary>
    public partial class HttpApi
    {
        /// <summary>
        /// 一个站点内的默认连接数限制
        /// </summary>
        private static int maxConnections = 128;

        /// <summary>
        /// 获取或设置一个站点内的默认最大连接数
        /// 这个值在初始化HttpClientHandler时使用
        /// 默认值为128
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
        /// 获取或设置安全协议版本
        /// </summary>
        public static SecurityProtocolType SecurityProtocol
        {
            get => ServicePointManager.SecurityProtocol;
            set => ServicePointManager.SecurityProtocol = value;
        }

        /// <summary>
        /// 静态构造函数
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
        /// 调试视图
        /// </summary>
        private partial class DebugView
        {
            /// <summary>
            /// 获取或设置一个站点内的默认最大连接数      
            /// </summary>
            public static int MaxConnections => HttpApi.MaxConnections;

#if NET45 || NET46
            /// <summary>
            /// 获取或设置安全协议版本
            /// </summary>
            public static SecurityProtocolType SecurityProtocol => HttpApi.SecurityProtocol;
#endif             
        }
    }
}
