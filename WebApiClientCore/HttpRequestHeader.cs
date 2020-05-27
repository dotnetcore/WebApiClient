using System.ComponentModel.DataAnnotations;

namespace WebApiClientCore
{
    /// <summary>
    /// 可以在客户端请求中指定的 HTTP 标头
    /// </summary>
    public enum HttpRequestHeader : byte
    {
        /// <summary>
        /// Cache-Control 标头，指定请求/响应链上所有缓存控制机制必须服从的指令
        /// </summary>
        [Display(Name = "Cache-Control")]
        CacheControl = 0,

        /// <summary>
        /// Connection 标头，指定特定连接需要的选项
        /// </summary>
        [Display(Name = "Connection")]
        Connection = 1,

        /// <summary>
        /// Date 标头，指定开始创建请求的日期和时间
        /// </summary>
        [Display(Name = "Date")]
        Date = 2,

        /// <summary>
        /// Keep-Alive 标头，指定用以维护持久性连接的参数
        /// </summary>
        [Display(Name = "Keep-Alive")]
        KeepAlive = 3,

        /// <summary>
        ///  Pragma 标头，指定可应用于请求/响应链上的任何代理的特定于实现的指令
        /// </summary>
        [Display(Name = "Pragma")]
        Pragma = 4,

        /// <summary>
        /// Trailer 标头，指定标头字段显示在以 chunked 传输编码方式编码的消息的尾部
        /// </summary>
        [Display(Name = "Trailer")]
        Trailer = 5,

        /// <summary>
        /// Transfer-Encoding 标头，指定对消息正文应用的转换的类型（如果有）
        /// </summary>
        [Display(Name = "Transfer-Encoding")]
        TransferEncoding = 6,

        /// <summary>
        /// Upgrade 标头，指定客户端支持的附加通信协议
        /// </summary>
        [Display(Name = "Upgrade")]
        Upgrade = 7,

        /// <summary>
        /// Via 标头，指定网关和代理程序要使用的中间协议
        /// </summary>
        [Display(Name = "Via")]
        Via = 8,

        /// <summary>
        /// Warning 标头，指定关于可能未在消息中反映的消息的状态或转换的附加信息
        /// </summary>
        [Display(Name = "Warning")]
        Warning = 9,

        /// <summary>
        /// Allow 标头，指定支持的 HTTP 方法集
        /// </summary>
        [Display(Name = "Allow")]
        Allow = 10,

        /// <summary>
        /// Content-Length 标头，指定伴随正文数据的长度（以字节为单位）
        /// </summary>
        [Display(Name = "Content-Length")]
        ContentLength = 11,

        /// <summary>
        /// Content-Type 标头，指定伴随正文数据的 MIME 类型
        /// </summary>
        [Display(Name = "Content-Type")]
        ContentType = 12,

        /// <summary>
        /// Content-Encoding 标头，指定已应用于伴随正文数据的编码
        /// </summary>
        [Display(Name = "Content-Encoding")]
        ContentEncoding = 13,

        /// <summary>
        /// Content-Language 标头，指定伴随正文数据的自然语言
        /// </summary>
        [Display(Name = "Content-Language")]
        ContentLanguage = 14,

        /// <summary>
        /// Content-Location 标头，指定可从其中获得伴随正文的 URI
        /// </summary>
        [Display(Name = "Content-Location")]
        ContentLocation = 15,

        /// <summary>
        /// Content-MD5 标头，指定伴随正文数据的 MD5 摘要，用于提供端到端消息完整性检查
        /// </summary>
        [Display(Name = "Content-MD5")]
        ContentMd5 = 16,

        /// <summary>
        /// Content-Range 标头，指定在完整正文中应用伴随部分正文数据的位置
        /// </summary>
        [Display(Name = "Content-Range")]
        ContentRange = 17,

        /// <summary>
        ///  Expires 标头，指定日期和时间，在此之后伴随的正文数据应视为陈旧的
        /// </summary>
        [Display(Name = "Expires")]
        Expires = 18,

        /// <summary>
        /// Last-Modified 标头，指定上次修改伴随的正文数据的日期和时间
        /// </summary>
        [Display(Name = "Last-Modified")]
        LastModified = 19,

        /// <summary>
        /// Accept 标头，指定响应可接受的 MIME 类型
        /// </summary>
        [Display(Name = "Accept")]
        Accept = 20,

        /// <summary>
        /// Accept-Charset 标头，指定响应可接受的字符集
        /// </summary>
        [Display(Name = "Accept-Charset")]
        AcceptCharset = 21,

        /// <summary>
        /// Accept-Encoding 标头，指定响应可接受的内容编码
        /// </summary>
        [Display(Name = "Accept-Encoding")]
        AcceptEncoding = 22,

        /// <summary>
        /// Accept-Language 标头，指定响应首选的自然语言
        /// </summary>
        [Display(Name = "Accept-Language")]
        AcceptLanguage = 23,

        /// <summary>
        /// Authorization 标头，指定客户端为向服务器验证自身身份而出示的凭据
        /// </summary>
        [Display(Name = "Authorization")]
        Authorization = 24,

        /// <summary>
        /// Cookie 标头，指定向服务器提供的 Cookie 数据
        /// </summary>
        [Display(Name = "Cookie")]
        Cookie = 25,

        /// <summary>
        /// Expect 标头，指定客户端要求的特定服务器行为
        /// </summary>
        [Display(Name = "Expect")]
        Expect = 26,

        /// <summary>
        /// From 标头，指定控制请求用户代理的用户的 Internet 电子邮件地址
        /// </summary>
        [Display(Name = "From")]
        From = 27,

        /// <summary>
        /// Host 标头，指定所请求资源的主机名和端口号
        /// </summary>
        [Display(Name = "Host")]
        Host = 28,

        /// <summary>
        /// If-Match 标头，指定仅当客户端的指示资源的缓存副本是最新的时，才执行请求的操作
        /// </summary>
        [Display(Name = "If-Match")]
        IfMatch = 29,

        /// <summary>
        /// If-Modified-Since 标头，指定仅当自指示的数据和时间之后修改了请求的资源时，才执行请求的操作
        /// </summary>
        [Display(Name = "If-Modified-Since")]
        IfModifiedSince = 30,

        /// <summary>
        /// If-None-Match 标头，指定仅当客户端的指示资源的缓存副本都不是最新的时，才执行请求的操作
        /// </summary>
        [Display(Name = "If-None-Match")]
        IfNoneMatch = 31,

        /// <summary>
        /// If-Range 标头，指定如果客户端的缓存副本是最新的，仅发送指定范围的请求资源
        /// </summary>
        [Display(Name = "If-Range")]
        IfRange = 32,

        /// <summary>
        /// If-Unmodified-Since 标头，指定仅当自指示的日期和时间之后修改了请求的资源时，才执行请求的操作
        /// </summary>
        [Display(Name = "If-Unmodified-Since")]
        IfUnmodifiedSince = 33,

        /// <summary>
        /// Max-Forwards 标头，指定一个整数，表示此请求还可转发的次数
        /// </summary>
        [Display(Name = "Max-Forwards")]
        MaxForwards = 34,

        /// <summary>
        /// Proxy-Authorization 标头，指定客户端为向代理验证自身身份而出示的凭据
        /// </summary>
        [Display(Name = "Proxy-Authorization")]
        ProxyAuthorization = 35,

        /// <summary>
        /// Referer 标头，指定从中获得请求 URI 的资源的 URI
        /// </summary>
        [Display(Name = "Referer")]
        Referer = 36,

        /// <summary>
        /// Range 标头，指定代替整个响应返回的客户端请求的响应的子范围
        /// </summary>
        [Display(Name = "Range")]
        Range = 37,

        /// <summary>
        /// TE 标头，指定响应可接受的传输编码方式
        /// </summary>
        [Display(Name = "TE")]
        Te = 38,

        /// <summary>
        /// Translate 标头，与 WebDAV 功能一起使用的 HTTP 规范的 Microsoft 扩展
        /// </summary>
        [Display(Name = "Translate")]
        Translate = 39,

        /// <summary>
        /// User-Agent 标头，指定有关客户端代理的信息
        /// </summary>
        [Display(Name = "User-Agent")]
        UserAgent = 40
    }
}


