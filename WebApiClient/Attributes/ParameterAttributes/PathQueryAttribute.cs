using System;
using System.Collections.Generic;
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
        [Obsolete]
        private Encoding encoding = System.Text.Encoding.UTF8;

        /// <summary>
        /// 获取或设置参数的编码名称
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        [Obsolete("Encoding将不再生效")]
        public string Encoding
        {
            get => this.encoding.WebName;
            set => this.encoding = System.Text.Encoding.GetEncoding(value);
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
        public CollectionFormat CollectionFormat { get; set; } = CollectionFormat.Multi;

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

            if (this.IgnoreWhenNull(parameter) == true)
            {
                return;
            }

            var options = context
                .HttpApiConfig
                .FormatOptions
                .CloneChange(this.DateTimeFormat);

            var keyValues = context
                .HttpApiConfig
                .KeyValueFormatter
                .Serialize(parameter, options)
                .FormatAs(this.CollectionFormat);

            context.RequestMessage.RequestUri = this.UsePathQuery(uri, keyValues);
            await ApiTask.CompletedTask;
        }

        /// <summary>
        /// url添加query或替换segment
        /// </summary>
        /// <param name="uri">url</param>
        /// <param name="keyValues">键值对</param>
        /// <returns></returns>
        protected virtual Uri UsePathQuery(Uri uri, IEnumerable<KeyValuePair<string, string>> keyValues)
        {
            var editor = new UriEditor(uri);
            foreach (var keyValue in keyValues)
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
