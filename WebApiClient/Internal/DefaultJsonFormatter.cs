using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using WebApiClient.DataAnnotations;
using WebApiClient.Interfaces;

namespace WebApiClient
{
    /// <summary>
    /// 默认的json解析工具
    /// </summary>
    class DefaultJsonFormatter : IJsonFormatter
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

            var setting = new JsonSerializerSettings
            {
                DateFormatString = options.DateTimeFormat,
                ContractResolver = new PropertyContractResolver(options.UseCamelCase)
            };
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
                return null;
            }
            var setting = new JsonSerializerSettings
            {
                ContractResolver = new PropertyContractResolver(false)
            };
            return JsonConvert.DeserializeObject(json, objType, setting);
        }

        /// <summary>
        /// 属性解析器
        /// </summary>
        private class PropertyContractResolver : DefaultContractResolver
        {
            /// <summary>
            /// 是否camel命名
            /// </summary>
            private readonly bool useCamelCase;

            /// <summary>
            /// 属性解析器
            /// </summary>
            /// <param name="camelCase">是否camel命名</param>
            public PropertyContractResolver(bool camelCase)
            {
                this.useCamelCase = camelCase;
            }

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
                var descriptor = PropertyDescriptor.GetDescriptor(member);

                property.PropertyName = descriptor.AliasName;
                if (this.useCamelCase == true)
                {
                    property.PropertyName = FormatOptions.CamelCase(property.PropertyName);
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
                /// 属性的描述缓存
                /// </summary>
                private static readonly ConcurrentDictionary<MemberInfo, PropertyDescriptor> descriptorCache;

                /// <summary>
                /// 获取属性别名或名称
                /// </summary>
                public string AliasName { get; private set; }

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
                private PropertyDescriptor(MemberInfo member)
                {
                    var aliasAs = member.GetAttribute<AliasAsAttribute>(true);
                    this.AliasName = aliasAs == null ? member.Name : aliasAs.Name;

                    var datatimeFormat = member.GetAttribute<DateTimeFormatAttribute>(true);
                    if (datatimeFormat != null)
                    {
                        this.DateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = datatimeFormat.Format };
                    }

                    this.IsIgnoreSerialized = member.IsDefined(typeof(IgnoreSerializedAttribute));
                }

                /// <summary>
                /// 静态构造器
                /// </summary>
                static PropertyDescriptor()
                {
                    descriptorCache = new ConcurrentDictionary<MemberInfo, PropertyDescriptor>();
                }

                /// <summary>
                /// 获取成员的描述
                /// </summary>
                /// <param name="member">成员</param>
                /// <returns></returns>
                public static PropertyDescriptor GetDescriptor(MemberInfo member)
                {
                    return descriptorCache.GetOrAdd(member, (m) => new PropertyDescriptor(m));
                }
            }
        }
    }

}

