using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if NETCOREAPP2_0
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
#endif

namespace WebApiClient
{
    /// <summary>
    /// 默认的json解析工具
    /// </summary>
    class DefaultJsonFormatter : IStringFormatter
    {
        /// <summary>
        /// 将参数值序列化为json文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public string Serialize(object obj, Encoding encoding)
        {
            if (obj == null)
            {
                return null;
            }

            var dateTimeFormate = "yyyy-MM-dd HH:mm:ss";
#if NET45
            if (JsonNet.IsSupported == true)
            {
                return JsonNet.SerializeObject(obj);
            }
            return JSON.Serialize(obj, dateTimeFormate);
#endif
#if NETCOREAPP2_0
            
            var setting = new JsonSerializerSettings { DateFormatString = dateTimeFormate };
            return JsonConvert.SerializeObject(obj, setting);
#endif
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

#if NET45
            if (JsonNet.IsSupported == true)
            {
                return JsonNet.DeserializeObject(json, objType);
            }
            return JSON.Deserialize(json, objType);
#endif

#if NETCOREAPP2_0
            return JsonConvert.DeserializeObject(json, objType);
#endif
        }
    }
}
