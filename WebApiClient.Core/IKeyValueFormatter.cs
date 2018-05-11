using System.Collections.Generic;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 定义将对象转换为键值对的行为
    /// </summary>
    public interface IKeyValueFormatter
    {
        /// <summary>
        /// 序列化对象为键值对
        /// </summary>
        /// <param name="name">对象名称</param>
        /// <param name="obj">对象实例</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> Serialize(string name, object obj, FormatOptions options);

        /// <summary>
        /// 序列化参数为键值对
        /// </summary>
        /// <param name="parameter">参数</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        IEnumerable<KeyValuePair<string, string>> Serialize(ApiParameterDescriptor parameter, FormatOptions options);
    }
}
