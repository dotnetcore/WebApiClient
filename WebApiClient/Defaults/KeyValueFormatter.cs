using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using WebApiClient.Contexts;
using WebApiClient.DataAnnotations;
using WebApiClient.Defaults.KeyValueFormates;
using WebApiClient.Defaults.KeyValueFormates.Converters;
using WebApiClient.Interfaces;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示默认键值对列化工具
    /// </summary>
    public class KeyValueFormatter : IKeyValueFormatter
    {
        /// <summary>
        /// 转换器链表
        /// </summary>
        private readonly LinkedList<ConverterBase> linkedList = new LinkedList<ConverterBase>();

        /// <summary>
        /// 默认的转换器组合
        /// </summary>
        private static readonly ConverterBase[] defaultConverters = new ConverterBase[]
        {
            new NullValueConverter(),
            new SimpleValueConverter(),
            new KeyValuePairConverter(),
            new DictionaryConverter(),
            new EnumerableConverter(),
            new PropertiesConverter()
        };

        /// <summary>
        /// 默认键值对列化工具
        /// </summary>
        public KeyValueFormatter()
            : this(defaultConverters)
        {
        }

        /// <summary>
        /// 默认键值对列化工具
        /// </summary>
        /// <param name="converters"></param>
        public KeyValueFormatter(IEnumerable<ConverterBase> converters)
        {
            if (converters == null)
            {
                throw new ArgumentNullException(nameof(converters));
            }

            foreach (var item in converters)
            {
                this.linkedList.AddLast(item);
            }
            this.linkedList.AddLast(new NotSupportedConverter());

            var node = this.linkedList.First;
            while (node.Next != null)
            {
                node.Value.Next = node.Next.Value;
                node.Value.Formatter = this;
                node = node.Next;
            }
        }

        /// <summary>
        /// 序列化参数为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> IKeyValueFormatter.Serialize(
            ApiParameterDescriptor parameter,
            FormatOptions options)
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
        IEnumerable<KeyValuePair<string, string>> IKeyValueFormatter.Serialize(
            string name,
            object obj,
            FormatOptions options)
        {
            return this.Serialize(name, obj, options);
        }

        /// <summary>
        /// 序列化对象为键值对
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="obj">对象实例</param>
        /// <param name="options">选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        protected virtual IEnumerable<KeyValuePair<string, string>> Serialize(
            string name,
            object obj,
            FormatOptions options)
        {
            if (options == null)
            {
                options = new FormatOptions();
            }

            var type = obj == null ? null : obj.GetType();
            var context = new ConvertContext
            {
                Name = name,
                Value = obj,
                Options = options,
                Descriptor = TypeDescriptor.GetDescriptor(type)
            };

            return this.linkedList.First.Value.Invoke(context);
        }

        /// <summary>
        /// 默认的转换器
        /// </summary>
        private class NotSupportedConverter : ConverterBase
        {
            public override IEnumerable<KeyValuePair<string, string>> Invoke(ConvertContext context)
            {
                throw new NotSupportedException("不支持的类型转换");
            }
        }
    }
}
