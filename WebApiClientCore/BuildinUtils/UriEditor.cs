using System;
using System.Text;
using System.Web;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Uri编辑器
    /// </summary>
    class UriEditor
    {
        /// <summary>
        /// uri的fragment
        /// </summary>
        private readonly string fragment;

        /// <summary>
        /// Path的索引
        /// </summary>
        private readonly int pathIndex;

        /// <summary>
        /// fragment长度
        /// </summary>
        private readonly int fragmentLength;



        /// <summary>
        /// 获取当前的Uri
        /// </summary>
        public Uri Uri { get; private set; }


        /// <summary>
        /// 获取Uri参数的编码
        /// </summary>
        public Encoding Encoding { get; }


        /// <summary>
        /// Uri编辑器
        /// </summary>
        /// <param name="uri">绝对路径的uri</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public UriEditor(Uri uri)
            : this(uri, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Url创建者
        /// </summary>
        /// <param name="uri">绝对路径的uri</param>
        /// <param name="encoding">参数的编码</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public UriEditor(Uri uri, Encoding encoding)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            if (uri.IsAbsoluteUri == false)
            {
                throw new UriFormatException(Resx.required_AbsoluteUri.Format(nameof(uri)));
            }

            this.Uri = uri;
            this.Encoding = encoding;

            const int delimiterLength = 3;
            this.fragment = uri.Fragment;
            this.pathIndex = uri.AbsoluteUri.IndexOf('/', uri.Scheme.Length + delimiterLength);
            this.fragmentLength = string.IsNullOrEmpty(uri.Fragment) ? 0 : uri.Fragment.Length;
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
            var uriSpan = this.Uri.OriginalString.AsSpan();
            if (uriSpan.Contains('{') == false)
            {
                return false;
            }

            // 申请可回收的缓冲区，用于转换uri为小写
            using var owner = ArrayPool.Rent<char>(uriSpan.Length);
            var uriLowerSpan = owner.Array.AsSpan();
            var length = uriSpan.ToLowerInvariant(uriLowerSpan);
            uriLowerSpan.Slice(0, length);

            var nameLowerSpan = $"{{{name}}}".ToLowerInvariant().AsSpan();
            var valueSpan = value == null
                ? Span<char>.Empty
                : HttpUtility.UrlEncode(value, this.Encoding).AsSpan();

            var replaced = false;
            using var writer = new BufferWriter<char>(256);

            while (uriLowerSpan.Length > 0)
            {
                var index = uriLowerSpan.IndexOf(nameLowerSpan);
                if (index > -1)
                {
                    // 左边未替换的
                    var left = uriSpan.Slice(0, index);
                    left.CopyTo(writer.GetSpan(left.Length));
                    writer.Advance(left.Length);

                    // 替换的值
                    valueSpan.CopyTo(writer.GetSpan(valueSpan.Length));
                    writer.Advance(valueSpan.Length);

                    // 切割长度
                    var sliceLength = index + nameLowerSpan.Length;

                    // 原始值与小写值同步切割
                    uriSpan = uriSpan.Slice(sliceLength);
                    uriLowerSpan = uriLowerSpan.Slice(sliceLength);

                    replaced = true;
                }
                else
                {
                    if (replaced == true)
                    {
                        // 替换过剩下的原始值
                        uriSpan.CopyTo(writer.GetSpan(uriSpan.Length));
                        writer.Advance(uriSpan.Length);
                    }

                    // 再也无匹配替换值，退出
                    break;
                }
            }

            // 成功替换过，writer的值才是有效值
            if (replaced == true)
            {
                var uri = writer.GetWrittenSpan().ToString();
                this.Uri = new Uri(uri);
            }

            return replaced;
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
            value = HttpUtility.UrlEncode(value, this.Encoding);

            var pathQuery = this.GetPathAndQuery();
            var concat = pathQuery.Contains('?') ? "&" : "?";
            var relativeUri = $"{pathQuery.ToString()}{concat}{name}={value}{this.fragment}";

            this.Uri = new Uri(this.Uri, relativeUri);
        }

        /// <summary>
        /// 获取原始的PathAndQuery
        /// </summary>
        /// <returns></returns>
        private ReadOnlySpan<char> GetPathAndQuery()
        {
            var originalUri = this.Uri.OriginalString.AsSpan();
            var length = originalUri.Length - this.pathIndex - this.fragmentLength;

            return length == 0 ?
                Span<char>.Empty :
                originalUri.Slice(this.pathIndex, length).TrimEnd('&').TrimEnd('?');
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
