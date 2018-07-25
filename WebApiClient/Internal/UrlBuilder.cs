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
        public void AddQuery(string name, string value)
        {
            var encode = HttpUtility.UrlEncode(value, this.Encoding);
            var append = $"{name}={encode}";
            var pathQuery = this.Uri.PathAndQuery.TrimEnd('&', '?');
            var concat = pathQuery.IndexOf('?') > -1 ? "&" : "?";
            var pathAndQuery = $"{pathQuery}{concat}{append}";

            var delimiter = this.Uri.UserInfo.Length > 0 ? "@" : null;
            var uri = $"{this.Uri.Scheme}://{this.Uri.UserInfo}{delimiter}{this.Uri.Authority}{pathAndQuery}{this.Uri.Fragment}";
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
