using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WebApiClient
{
    /// <summary>
    /// 默认的Xml解析工具
    /// </summary>
    class DefaultXmlFormatter : IStringFormatter
    {
        /// <summary>
        /// 序列化为xml
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public string Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var xmlSerializer = new XmlSerializer(obj.GetType());
            using (var stream = new MemoryStream())
            {
                xmlSerializer.Serialize(stream, obj);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="xml">xml</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        public object Deserialize(string xml, Type objType)
        {
            var xmlSerializer = new XmlSerializer(objType);
            using (var reader = new StringReader(xml))
            {
                return xmlSerializer.Deserialize(reader);
            }
        }
    }
}
