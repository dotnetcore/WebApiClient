using System;
using System.IO;
using System.Text;
using System.Xml;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// 默认的Xml序列化工具
    /// </summary>
    public class XmlSerializer : IXmlSerializer
    {
        private static readonly XmlReaderSettings readerSettings = new XmlReaderSettings();
        private static readonly XmlWriterSettings writerSettings = new XmlWriterSettings();

        /// <summary>
        /// 将对象序列化为xml文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">配置选项</param> 
        /// <returns></returns>
        public string? Serialize(object? obj, XmlWriterSettings? options)
        {
            if (obj == null)
            {
                return null;
            }

            var settings = options ?? writerSettings;
            var writer = new EncodingWriter(settings.Encoding);
            using var xmlWriter = XmlWriter.Create(writer, settings);
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(obj.GetType());
            xmlSerializer.Serialize(xmlWriter, obj);
            return writer.ToString();
        }

        /// <summary>
        /// 将xml文本反序列化对象
        /// </summary>
        /// <param name="xml">xml文本内容</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">配置选项</param>
        /// <returns></returns>
        public object? Deserialize(string? xml, Type objType, XmlReaderSettings? options)
        {
            if (objType == null)
            {
                throw new ArgumentNullException(nameof(objType));
            }

            if (string.IsNullOrEmpty(xml))
            {
                return objType.DefaultValue();
            }

            var settings = options ?? readerSettings;
            using var reader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(reader, settings);
            var xmlSerializer = new System.Xml.Serialization.XmlSerializer(objType);
            return xmlSerializer.Deserialize(xmlReader);
        }

        /// <summary>
        /// 表示可指定编码文本写入器
        /// </summary>
        private class EncodingWriter : StringWriter
        {
            /// <summary>
            /// 编码
            /// </summary>
            private readonly Encoding encoding;

            /// <summary>
            /// 获取编码
            /// </summary>
            public override Encoding Encoding => this.encoding;

            /// <summary>
            /// 可指定编码文本写入器
            /// </summary>
            /// <param name="encoding">编码</param>
            public EncodingWriter(Encoding encoding)
            {
                this.encoding = encoding;
            }
        }
    }
}
