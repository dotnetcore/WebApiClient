using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 使用KeyValueFormatter序列化参数值得到的键值对作为url路径参数或query参数的特性
    /// 没有任何特性修饰的参数，将默认被PathQueryAttribute修饰
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class PathQueryAttribute : Attribute, IApiParameterAttribute, IIgnoreWhenNullable, IDateTimeFormatable, IEncodingable
    {
        /// <summary>
        /// 编码
        /// </summary>
        private Encoding encoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// 获取或设置参数的编码名称
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public string Encoding
        {
            get
            {
                return this.encoding.WebName;
            }
            set
            {
                this.encoding = System.Text.Encoding.GetEncoding(value);
            }
        }

        /// <summary>
        /// 获取或设置当值为null是否忽略提交
        /// 默认为false
        /// </summary>
        public bool IgnoreWhenNull { get; set; }

        /// <summary>
        /// 获取或设置时期时间格式
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 获取或设置集合格式化方式
        /// </summary>
        public CollectionFormat CollectionFormat = CollectionFormat.Multi;

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
            this.DateTimeFormat = datetimeFormat;
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <exception cref="HttpApiConfigException"></exception>
        /// <returns></returns>
        public async Task BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            var uri = context.RequestMessage.RequestUri;
            if (uri == null)
            {
                throw new HttpApiConfigException($"未配置HttpHost，无法使用参数{parameter.Name}");
            }

            if (this.IsIgnoreWith(parameter) == true)
            {
                return;
            }

            var options = context.HttpApiConfig.FormatOptions.CloneChange(this.DateTimeFormat);
            var keyValues = context.HttpApiConfig.KeyValueFormatter.Serialize(parameter, options);
            var query = this.FormateCollection(keyValues);

            context.RequestMessage.RequestUri = this.UsePathQuery(uri, query);
            await ApiTask.CompletedTask;
        }

        /// <summary>
        /// 格式化集合
        /// </summary>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<string, string>> FormateCollection(IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            IEnumerable<KeyValuePair<string, string>> JoinValue(IEnumerable<IGrouping<string, KeyValuePair<string, string>>> grouping, string separator)
            {
                return grouping.Select(item =>
                {
                    var value = string.Join(separator, item.Select(i => i.Value));
                    return new KeyValuePair<string, string>(item.Key, value);
                });
            }

            if (this.CollectionFormat == CollectionFormat.Multi)
            {
                return keyValues;
            }

            var groups = keyValues.GroupBy(item => item.Key);
            switch (this.CollectionFormat)
            {
                case CollectionFormat.Csv:
                    return JoinValue(groups, @",");

                case CollectionFormat.Ssv:
                    return JoinValue(groups, @" ");

                case CollectionFormat.Tsv:
                    return JoinValue(groups, @"\");

                case CollectionFormat.Pipes:
                    return JoinValue(groups, @"|");

                default:
                    throw new NotImplementedException();
            }
        }


        /// <summary>
        /// url添加query或替换segment
        /// </summary>
        /// <param name="uri">url</param>
        /// <param name="query">键值对</param>
        /// <returns></returns>
        protected Uri UsePathQuery(Uri uri, IEnumerable<KeyValuePair<string, string>> query)
        {
            var editor = new UriEditor(uri, this.encoding);
            foreach (var keyValue in query)
            {
                if (editor.Replace(keyValue.Key, keyValue.Value) == false)
                {
                    editor.AddQuery(keyValue.Key, keyValue.Value);
                }
            }
            return editor.Uri;
        }
    }
}
