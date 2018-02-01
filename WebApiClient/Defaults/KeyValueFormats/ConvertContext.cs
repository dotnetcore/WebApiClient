using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WebApiClient.Defaults.KeyValueFormats
{
    /// <summary>
    /// 表示要转换的上下文
    /// </summary>
    [DebuggerDisplay("{Name} = {Data}")]
    public class ConvertContext
    {
        /// <summary>
        /// 获取名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取要转换的数据
        /// </summary>
        public object Data { get; private set; }

        /// <summary>
        /// 获取已递归层数
        /// </summary>
        public int Depths { get; internal set; }

        /// <summary>
        /// 获取格式化选项
        /// </summary>
        public FormatOptions Options { get; private set; }

        /// <summary>
        /// 获取数据类型
        /// </summary>
        public Type DataType { get; private set; }

        /// <summary>
        /// 要转换的上下文
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="data">要转换的数据</param>
        /// <param name="depths">已递归层数</param>
        /// <param name="options">格式化选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ConvertContext(string name, object data, int depths, FormatOptions options)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Name = name;
            this.Data = data;
            this.Depths = depths;
            this.DataType = data?.GetType();
            this.Options = options ?? new FormatOptions();
        }

        /// <summary>
        /// 转换为键值对
        /// 并使用IEnumerable封装
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs()
        {
            yield return this.ToKeyValuePair();
        }

        /// <summary>
        /// 转换为键值对
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<string, string> ToKeyValuePair()
        {
            var key = this.Name;
            if (this.Options.UseCamelCase == true)
            {
                key = FormatOptions.CamelCase(key);
            }

            if (this.Data == null)
            {
                return new KeyValuePair<string, string>(key, null);
            }

            var dataType = Nullable.GetUnderlyingType(this.DataType);
            if (dataType == null)
            {
                dataType = this.DataType;
            }

            if (dataType == typeof(DateTime))
            {
                var dateTimeString = this.Options.FormatDateTime((DateTime)this.Data);
                return new KeyValuePair<string, string>(key, dateTimeString);
            }

            if (dataType.IsEnum == true)
            {
                var enumValueString = ((int)this.Data).ToString();
                return new KeyValuePair<string, string>(key, enumValueString);
            }

            return new KeyValuePair<string, string>(key, this.Data.ToString());
        }
    }
}
