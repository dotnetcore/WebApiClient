using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

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
        /// <param name="parameter">对象</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        public string Serialize(ApiParameterDescriptor parameter, Encoding encoding)
        {
            if (parameter.Value == null)
            {
                return null;
            }

            if (JsonNet.IsSupported == true)
            {
                return JsonNet.SerializeObject(parameter.Value);
            }

            var serializer = new DataContractJsonSerializer(parameter.Value.GetType());
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, parameter.Value);
                var bytes = stream.ToArray();
                return encoding.GetString(bytes);
            }
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

            if (JsonNet.IsSupported == true)
            {
                return JsonNet.DeserializeObject(json, objType);
            }

            var bytes = Encoding.UTF8.GetBytes(json);
            var serializer = new DataContractJsonSerializer(objType);
            using (var stream = new MemoryStream(bytes))
            {
                stream.Position = 0;
                return serializer.ReadObject(stream);
            }
        }
    }
}
