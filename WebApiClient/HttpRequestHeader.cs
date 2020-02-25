﻿using System.ComponentModel.DataAnnotations;

namespace WebApiClient
{
    /// <summary>
    /// HTTP headers that can be specified in client requests
    /// </summary>
    public enum HttpRequestHeader
    {
        /// <summary>
        /// Cache-Control header, specifying the instructions that all cache control mechanisms on the request / response chain must obey
        /// </summary>
        [Display(Name = "Cache-Control")]
        CacheControl = 0,

        /// <summary>
        /// Connection header, specifying options required for a particular connection
        /// </summary>
        [Display(Name = "Connection")]
        Connection = 1,

        /// <summary>
        /// Date header, specifying the date and time when the creation of the request started
        /// </summary>
        [Display(Name = "Date")]
        Date = 2,

        /// <summary>
        /// Keep-Alive header, specifying parameters to maintain persistent connections
        /// </summary>
        [Display(Name = "Keep-Alive")]
        KeepAlive = 3,

        /// <summary>
        /// Pragma header specifying implementation-specific instructions that can be applied to any proxy on the request / response chain
        /// </summary>
        [Display(Name = "Pragma")]
        Pragma = 4,

        /// <summary>
        /// Trailer header, which specifies that the header field is displayed at the end of the message encoded in chunked transfer encoding
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
        /// If-Unmodified-Since header specifying that the requested operation is performed only if the requested resource has been modified since the indicated date and time
        /// </summary>
        [Display(Name = "If-Unmodified-Since")]
        IfUnmodifiedSince = 33,

        /// <summary>
        /// Max-Forwards header, specifying an integer indicating the number of times this request can be forwarded
        /// </summary>
        [Display(Name = "Max-Forwards")]
        MaxForwards = 34,

        /// <summary>
        /// Proxy-Authorization header, specifying the credentials the client presents to authenticate itself with the proxy
        /// </summary>
        [Display(Name = "Proxy-Authorization")]
        ProxyAuthorization = 35,

        /// <summary>
        /// Referer header specifying the URI of the resource from which the request URI was obtained
        /// </summary>
        [Display(Name = "Referer")]
        Referer = 36,

        /// <summary>
        /// Range header, specifying a subrange of the client request's response returned in place of the entire response
        /// </summary>
        [Display(Name = "Range")]
        Range = 37,

        /// <summary>
        /// TE header specifying the acceptable transport encoding for the response
        /// </summary>
        [Display(Name = "TE")]
        Te = 38,

        /// <summary>
        /// Translate header, a Microsoft extension to the HTTP specification for use with WebDAV functionality
        /// </summary>
        [Display(Name = "Translate")]
        Translate = 39,

        /// <summary>
        /// User-Agent header, specifying information about the client agent
        /// </summary>
        [Display(Name = "User-Agent")]
        UserAgent = 40
    }
}


