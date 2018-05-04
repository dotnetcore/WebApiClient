using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        /// 创建属性
        /// </summary>
        /// <param name="member"></param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var descriptor = PropertyDescriptor.GetDescriptor(this.formatScope, member);

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
            /// <param name="scopeMember"></param>
            private PropertyDescriptor(ScopeMember scopeMember)
            {
                var member = scopeMember.MemberInfo;
                var formatScope = scopeMember.FormatScope;

                var aliasAsAttribute = member.GetAttribute<AliasAsAttribute>(true);
                if (aliasAsAttribute != null && aliasAsAttribute.IsDefinedScope(formatScope))
                {
                    this.Name = aliasAsAttribute.Name;
                }
                else
                {
                    this.Name = member.Name;
                }

                var datetimeFormatAttribute = member.GetAttribute<DateTimeFormatAttribute>(true);
                if (datetimeFormatAttribute != null && datetimeFormatAttribute.IsDefinedScope(formatScope))
                {
                    this.DateTimeConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeFormatAttribute.Format };
                }

                this.IgnoreSerialized = member.IsDefinedFormatScope<IgnoreSerializedAttribute>(formatScope);
                this.IgnoreWhenNull = member.IsDefinedFormatScope<IgnoreWhenNullAttribute>(formatScope);
            }


            /// <summary>
            /// 属性的描述缓存
            /// </summary>
            private static readonly ConcurrentDictionary<ScopeMember, PropertyDescriptor> descriptorCache;

            /// <summary>
            /// 静态构造器
            /// </summary>
            static PropertyDescriptor()
            {
                descriptorCache = new ConcurrentDictionary<ScopeMember, PropertyDescriptor>(new ScopeMemberComparer());
            }

            /// <summary>
            /// 获取成员的描述
            /// </summary>
            /// <param name="scope">范围</param>
            /// <param name="member">成员</param>
            /// <returns></returns>
            public static PropertyDescriptor GetDescriptor(FormatScope scope, MemberInfo member)
            {
                var scopeProperty = new ScopeMember(scope, member);
                return descriptorCache.GetOrAdd(scopeProperty, (s) => new PropertyDescriptor(s));
            }

            /// <summary>
            /// 表示序列化范围属性
            /// </summary>
            private class ScopeMember
            {
                /// <summary>
                /// 序列化范围
                /// </summary>
                public FormatScope FormatScope { get; private set; }

                /// <summary>
                /// 属性
                /// </summary>
                public MemberInfo MemberInfo { get; private set; }

                /// <summary>
                /// 序列化范围属性
                /// </summary>
                /// <param name="scope"></param>
                /// <param name="member"></param>
                public ScopeMember(FormatScope scope, MemberInfo member)
                {
                    this.FormatScope = scope;
                    this.MemberInfo = member;
                }
            }

            /// <summary>
            /// 表示序列化范围属性比较器
            /// </summary>
            private class ScopeMemberComparer : IEqualityComparer<ScopeMember>
            {
                /// <summary>
                /// 是否相等
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <returns></returns>
                public bool Equals(ScopeMember x, ScopeMember y)
                {
                    return true;
                }

                /// <summary>
                /// 获取哈希值
                /// </summary>
                /// <param name="obj"></param>
                /// <returns></returns>
                public int GetHashCode(ScopeMember obj)
                {
                    return obj.FormatScope.GetHashCode() ^ obj.MemberInfo.GetHashCode();
                }
            }
        }
    }
}