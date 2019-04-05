using System;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApiClient
{
    /// <summary>
    /// 表示Uri编辑器
    /// </summary>
    public class UriEditor
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
        /// 当前的Uri
        /// </summary>
        private Uri uriValue;

        /// <summary>
        /// 当前的Uri是否可替换值
        /// </summary>
        private bool uriCanReplace = false;


        /// <summary>
        /// 获取当前的Uri
        /// </summary>
        public Uri Uri
        {
            get
            {
                return this.uriValue;
            }
            private set
            {
                this.uriValue = value;
                this.uriCanReplace = value.OriginalString.IndexOf('{') > -1;
            }
        }

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
            this.Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            this.Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            if (uri.IsAbsoluteUri == false)
            {
                throw new UriFormatException($"{nameof(uri)}必须为绝对完整URI");
            }

            this.fragment = uri.Fragment;
            this.pathIndex = uri.AbsoluteUri.IndexOf(uri.PathAndQuery);
            this.fragmentLength = string.IsNullOrEmpty(uri.Fragment) ? 0 : uri.Fragment.Length;
        }

        /// <summary>
        /// 替换带有花括号的参数的值
        /// </summary>
        /// <param name="name">参数名称，不带花括号</param>
        /// <param name="value">参数的值</param>
        /// <returns>替换成功则返回true</returns>
        public bool Replace(string name, string value)
        {
            if (this.uriCanReplace == false)
            {
                return false;
            }

            var replaced = false;
            var regex = new Regex($"{{{name}}}", RegexOptions.IgnoreCase);
            var url = regex.Replace(this.Uri.OriginalString, m =>
            {
                replaced = true;
                return HttpUtility.UrlEncode(value, this.Encoding);
            });

            if (replaced == true)
            {
                this.Uri = new Uri(url);
            }
            return replaced;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddQuery(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            name = HttpUtility.UrlEncode(name, this.Encoding);
            value = HttpUtility.UrlEncode(value, this.Encoding);

            var pathQuery = this.GetPathAndQuery();
            var concat = pathQuery.IndexOf('?') > -1 ? "&" : "?";
            var relativeUri = $"{pathQuery}{concat}{name}={value}{this.fragment}";

            this.Uri = new Uri(this.Uri, relativeUri);
        }

        /// <summary>
        /// 获取原始的PathAndQuery
        /// </summary>
        /// <returns></returns>
        private string GetPathAndQuery()
        {
            var originalUri = this.Uri.OriginalString;
            var length = originalUri.Length - this.pathIndex - this.fragmentLength;
            return originalUri.Substring(this.pathIndex, length).TrimEnd('&', '?');
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
