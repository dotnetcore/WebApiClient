using System;

namespace WebApiClientCore.Internals
{
    /// <summary>
    /// 表示Uri值
    /// </summary>
    public readonly ref struct UriValue
    {
        /// <summary>
        /// uri
        /// </summary>
        private readonly Uri? uri;

        /// <summary>
        /// uri文本
        /// </summary>
        private readonly string uriString;

        /// <summary>
        /// Uri值
        /// </summary>
        /// <param name="uri"></param>
        public UriValue(Uri uri)
        {
            this.uri = uri;
            this.uriString = uri.OriginalString;
        }

        /// <summary>
        /// Uri文本
        /// </summary>
        /// <param name="uriString"></param>
        public UriValue(string uriString)
        {
            this.uri = null;
            this.uriString = uriString;
        }

        /// <summary>
        /// 返回替换带有花括号参数后的UriString
        /// </summary> 
        /// <param name="name">参数名称，不带花括号</param>
        /// <param name="value">参数的值</param>
        /// <param name="replaced">是否替换成功</param>
        /// <returns></returns>
        public UriValue Replace(string name, string? value, out bool replaced)
        {
            replaced = false;
            if (this.uriString.Contains('{') == false)
            {
                return this;
            }

            if (string.IsNullOrEmpty(value) == false)
            {
                value = Uri.EscapeDataString(value);
            }

            var newUri = uriString.RepaceIgnoreCase($"{{{name}}}", value, out replaced);
            return replaced ? new UriValue(newUri) : this;
        }

        /// <summary>
        /// 返回添加参数后的UriString
        /// </summary> 
        /// <param name="name">参数名称</param>
        /// <param name="value">参数的值</param>
        /// <returns></returns>
        public UriValue AddQuery(string name, string? value)
        {
            var uriSpan = this.uriString.AsSpan();
            var fragmentSpan = GetFragment(uriSpan);
            var baseSpan = uriSpan.Slice(0, uriSpan.Length - fragmentSpan.Length).TrimEnd('?').TrimEnd('&');
            var concat = baseSpan.LastIndexOf('?') < 0 ? '?' : '&';
            var nameSpan = Uri.EscapeDataString(name);
            var valueSpan = string.IsNullOrEmpty(value)
                ? ReadOnlySpan<char>.Empty
                : Uri.EscapeDataString(value).AsSpan();

            var builder = new ValueStringBuilder(stackalloc char[256]);
            builder.Append(baseSpan);
            if (IsEmptyPath(baseSpan) == true)
            {
                builder.Append('/');
            }
            builder.Append(concat);
            builder.Append(nameSpan);
            builder.Append('=');
            builder.Append(valueSpan);
            builder.Append(fragmentSpan);

            return new UriValue(builder.ToString());
        }

        /// <summary>
        /// 获取Fragment
        /// </summary>
        /// <param name="uriSpan">Uri</param>
        /// <returns></returns>
        private static ReadOnlySpan<char> GetFragment(ReadOnlySpan<char> uriSpan)
        {
            var index = uriSpan.LastIndexOf('#');
            return index < 0 ? ReadOnlySpan<char>.Empty : uriSpan[index..];
        }

        /// <summary>
        /// 返回uriString是否不带path
        /// </summary>
        /// <param name="uriSpan"></param>
        /// <returns></returns>
        private static bool IsEmptyPath(ReadOnlySpan<char> uriSpan)
        {
            var index = uriSpan.IndexOf("://");
            if (index < 0)
            {
                return false;
            }
            return uriSpan[(index + 3)..].IndexOf('/') < 0;
        }

        /// <summary>
        /// 转换为Uri
        /// </summary>
        /// <returns></returns>
        public Uri ToUri()
        {
            return this.uri ?? new Uri(this.uriString, UriKind.RelativeOrAbsolute);
        }

        /// <summary>
        /// 转换为文本
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.uriString;
        }
    }
}
