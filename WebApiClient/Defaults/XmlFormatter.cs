using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 默认的Xml解析工具
    /// </summary>
    public class XmlFormatter : IXmlFormatter
    {
        /// <summary>
        /// 将对象序列化为xml文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="encoding">编码</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public virtual string Serialize(object obj, Encoding encoding)
        {
            if (encoding == null)
            {
                throw new ArgumentNullException(nameof(encoding));
            }

            if (obj == null)
            {
                return null;
            }

            var stringWriter = new EncodingStringWriter(encoding);
            var xmlSerializer = new XmlSerializer(obj.GetType());
            using (var xmlWriter = XmlWriter.Create(stringWriter))
            {
                xmlSerializer.Serialize(xmlWriter, obj);
            }
            return stringWriter.ToString();
        }

        /// <summary>
        /// 反序列化xml为对象
        /// </summary>
        /// <param name="xml">xml</param>
        /// <param name="objType">对象类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <returns></returns>
        public virtual object Deserialize(string xml, Type objType)
        {
            if (objType == null)
            {
                throw new ArgumentNullException(nameof(objType));
            }

            if (string.IsNullOrEmpty(xml))
            {
                return objType.DefaultValue();
            }

            var xmlSerializer = new XmlSerializer(objType);
            using (var reader = new StringReader(xml))
            {
                return xmlSerializer.Deserialize(reader);
            }
        }

        /// <summary>
        /// 表示可指定编码文本写入器
        /// </summary>
        private class EncodingStringWriter : StringWriter
        {
            /// <summary>
            /// 编码
            /// </summary>
            private readonly Encoding encoding;

            /// <summary>
            /// 获取编码
            /// </summary>
            public override Encoding Encoding
            {
                get => this.encoding;
            }

            /// <summary>
            /// 可指定编码文本写入器
            /// </summary>
            /// <param name="encoding">编码</param>
            public EncodingStringWriter(Encoding encoding)
            {
                this.encoding = encoding;
            }
        }
    }
}
