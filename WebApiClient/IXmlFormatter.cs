using System;
using System.Text;

namespace WebApiClient
{
    /// <summary>
    /// Define xml serialization / deserialization behavior
    /// </summary>
    public interface IXmlFormatter
    {
        /// <summary>
        /// Serialize objects as xml text
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="encoding">encoding</param>
        /// <returns></returns>
        string Serialize(object obj, Encoding encoding);

        /// <summary>
        /// Deserialize xml text object
        /// </summary>
        /// <param name="xml">xml text content</param>
        /// <param name="objType">Object type</param>
        /// <returns></returns>
        object Deserialize(string xml, Type objType);
    }
}
