using System.Collections.Generic;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// 定义将对象转换为键值对的行为
    /// </summary>
    public interface IKeyValueSerializer
    {
        /// <summary>
        /// 序列化对象为键值对
        /// </summary>
        /// <param name="key">对象名称</param>
        /// <param name="obj">对象实例</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        IList<KeyValue> Serialize(string key, object? obj, KeyValueSerializerOptions? options);
    }
}
