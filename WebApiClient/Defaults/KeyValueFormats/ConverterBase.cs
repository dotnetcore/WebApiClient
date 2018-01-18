using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Interfaces;

namespace WebApiClient.Defaults.KeyValueFormats
{
    /// <summary>
    /// 表示转换器的抽象类
    /// </summary>
    public abstract class ConverterBase
    {
        /// <summary>
        /// 设置过滤器
        /// </summary>
        public IKeyValueFormatter Formatter { private get; set; }

        /// <summary>
        /// 获取下一个转换器
        /// </summary>
        public ConverterBase Next { get; set; }

        /// <summary>
        /// 执行转换
        /// </summary>
        /// <param name="context">转换上下文</param>
        /// <returns></returns>
        public abstract IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context);

        /// <summary>
        /// 使用与Converter关联的IJsonFormatter进行序列化
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        protected IEnumerable<KeyValuePair<string, string>> SerializeByFormatter(ConvertContext context)
        {
            return this.SerializeByFormatter(context.Name, context.Value, context.Options);
        }

        /// <summary>
        /// 使用与Converter关联的IJsonFormatter进行序列化
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        protected IEnumerable<KeyValuePair<string, string>> SerializeByFormatter(string name, object value, FormatOptions options)
        {
            return this.Formatter.Serialize(name, value, options);
        }

        /// <summary>
        /// 将上下文简单转换为键值对
        /// </summary>
        /// <param name="context">上下文</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        protected KeyValuePair<string, string> ToKeyValuePair(ConvertContext context)
        {
            return this.ToKeyValuePair(context.Name, context.Value, context.Options);
        }

        /// <summary>
        /// 将值简单转换为键值对
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="options">选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        protected KeyValuePair<string, string> ToKeyValuePair(string name, object value, FormatOptions options)
        {
            if (string.IsNullOrEmpty(name) == true)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (options == null)
            {
                options = new FormatOptions();
            }

            if (options.UseCamelCase == true)
            {
                name = FormatOptions.CamelCase(name);
            }

            if (value == null)
            {
                return new KeyValuePair<string, string>(name, null);
            }

            var isDateTime = value is DateTime;
            if (isDateTime == false)
            {
                return new KeyValuePair<string, string>(name, value.ToString());
            }

            // 时间格式转换         
            var dateTime = ((DateTime)value).ToString(options.DateTimeFormat, DateTimeFormatInfo.InvariantInfo);
            return new KeyValuePair<string, string>(name, dateTime);
        }
    }
}
