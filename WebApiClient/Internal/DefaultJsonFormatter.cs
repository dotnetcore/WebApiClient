using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


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
                datetimeFormate = DateTimeFormats.ISO8601WithMillisecond;
            }
            var setting = new JsonSerializerSettings { DateFormatString = datetimeFormate };
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
}
