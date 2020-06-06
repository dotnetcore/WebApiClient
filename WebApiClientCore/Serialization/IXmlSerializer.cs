using System;
using System.Xml;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// 定义xml序列化/反序列化的行为
    /// </summary>
    public interface IXmlSerializer
    {
        /// <summary>
        /// 将对象序列化为xml文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">配置选项</param> 
        /// <returns></returns>
        string? Serialize(object? obj, XmlWriterSettings? options);

        /// <summary>
        /// 将xml文本反序列化对象
        /// </summary>
        /// <param name="xml">xml文本内容</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">配置选项</param>
        /// <returns></returns>
        object? Deserialize(string? xml, Type objType, XmlReaderSettings? options);
    }
}
