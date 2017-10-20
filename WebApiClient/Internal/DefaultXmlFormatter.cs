using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WebApiClient
{
    /// <summary>
    /// 默认的Xml解析工具
    /// </summary>
    class DefaultXmlFormatter : IStringFormatter
    {
        /// <summary>
        /// 将参数值序列化为xml文本
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

            var xmlSerializer = new XmlSerializer(parameter.ParameterType);
            using (var stream = new MemoryStream())
            {
                var xmlWriter = new XmlTextWriter(stream, encoding);
                xmlSerializer.Serialize(xmlWriter, parameter.Value);
                return encoding.GetString(stream.ToArray());
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
