using System;

namespace WebApiClient
{
    /// <summary>
    /// 定义json序列化/反序列化的行为
    /// </summary>
    public interface IJsonFormatter
    {
        /// <summary>
        /// 将对象序列化为json文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        string Serialize(object obj, FormatOptions options);

        /// <summary>
        /// 将json文本反序列化对象
        /// </summary>
        /// <param name="json">json文本内容</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        object Deserialize(string json, Type objType);
    }
}
