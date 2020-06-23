using System;
using System.Buffers;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// 定义json序列化/反序列化的行为
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// 将对象序列化为utf8编码的Json到指定的bufferWriter
        /// </summary>
        /// <param name="bufferWriter">buffer写入器</param>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        void Serialize(IBufferWriter<byte> bufferWriter, object? obj, JsonSerializerOptions? options);

        /// <summary>
        /// 将utf8编码的Json反序列化对象
        /// </summary>
        /// <param name="utf8Json">utf8编码的Json</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        object? Deserialize(ReadOnlySpan<byte> utf8Json, Type objType, JsonSerializerOptions? options);

        /// <summary>
        /// 将utf8编码的Json流反序列化对象
        /// </summary>
        /// <param name="utf8Json">utf8编码的Json流</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        Task<object> DeserializeAsync(Stream utf8Json, Type objType, JsonSerializerOptions? options);
    }
}
