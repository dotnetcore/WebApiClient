using System;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApiClient
{
    /// <summary>
    /// 表示Url创建者
    /// </summary>
    class UrlBuilder
    {
        /// <summary>
        /// 获取当前的Uri
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// 获取Uri参数的编码
        /// </summary>
        public Encoding Encoding { get; private set; }

        /// <summary>
        /// Url创建者
        /// </summary>
        /// <param name="uri">绝对路径的uri</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public UrlBuilder(Uri uri)
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
        public UrlBuilder(Uri uri, Encoding encoding)
        {
            this.Uri = uri ?? throw new ArgumentNullException(nameof(uri));
            this.Encoding = encoding ?? throw new ArgumentNullException(nameof(encoding));
            if (uri.IsAbsoluteUri == false)
            {
                throw new UriFormatException($"{nameof(uri)}必须为绝对完整URI");
            }
        }

        /// <summary>
        /// 替换带有花括号的参数的值
        /// </summary>
        /// <param name="name">参数名称，不带花括号</param>
        /// <param name="value">参数的值</param>
        /// <returns>替换成功则返回true</returns>
        public bool Replace(string name, string value)
        {
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

            var pathQuery = this.Uri.PathAndQuery.TrimEnd('&', '?');
            var concat = pathQuery.IndexOf('?') > -1 ? "&" : "?";
            var relativeUri = $"{pathQuery}{concat}{name}={value}{this.Uri.Fragment}";

            this.Uri = new Uri(this.Uri, relativeUri);
        }

        /// <summary>
        /// 设置Path
        /// </summary>
        /// <param name="path">新路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void SetPath(string path)
        {
            if (string.IsNullOrEmpty(path) == true)
            {
                throw new ArgumentNullException(nameof(path));
            }
            this.SetPath(p => path);
        }

        /// <summary>
        /// 设置Path
        /// </summary>
        /// <param name="path">新路径</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void SetPath(Func<string, string> path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var newPath = path.Invoke(this.Uri.AbsolutePath);
            if (string.IsNullOrEmpty(newPath) == true)
            {
                throw new ArgumentException("要求path值不能为空", nameof(path));
            }

            var relativeUri = $"{newPath}{this.Uri.Query}{this.Uri.Fragment}";
            this.Uri = new Uri(this.Uri, relativeUri);
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
