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
