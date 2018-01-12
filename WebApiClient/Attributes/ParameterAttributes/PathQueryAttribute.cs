using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示将参数值作为url路径参数或query参数的特性
    /// 支持单一值类型如string、int、guid、枚举等，以及他们的可空类型或集合
    /// 支持POCO类型、IDictionaryOf(string,string)类型、IDictionaryOf(string,object)类型
    /// 没有任何特性修饰的普通参数，将默认为PathQuery修饰
    /// 依赖于HttpApiConfig.KeyValueFormatter
    /// 不可继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public sealed class PathQueryAttribute : Attribute, IApiParameterAttribute
    {
        /// <summary>
        /// 时期时间格式
        /// </summary>
        private readonly string datetimeFormate;

        /// <summary>
        /// 表示Url路径参数或query参数的特性
        /// </summary>
        public PathQueryAttribute()
            : this(null)
        {
        }

        /// <summary>
        /// 表示Url路径参数或query参数的特性
        /// </summary>
        /// <param name="datetimeFormat">时期时间格式</param>
        public PathQueryAttribute(string datetimeFormat)
        {
            this.datetimeFormate = datetimeFormat;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <exception cref="ApiConfigException"></exception>
        /// <returns></returns>
        public async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var uri = context.RequestMessage.RequestUri;
            if (uri == null)
            {
                throw new ApiConfigException("未配置HttpConfig.HttpHost或未使用HttpHostAttribute特性");
            }

            var fixUrl = uri.ToString().TrimEnd('?', '&', '/');
            var timeFormat = context.HttpApiConfig.SelectDateTimeFormat(this.datetimeFormate);
            var keyValues = context.HttpApiConfig.KeyValueFormatter.Serialize(parameter, timeFormat);
            var targetUrl = new Uri(this.UseQuery(fixUrl, keyValues));

            context.RequestMessage.RequestUri = targetUrl;
            await ApiTask.CompletedTask;
        }

        /// <summary>
        /// url添加query
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        private string UseQuery(string url, IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            foreach (var keyValue in keyValues)
            {
                url = this.UseQuery(url, keyValue);
            }
            return url;
        }

        /// <summary>
        /// url添加query
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="keyValue">键值对</param>
        /// <returns></returns>
        private string UseQuery(string url, KeyValuePair<string, string> keyValue)
        {
            var key = keyValue.Key;
            var value = keyValue.Value == null ? string.Empty : keyValue.Value;
            var regex = new Regex("{" + key + "}", RegexOptions.IgnoreCase);

            if (regex.IsMatch(url) == true)
            {
                return regex.Replace(url, value);
            }

            var valueEncoded = HttpUtility.UrlEncode(value, Encoding.UTF8);
            var query = string.Format("{0}={1}", key, valueEncoded);
            var concat = url.Contains('?') ? "&" : "?";
            return url + concat + query;
        }
    }
}
