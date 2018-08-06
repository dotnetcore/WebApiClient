using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Reflection;
using WebApiClient.DataAnnotations;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示属性解析约定
    /// 用于实现DataAnnotations的功能
    /// </summary>
    class PropertyContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 是否camel命名
        /// </summary>
        private readonly bool useCamelCase;

        /// <summary>
        /// 序列化范围
        /// </summary>
        private readonly FormatScope formatScope;

        /// <summary>
        /// 属性解析器
        /// </summary>
        /// <param name="camelCase">是否camel命名</param>
        /// <param name="scope">序列化范围</param>
        public PropertyContractResolver(bool camelCase, FormatScope scope)
        {
            this.useCamelCase = camelCase;
            this.formatScope = scope;
        }

        /// <summary>
        /// 字典Key的CamelCase
        /// </summary>
        /// <param name="dictionaryKey"></param>
        /// <returns></returns>
        protected override string ResolveDictionaryKey(string dictionaryKey)
        {
            var name = base.ResolveDictionaryKey(dictionaryKey);
            return this.useCamelCase ? FormatOptions.CamelCase(name) : name;
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
            var descriptor = new PropertyDescriptor(this.formatScope, member);

            property.PropertyName = this.useCamelCase ? FormatOptions.CamelCase(descriptor.AliasName) : descriptor.AliasName;
            property.Ignored = descriptor.IgnoreSerialized;

            if (property.Converter == null && descriptor.DateTimeFormat != null)
            {
                property.Converter = new IsoDateTimeConverter { DateTimeFormat = descriptor.DateTimeFormat };
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
            /// 获取属性别名或名称
            /// </summary>
            public string AliasName { get; private set; }

            /// <summary>
            /// 获取日期时间格式
            /// </summary>
            public string DateTimeFormat { get; private set; }

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
            /// <param name="scope"></param>
            /// <param name="member"></param>
            public PropertyDescriptor(FormatScope scope, MemberInfo member)
            {
                var aliasAsAttribute = member.GetCustomAttribute<AliasAsAttribute>(true);
                if (aliasAsAttribute != null && aliasAsAttribute.IsDefinedScope(scope))
                {
                    this.AliasName = aliasAsAttribute.Name;
                }
                else
                {
                    this.AliasName = member.Name;
                }

                var datetimeFormatAttribute = member.GetCustomAttribute<DateTimeFormatAttribute>(true);
                if (datetimeFormatAttribute != null && datetimeFormatAttribute.IsDefinedScope(scope))
                {
                    this.DateTimeFormat = datetimeFormatAttribute.Format;
                }

                this.IgnoreSerialized = member.IsDefinedFormatScope<IgnoreSerializedAttribute>(scope);
                this.IgnoreWhenNull = member.IsDefinedFormatScope<IgnoreWhenNullAttribute>(scope);
            }
        }
    }
}