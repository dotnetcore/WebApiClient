using System;
using System.Buffers;
using System.Text.Json;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// 定义json序列化/反序列化的行为
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// 将对象序列化为utf8 json到指定的bufferWriter
        /// </summary>
        /// <param name="bufferWriter">buffer写入器</param>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        void Serialize(IBufferWriter<byte> bufferWriter, object? obj, JsonSerializerOptions? options);

        /// <summary>
        /// 将对象序列化为utf8 json
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        byte[] Serialize(object? obj, JsonSerializerOptions? options);

        /// <summary>
        /// 将json文本反序列化对象
        /// </summary>
        /// <param name="json">utf8 json</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        object? Deserialize(byte[]? json, Type objType, JsonSerializerOptions? options);
    }
}
