using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供Uri扩展
    /// </summary>
    static class UriExtensions
    {
        /// <summary>
        /// 转换为相对uri
        /// </summary>
        /// <param name="uri">uri</param> 
        /// <returns></returns>
        public static string ToRelativeUri(this Uri uri)
        {
            if (uri.IsAbsoluteUri == false)
            {
                return uri.OriginalString;
            }

            var path = uri.OriginalString.AsSpan().Slice(uri.Scheme.Length + 3);
            var index = path.IndexOf('/');
            if (index < 0)
            {
                return "/";
            }

            return path.Slice(index).ToString();
        }

        /// <summary>
        /// 替换带有花括号的参数的值
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name">参数名称，不带花括号</param>
        /// <param name="value">参数的值</param>
        /// <param name="replaced">是否替换成功</param>
        /// <returns></returns>
        public static Uri Replace(this Uri source, string name, string? value, out bool replaced)
        {
            replaced = false;
            var uriString = source.OriginalString;
            if (uriString.Contains('{') == false)
            {
                return source;
            }

            if (string.IsNullOrEmpty(value) == false)
            {
                value = Uri.EscapeDataString(value);
            }

            var newUri = uriString.RepaceIgnoreCase($"{{{name}}}", value, out replaced);
            return replaced ? new Uri(newUri, UriKind.RelativeOrAbsolute) : source;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数的值</param>
        /// <returns></returns>
        public static Uri AddQuery(this Uri source, string name, string? value)
        {
            var uriSpan = source.OriginalString.AsSpan();
            var fragmentSpan = source.IsAbsoluteUri ? source.Fragment : GetFragment(uriSpan);
            var baseSpan = uriSpan.Slice(0, uriSpan.Length - fragmentSpan.Length).TrimEnd('?').TrimEnd('&');
            var concat = baseSpan.LastIndexOf('?') < 0 ? '?' : '&';
            var nameSpan = Uri.EscapeDataString(name);
            var valueSpan = string.IsNullOrEmpty(value)
                ? ReadOnlySpan<char>.Empty
                : Uri.EscapeDataString(value).AsSpan();

            var builder = new ValueStringBuilder(stackalloc char[256]);
            builder.Append(baseSpan);
            builder.Append(concat);
            builder.Append(nameSpan);
            builder.Append('=');
            builder.Append(valueSpan);
            builder.Append(fragmentSpan);

            return new Uri(builder.ToString(), UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// 获取Fragment
        /// </summary>
        /// <param name="uriSpan">Uri</param>
        /// <returns></returns>
        private static ReadOnlySpan<char> GetFragment(ReadOnlySpan<char> uriSpan)
        {
            var index = uriSpan.LastIndexOf('#');
            return index < 0 ? ReadOnlySpan<char>.Empty : uriSpan.Slice(index);
        }
    }
}
