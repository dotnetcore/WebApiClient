using Newtonsoft.Json;
using System;
using WebApiClient.DataAnnotations;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 默认的json解析工具
    /// </summary>
    public class JsonFormatter : IJsonFormatter
    {
        /// <summary>
        /// 将对象列化为json文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public string Serialize(object obj, FormatOptions options)
        {
            if (obj == null)
            {
                return null;
            }

            if (options == null)
            {
                options = new FormatOptions();
            }

            var setting = this.CreateSerializerSettings(options);
            return JsonConvert.SerializeObject(obj, setting);
        }

        /// <summary>
        /// 反序列化json为对象
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        public object Deserialize(string json, Type objType)
        {
            if (string.IsNullOrEmpty(json))
            {
                return objType.DefaultValue();
            }

            var setting = this.CreateSerializerSettings(null);
            return JsonConvert.DeserializeObject(json, objType, setting);
        }

        /// <summary>
        /// 创建序列化或反序列化配置       
        /// </summary>
        /// <param name="options">格式化选项</param>
        /// <returns></returns>
        protected virtual JsonSerializerSettings CreateSerializerSettings(FormatOptions options)
        {
            var useCamelCase = options?.UseCamelCase == true;
            return new JsonSerializerSettings
            {
                DateFormatString = options?.DateTimeFormat,
                ContractResolver = AnnotationsContractResolver.GetResolver(FormatScope.JsonFormat, useCamelCase)
            };
        }
    }
}