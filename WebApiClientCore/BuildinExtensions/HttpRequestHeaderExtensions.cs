using System.Collections.Generic;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供请求头枚举到名称的转换
    /// </summary>
    static class HttpRequestHeaderExtensions
    {
        /// <summary>
        /// 请求头枚举和名称的缓存
        /// </summary>
        private static readonly Dictionary<HttpRequestHeader, string> cache = new()
        {
            [HttpRequestHeader.CacheControl] = nameof(HttpRequestHeader.CacheControl),
            [HttpRequestHeader.Connection] = nameof(HttpRequestHeader.Connection),
            [HttpRequestHeader.Date] = nameof(HttpRequestHeader.Date),
            [HttpRequestHeader.KeepAlive] = nameof(HttpRequestHeader.KeepAlive),
            [HttpRequestHeader.Pragma] = nameof(HttpRequestHeader.Pragma),
            [HttpRequestHeader.Trailer] = nameof(HttpRequestHeader.Trailer),
            [HttpRequestHeader.TransferEncoding] = nameof(HttpRequestHeader.TransferEncoding),
            [HttpRequestHeader.Upgrade] = nameof(HttpRequestHeader.Upgrade),
            [HttpRequestHeader.Via] = nameof(HttpRequestHeader.Via),
            [HttpRequestHeader.Warning] = nameof(HttpRequestHeader.Warning),
            [HttpRequestHeader.Allow] = nameof(HttpRequestHeader.Allow),
            [HttpRequestHeader.ContentLength] = nameof(HttpRequestHeader.ContentLength),
            [HttpRequestHeader.ContentType] = nameof(HttpRequestHeader.ContentType),
            [HttpRequestHeader.ContentEncoding] = nameof(HttpRequestHeader.ContentEncoding),
            [HttpRequestHeader.ContentLanguage] = nameof(HttpRequestHeader.ContentLanguage),
            [HttpRequestHeader.ContentLocation] = nameof(HttpRequestHeader.ContentLocation),
            [HttpRequestHeader.ContentMd5] = nameof(HttpRequestHeader.ContentMd5),
            [HttpRequestHeader.ContentRange] = nameof(HttpRequestHeader.ContentRange),
            [HttpRequestHeader.Expires] = nameof(HttpRequestHeader.Expires),
            [HttpRequestHeader.LastModified] = nameof(HttpRequestHeader.LastModified),
            [HttpRequestHeader.Accept] = nameof(HttpRequestHeader.Accept),
            [HttpRequestHeader.AcceptCharset] = nameof(HttpRequestHeader.AcceptCharset),
            [HttpRequestHeader.AcceptEncoding] = nameof(HttpRequestHeader.AcceptEncoding),
            [HttpRequestHeader.AcceptLanguage] = nameof(HttpRequestHeader.AcceptLanguage),
            [HttpRequestHeader.Authorization] = nameof(HttpRequestHeader.Authorization),
            [HttpRequestHeader.Cookie] = nameof(HttpRequestHeader.Cookie),
            [HttpRequestHeader.Expect] = nameof(HttpRequestHeader.Expect),
            [HttpRequestHeader.From] = nameof(HttpRequestHeader.From),
            [HttpRequestHeader.Host] = nameof(HttpRequestHeader.Host),
            [HttpRequestHeader.IfMatch] = nameof(HttpRequestHeader.IfMatch),
            [HttpRequestHeader.IfModifiedSince] = nameof(HttpRequestHeader.IfModifiedSince),
            [HttpRequestHeader.IfNoneMatch] = nameof(HttpRequestHeader.IfNoneMatch),
            [HttpRequestHeader.IfRange] = nameof(HttpRequestHeader.IfRange),
            [HttpRequestHeader.IfUnmodifiedSince] = nameof(HttpRequestHeader.IfUnmodifiedSince),
            [HttpRequestHeader.MaxForwards] = nameof(HttpRequestHeader.MaxForwards),
            [HttpRequestHeader.ProxyAuthorization] = nameof(HttpRequestHeader.ProxyAuthorization),
            [HttpRequestHeader.Referer] = nameof(HttpRequestHeader.Referer),
            [HttpRequestHeader.Range] = nameof(HttpRequestHeader.Range),
            [HttpRequestHeader.Te] = nameof(HttpRequestHeader.Te),
            [HttpRequestHeader.Translate] = nameof(HttpRequestHeader.Translate),
            [HttpRequestHeader.UserAgent] = nameof(HttpRequestHeader.UserAgent)
        };

        /// <summary>
        /// 请求头枚举和名称的缓存
        /// </summary>
        private static readonly Dictionary<HttpRequestHeader, string> displayCache = new()
        {
            [HttpRequestHeader.CacheControl] = "Cache-Control",
            [HttpRequestHeader.Connection] = "Connection",
            [HttpRequestHeader.Date] = "Date",
            [HttpRequestHeader.KeepAlive] = "Keep-Alive",
            [HttpRequestHeader.Pragma] = "Pragma",
            [HttpRequestHeader.Trailer] = "Trailer",
            [HttpRequestHeader.TransferEncoding] = "Transfer-Encoding",
            [HttpRequestHeader.Upgrade] = "Upgrade",
            [HttpRequestHeader.Via] = "Via",
            [HttpRequestHeader.Warning] = "Warning",
            [HttpRequestHeader.Allow] = "Allow",
            [HttpRequestHeader.ContentLength] = "Content-Length",
            [HttpRequestHeader.ContentType] = "Content-Type",
            [HttpRequestHeader.ContentEncoding] = "Content-Encoding",
            [HttpRequestHeader.ContentLanguage] = "Content-Language",
            [HttpRequestHeader.ContentLocation] = "Content-Location",
            [HttpRequestHeader.ContentMd5] = "Content-MD5",
            [HttpRequestHeader.ContentRange] = "Content-Range",
            [HttpRequestHeader.Expires] = "Expires",
            [HttpRequestHeader.LastModified] = "Last-Modified",
            [HttpRequestHeader.Accept] = "Accept",
            [HttpRequestHeader.AcceptCharset] = "Accept-Charset",
            [HttpRequestHeader.AcceptEncoding] = "Accept-Encoding",
            [HttpRequestHeader.AcceptLanguage] = "Accept-Language",
            [HttpRequestHeader.Authorization] = "Authorization",
            [HttpRequestHeader.Cookie] = "Cookie",
            [HttpRequestHeader.Expect] = "Expect",
            [HttpRequestHeader.From] = "From",
            [HttpRequestHeader.Host] = "Host",
            [HttpRequestHeader.IfMatch] = "If-Match",
            [HttpRequestHeader.IfModifiedSince] = "If-Modified-Since",
            [HttpRequestHeader.IfNoneMatch] = "If-None-Match",
            [HttpRequestHeader.IfRange] = "If-Range",
            [HttpRequestHeader.IfUnmodifiedSince] = "If-Unmodified-Since",
            [HttpRequestHeader.MaxForwards] = "Max-Forwards",
            [HttpRequestHeader.ProxyAuthorization] = "Proxy-Authorization",
            [HttpRequestHeader.Referer] = "Referer",
            [HttpRequestHeader.Range] = "Range",
            [HttpRequestHeader.Te] = "TE",
            [HttpRequestHeader.Translate] = "Translate",
            [HttpRequestHeader.UserAgent] = "User-Agent"
        };

        /// <summary>
        /// 返回枚举的DisplayName
        /// </summary>
        /// <param name="header">请求头枚举</param>
        /// <returns></returns>
        private static string GetHeaderName(this HttpRequestHeader header)
        {
            return displayCache.TryGetValue(header, out var name) ? name : header.ToString();
        }

        /// <summary>
        /// 转换为header名
        /// </summary>
        /// <param name="header">请求头枚举</param>
        /// <returns></returns>
        public static string ToHeaderName(this HttpRequestHeader header)
        {
            return cache.TryGetValue(header, out var name) ? name : header.ToString();
        }
    }
}
