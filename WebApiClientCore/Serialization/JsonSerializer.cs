using System;
using System.Text.Json;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// 默认的json序列化工具
    /// </summary>
    public class JsonSerializer : IJsonSerializer
    {
        /// <summary>
        /// 将对象列化为json
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public virtual byte[] Serialize(object? obj, JsonSerializerOptions? options)
        {
            return obj == null ?
                Array.Empty<byte>() :
                System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj, obj.GetType(), options);
        }

        /// <summary>
        /// 反序列化json为对象
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public virtual object? Deserialize(byte[]? json, Type objType, JsonSerializerOptions? options)
        {
            return json == null || json.Length == 0 ?
                objType.DefaultValue() :
                System.Text.Json.JsonSerializer.Deserialize(json, objType, options);
        }
    }
}