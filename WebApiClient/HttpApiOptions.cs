# if NETCOREAPP2_1
using System;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApi选项
    /// </summary>
    public class HttpApiOptions<THttpApi> where THttpApi : class, IHttpApi
    {
        /// <summary>
        /// 获取或设置是否对参数的属性值进行输入有效性验证
        /// </summary>
        public bool? UseParameterPropertyValidate { get; set; }

        /// <summary>
        /// 获取或设置是否对返回值的属性值进行输入有效性验证
        /// </summary>
        public bool? UseReturnValuePropertyValidate { get; set; }


        /// <summary>
        /// 获取或设置Http服务完整主机域名
        /// 例如http://www.webapiclient.com
        /// 设置了HttpHost值，HttpHostAttribute将失效  
        /// </summary>
        public Uri HttpHost { get; set; }

        /// <summary>
        /// 获取或设置请求时序列化使用的默认格式   
        /// 影响JsonFormatter或KeyValueFormatter的序列化   
        /// </summary>
        public FormatOptions FormatOptions { get; set; }


        /// <summary>
        /// 获取或设置Xml格式化工具
        /// </summary>
        public IXmlFormatter XmlFormatter { get; set; }


        /// <summary>
        /// 获取或设置Json格式化工具
        /// </summary>
        public IJsonFormatter JsonFormatter { get; set; }


        /// <summary>
        /// 获取或设置KeyValue格式化工具
        /// </summary>
        public IKeyValueFormatter KeyValueFormatter { get; set; }


        /// <summary>
        /// 获取或设置Api的缓存提供者
        /// </summary>
        public IResponseCacheProvider ResponseCacheProvider { get; set; }
    }
}
#endif