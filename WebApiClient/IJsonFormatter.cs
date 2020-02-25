using System;

namespace WebApiClient
{
    /// <summary>
    /// Define json serialization / deserialization behavior
    /// </summary>
    public interface IJsonFormatter
    {
        /// <summary>
        /// Serialize objects to json text
        /// </summary>
        /// <param name="obj">Object</param>
        /// <param name="options">Options</param>
        /// <returns></returns>
        string Serialize(object obj, FormatOptions options);

        /// <summary>
        /// Deserialize json text object
        /// </summary>
        /// <param name="json">json text content</param>
        /// <param name="objType">Object type</param>
        /// <returns></returns>
        object Deserialize(string json, Type objType);
    }
}
