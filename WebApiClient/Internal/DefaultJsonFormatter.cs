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
                ContractResolver = new DateTimeFormatContractResolver()
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
            return JsonConvert.DeserializeObject(json, objType);
        }
    }

    /// <summary>
    /// 时间格式化解析器
    /// </summary>
    class DateTimeFormatContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private static readonly ConcurrentDictionary<MemberInfo, IsoDateTimeConverter> cache = new ConcurrentDictionary<MemberInfo, IsoDateTimeConverter>();

        /// <summary>
        /// 获取转换器
        /// </summary>
        /// <param name="member">成员</param>
        /// <returns></returns>
        private static IsoDateTimeConverter GetConverter(MemberInfo member)
        {
            var datatimeFormat = member.GetAttribute<DateTimeFormatAttribute>(true);
            if (datatimeFormat == null)
            {
                return null;
            }
            return new IsoDateTimeConverter { DateTimeFormat = datatimeFormat.Format };
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
            if (property.Converter == null)
            {
                property.Converter = cache.GetOrAdd(member, GetConverter);
            }
            return property;
        }
    }
}

