using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using WebApiClient.DataAnnotations;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 表示属性解析约定
    /// 用于实现DataAnnotations的功能
    /// </summary>
    [DebuggerDisplay("FormatScope = {FormatScope}")]
    public class AnnotationsContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// ContractResolver缓存
        /// </summary>
        private static readonly ConcurrentDictionary<ContractKey, AnnotationsContractResolver> cache = new ConcurrentDictionary<ContractKey, AnnotationsContractResolver>();

        /// <summary>
        /// 返回属性解析约定
        /// </summary>
        /// <param name="scope">序列化范围</param>
        /// <param name="camelCase">是否使用CamelCase</param>
        /// <exception cref="NotImplementedException"></exception>
        /// <returns></returns>
        public static AnnotationsContractResolver GetResolver(FormatScope scope, bool camelCase)
        {
            var key = new ContractKey(camelCase, scope);
            return cache.GetOrAdd(key, k => new AnnotationsContractResolver(k));
        }

        /// <summary>
        /// 是否camel命名
        /// </summary>
        private readonly bool useCamelCase;

        /// <summary>
        /// 获取序列化范围
        /// </summary>
        public FormatScope FormatScope { get; private set; }


        /// <summary>
        /// 属性解析器
        /// </summary>
        /// <param name="contractKey">contractKey</param>
        private AnnotationsContractResolver(ContractKey contractKey)
            : this(contractKey.CamelCase, contractKey.FormatScope)
        {
        }

        /// <summary>
        /// 属性解析器
        /// </summary>
        /// <param name="camelCase">是否camel命名</param>
        /// <param name="scope">序列化范围</param>
        public AnnotationsContractResolver(bool camelCase, FormatScope scope)
        {
            this.useCamelCase = camelCase;
            this.FormatScope = scope;
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
        /// 创建Json属性
        /// </summary>
        /// <param name="member">属性</param>
        /// <param name="memberSerialization"></param>
        /// <returns></returns>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var annotations = Annotations.GetAnnotations(member, this.FormatScope);

            if (string.IsNullOrEmpty(annotations.AliasName) == false)
            {
                property.PropertyName = annotations.AliasName;
            }
            else if (this.useCamelCase == true)
            {
                property.PropertyName = FormatOptions.CamelCase(property.PropertyName);
            }

            if (string.IsNullOrEmpty(annotations.DateTimeFormat) == false)
            {
                property.Converter = new IsoDateTimeConverter { DateTimeFormat = annotations.DateTimeFormat };
            }

            if (annotations.IgnoreWhenNull == true)
            {
                property.NullValueHandling = NullValueHandling.Ignore;
            }

            property.Ignored = annotations.IgnoreSerialized;
            return property;
        }

        /// <summary>
        /// 表示ContractResolver缓存的键
        /// </summary>
        private struct ContractKey : IEquatable<ContractKey>
        {
            public bool CamelCase { get; private set; }

            public FormatScope FormatScope { get; private set; }

            /// <summary>
            /// ContractResolver缓存的键
            /// </summary>
            /// <param name="camelCase"></param>
            /// <param name="formatScope"></param>
            public ContractKey(bool camelCase, FormatScope formatScope)
            {
                this.CamelCase = camelCase;
                this.FormatScope = formatScope;
            }

            /// <summary>
            /// ContractKey的哈希一样时，才调用此方法
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(ContractKey other)
            {
                return true;
            }

            public override int GetHashCode()
            {
                return this.CamelCase.GetHashCode() ^ this.FormatScope.GetHashCode();
            }
        }
    }
}