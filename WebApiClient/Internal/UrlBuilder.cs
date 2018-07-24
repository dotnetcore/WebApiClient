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
        /// <param name="uri">uri</param>
        /// <param name="encoding">参数的编码</param>
        public UrlBuilder(Uri uri, Encoding encoding)
        {
            this.Uri = uri;
            this.Encoding = encoding;
        }

        /// <summary>
        /// 替换带有花括号的参数的值
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数的值</param>
        /// <returns>替换成功则返回true</returns>
        public bool Replace(string name, string value)
        {
            value = HttpUtility.UrlEncode(value, this.Encoding);
            var regex = new Regex($"{{{name}}}", RegexOptions.IgnoreCase);

            var replaced = false;
            var url = regex.Replace(this.Uri.OriginalString, m =>
            {
                replaced = true;
                return value;
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
        public void AddQuery(string name, string value)
        {
            value = HttpUtility.UrlEncode(value, this.Encoding);
            var append = $"{name}={value}";
            var query = this.Uri.Query.TrimStart('?').TrimEnd('&');

            if (query.Length > 0)
            {
                query = string.Concat(query, "&", append);
            }
            else
            {
                query = append;
            }

            var builder = new UriBuilder(this.Uri);
            builder.Query = query;
            this.Uri = builder.Uri;
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
