using System;
using System.Text;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Uri编辑器
    /// </summary>
    class UriEditor
    {
        /// <summary>
        /// 获取当前的Uri
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// 获取Uri参数的编码
        /// </summary>
        public Encoding Encoding { get; }

        /// <summary>
        /// Url创建者
        /// </summary>
        /// <param name="absoluteUri">绝对路径的uri</param>
        /// <param name="encoding">参数的编码</param>
        /// <exception cref="UriFormatException"></exception>
        public UriEditor(Uri absoluteUri, Encoding encoding)
        {
            if (absoluteUri.IsAbsoluteUri == false)
            {
                throw new UriFormatException(Resx.required_AbsoluteUri.Format(nameof(absoluteUri)));
            }

            this.Uri = absoluteUri;
            this.Encoding = encoding;
        }


        /// <summary>
        /// 替换带有花括号的参数的值
        /// 这个方法是经过严格的benchmark测试的
        /// 时间占用为Regex.Replace的1/2
        /// 时间占用为New Regex的1/12
        /// </summary>
        /// <param name="name">参数名称，不带花括号</param>
        /// <param name="value">参数的值</param>
        /// <returns>替换成功则返回true</returns>
        public bool Replace(string name, string? value)
        {
            var uri = this.Uri.OriginalString;
            if (uri.Contains('{') == false)
            {
                return false;
            }

            name = $"{{{name}}}";
            value = value == null ? null : HttpUtility.UrlEncode(value, this.Encoding);
            if (uri.RepaceIgnoreCase(name, value, out var newUri) == true)
            {
                this.Uri = new Uri(newUri);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddQuery(string name, string? value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            name = HttpUtility.UrlEncode(name, this.Encoding);
            value = value == null ? null : HttpUtility.UrlEncode(value, this.Encoding);

            var uriSpan = this.Uri.OriginalString.AsSpan();
            var fragmentSpan = this.Uri.Fragment.AsSpan();
            uriSpan = uriSpan.Slice(0, uriSpan.Length - fragmentSpan.Length).TrimEnd('?').TrimEnd('&');
            var concat = uriSpan.Contains('?') ? '&' : '?';

            using var writer = new BufferWriter<char>(256);
            writer.Write(uriSpan);
            writer.Write(concat);
            writer.Write(name);
            writer.Write('=');
            writer.Write(value);
            writer.Write(fragmentSpan);

            var uri = writer.GetWrittenSpan().ToString();
            this.Uri = new Uri(uri);
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Uri.ToString();
        }
    }
}
