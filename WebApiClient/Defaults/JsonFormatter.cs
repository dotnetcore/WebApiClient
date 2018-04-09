using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Reflection;
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

            var setting = this.CreateSerializerSettings();
            setting.DateFormatString = options.DateTimeFormat;
            setting.ContractResolver = new PropertyContractResolver(options.UseCamelCase);

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

            var setting = this.CreateSerializerSettings();
            setting.ContractResolver = new PropertyContractResolver(false);

            return JsonConvert.DeserializeObject(json, objType, setting);
        }

        /// <summary>
        /// 创建JsonSerializerSettings新实例
        /// 用于序列化或反序列化
        /// </summary>
        /// <returns></returns>
        protected virtual JsonSerializerSettings CreateSerializerSettings()
        {
            return new JsonSerializerSettings();
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
            /// 创建属性
            /// </summary>
            /// <param name="member"></param>
            /// <param name="memberSerialization"></param>
            /// <returns></returns>
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);
                var descriptor = PropertyDescriptor.GetDescriptor(member);

                property.PropertyName = descriptor.Name;
                property.Ignored = descriptor.IgnoreSerialized;

                if (this.useCamelCase == true)
                {
                    property.PropertyName = FormatOptions.CamelCase(property.PropertyName);
                }

                if (property.Converter == null)
                {
                    property.Converter = descriptor.DateTimeConverter;
                }

                if (descriptor.IgnoreWhenNull == true)
                {
                    property.NullValueHandling = NullValueHandling.Ignore;
                }
                return property;
            }

            /// <summary>
            /// 表示属性的描述
            /// </summary>
            private class PropertyDescriptor
            {
                /// <summary>
                /// 序列化范围
                /// </summary>
                private const FormatScope jsonFormatScope = FormatScope.JsonFormat;

                /// <summary>
                /// 属性的描述缓存
                /// </summary>
                private static readonly ConcurrentDictionary<MemberInfo, PropertyDescriptor> descriptorCache;

                /// <summary>
                /// 获取属性别名或名称
                /// </summary>
                public string Name { get; private set; }

                /// <summary>
                /// 获取时间转换器
                /// </summary>
                public JsonConverter DateTimeConverter { get; private set; }

                /// <summary>
                /// 获取是否忽略序列化
                /// </summary>      
                public bool IgnoreSerialized { get; private set; }

                /// <summary>
                /// 获取当值为null时是否忽略序列化
                /// </summary>
                public bool IgnoreWhenNull { get; private set; }

                /// <summary>
                /// 属性的描述
                /// </summary>
                /// <param name="member"></param>
                private PropertyDescriptor(MemberInfo member)
                {
                    var aliasAsAttribute = member.GetAttribute<AliasAsAttribute>(true);
                    if (aliasAsAttribute != null && aliasAsAttribute.IsDefinedScope(jsonFormatScope))
                    {
                        this.Name = aliasAsAttribute.Name;
                    }
                    else
                    {
                        this.Name = member.Name;
                    }

                    var datetimeFormatAttribute = member.GetAttribute<DateTimeFormatAttribute>(true);
                    if (datetimeFormatAttribute != null && datetimeFormatAttribute.IsDefinedScope(jsonFormatScope))
                    {
                        this.DateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeFormatAttribute.Format };
                    }

                    this.IgnoreSerialized = member.IsDefinedFormatScope<IgnoreSerializedAttribute>(jsonFormatScope);
                    this.IgnoreWhenNull = member.IsDefinedFormatScope<IgnoreWhenNullAttribute>(jsonFormatScope);
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

