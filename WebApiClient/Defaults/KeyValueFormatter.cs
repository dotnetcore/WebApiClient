using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using WebApiClient.Contexts;
using WebApiClient.DataAnnotations;
using WebApiClient.Defaults.KeyValueFormats;
using WebApiClient.Defaults.KeyValueFormats.Converters;
using WebApiClient.Interfaces;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示默认键值对列化工具
    /// </summary>
    public class KeyValueFormatter : IKeyValueFormatter
    {
        /// <summary>
        /// 默认的转换器组合
        /// </summary>
        private static readonly ConverterBase[] defaultConverters = new ConverterBase[]
        {
            new NullValueConverter(),
            new SimpleTypeConverter(),
            new KeyValuePairConverter(),
            new EnumerableConverter(),
            new PropertiesConverter()
        };

        /// <summary>
        /// 第一个转换器
        /// </summary>
        private readonly ConverterBase firstConverter;

        /// <summary>
        /// 默认键值对列化工具
        /// </summary>
        public KeyValueFormatter()
        {
            var notSupported = new NotSupportedConverter();
            var converters = this.GetConverters().Concat(new[] { notSupported });
            this.firstConverter = converters.First();

            converters.Aggregate((cur, next) =>
            {
                cur.Next = next;
                cur.Formatter = this;
                return next;
            }).Formatter = this;
        }

        /// <summary>
        /// 序列化参数为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> Serialize(ApiParameterDescriptor parameter, FormatOptions options)
        {
            return this.Serialize(parameter.Name, parameter.Value, options);
        }

        /// <summary>
        /// 序列化对象为键值对
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="obj">对象实例</param>
        /// <param name="options">选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> Serialize(string name, object obj, FormatOptions options)
        {
            var context = new ConvertContext
            {
                Name = name,
                Value = obj,
                Type = obj == null ? null : obj.GetType(),
                Options = options == null ? new FormatOptions() : options
            };
            return this.firstConverter.Invoke(context);
        }

        /// <summary>
        /// 返回一组按顺序的转换器
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<ConverterBase> GetConverters()
        {
            return KeyValueFormatter.defaultConverters;
        }

        /// <summary>
        /// 默认的转换器
        /// </summary>
        private class NotSupportedConverter : ConverterBase
        {
            public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
            {
                throw new NotSupportedException("不支持的类型转换：" + context.Type);
            }
        }
    }
}
