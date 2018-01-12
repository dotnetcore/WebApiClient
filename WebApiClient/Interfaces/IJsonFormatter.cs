using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义json序列化/反序列化的行为
    /// </summary>
    public interface IJsonFormatter
    {
        /// <summary>
        /// 将参数值序列化为json文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        string Serialize(object obj, FormatOptions options);

        /// <summary>
        /// 将接口回复的内容反序列化对象
        /// </summary>
        /// <param name="content">json文本内容</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        object Deserialize(string content, Type objType);
    }
}
