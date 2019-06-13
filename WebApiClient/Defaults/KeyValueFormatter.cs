using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WebApiClient.Contexts;
using WebApiClient.DataAnnotations;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示默认键值对列化工具
    /// </summary>
    public class KeyValueFormatter : IKeyValueFormatter
    {
        /// <summary>
        /// 默认实例
        /// </summary>
        public static readonly KeyValueFormatter Instance = new KeyValueFormatter();

        /// <summary>
        /// 获取或设置配置
        /// </summary>
        public Action<JsonSerializerSettings> Settings { get; set; }

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
            if (string.IsNullOrEmpty(name) == true)
            {
                throw new ArgumentNullException(nameof(name));
            }

            var setting = this.CreateSerializerSettings(options);
            this.Settings?.Invoke(setting);
            var serializer = JsonSerializer.Create(setting);
            var keyValuesWriter = new KeyValuesWriter(name);

            serializer.Serialize(keyValuesWriter, obj);
            return keyValuesWriter;
        }

        /// <summary>
        /// 序列化参数为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="options">选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public IEnumerable<KeyValuePair<string, string>> Serialize(ApiParameterDescriptor parameter, FormatOptions options)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }
            return this.Serialize(parameter.Name, parameter.Value, options);
        }

        /// <summary>
        /// 创建序列化配置     
        /// </summary>
        /// <param name="options">格式化选项</param>
        /// <returns></returns>
        protected virtual JsonSerializerSettings CreateSerializerSettings(FormatOptions options)
        {
            var useCamelCase = options?.UseCamelCase == true;
            var setting = options.ToSerializerSettings(FormatScope.KeyValueFormat);

            setting.Converters.Add(JsonStringConverter.Instance);
            setting.Converters.Add(new KeyValuePairConverter(useCamelCase));
            return setting;
        }
    }
}
