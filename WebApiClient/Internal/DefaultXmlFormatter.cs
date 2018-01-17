using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using WebApiClient.Interfaces;

namespace WebApiClient
{
    /// <summary>
    /// 默认的Xml解析工具
    /// </summary>
    class DefaultXmlFormatter : IXmlFormatter
    {
        /// <summary>
        /// 将对象序列化为xml文本
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

            var xmlSerializer = new XmlSerializer(obj.GetType());
            using (var stream = new MemoryStream())
            {
                var xmlWriter = new XmlTextWriter(stream, encoding);
                xmlSerializer.Serialize(xmlWriter, obj);
                return encoding.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 反序列化xml为对象
        /// </summary>
        /// <param name="xml">xml</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        public object Deserialize(string xml, Type objType)
        {
            if (string.IsNullOrEmpty(xml))
            {
                return null;
            }

            var xmlSerializer = new XmlSerializer(objType);
            using (var reader = new StringReader(xml))
            {
                return xmlSerializer.Deserialize(reader);
            }
        }
    }
}
