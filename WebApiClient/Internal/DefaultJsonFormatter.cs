using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using Newtonsoft.Json.Converters;
using System.Collections.Concurrent;

namespace WebApiClient
{
    /// <summary>
    /// 默认的json解析工具
    /// </summary>
    class DefaultJsonFormatter : IJsonFormatter
    {
        /// <summary>
        /// 将参数值序列化为json文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="datetimeFormate">时期格式，null则ISO 8601</param>
        /// <returns></returns>
        public string Serialize(object obj, string datetimeFormate)
        {
            if (obj == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(datetimeFormate))
            {
                datetimeFormate = DateTimeFormats.ISO8601_WithMillisecond;
            }

            var setting = new JsonSerializerSettings
            {
                DateFormatString = datetimeFormate,
                ContractResolver = new PropertyContractResolver()
            };
            return JsonConvert.SerializeObject(obj, setting);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        public object Deserialize(string json, Type objType)
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            var setting = new JsonSerializerSettings
            {
                ContractResolver = new PropertyContractResolver()
            };
            return JsonConvert.DeserializeObject(json, objType, setting);
        }
    }

    /// <summary>
    /// 属性解析器
    /// </summary>
    class PropertyContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 属性的描述缓存
        /// </summary>
        private static readonly ConcurrentDictionary<MemberInfo, PropertyDescriptor> descriptorCache = new ConcurrentDictionary<MemberInfo, PropertyDescriptor>();

        /// <summary> 
        /// <summary>
        /// 创建属性
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var descriptor = descriptorCache.GetOrAdd(member, (m) => new PropertyDescriptor(m));

            if (string.IsNullOrEmpty(descriptor.Alias) == false)
            {
                property.PropertyName = descriptor.Alias;
            }

            if (property.Converter != null)
            {
                property.Converter = descriptor.DateTimeConverter;
            }

            property.Ignored = descriptor.IsIgnoreSerialized;
            return property;
        }

        /// <summary>
        /// 表示属性的描述
        /// </summary>
        private class PropertyDescriptor
        {
            /// <summary>
            /// 获取别名
            /// </summary>
            public string Alias { get; private set; }

            /// <summary>
            /// 获取时间转换器
            /// </summary>
            public JsonConverter DateTimeConverter { get; private set; }

            /// <summary>
            /// 获取是否序列化忽略
            /// </summary>
            public bool IsIgnoreSerialized { get; private set; }

            /// <summary>
            /// 属性的描述
            /// </summary>
            /// <param name="member"></param>
            public PropertyDescriptor(MemberInfo member)
            {
                var datatimeFormat = member.GetAttribute<DateTimeFormatAttribute>(true);
                if (datatimeFormat != null)
                {
                    this.DateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = datatimeFormat.Format };
                }

                var aliasAs = member.GetAttribute<AliasAsAttribute>(true);
                if (aliasAs != null)
                {
                    this.Alias = aliasAs.Name;
                }

                this.IsIgnoreSerialized = member.IsDefined(typeof(IgnoreSerializedAttribute));
            }
        }
    }
}

