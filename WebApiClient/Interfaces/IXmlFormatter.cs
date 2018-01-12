using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义xml序列化/反序列化的行为
    /// </summary>
    public interface IXmlFormatter
    {
        /// <summary>
        /// 将参数值序列化为xml文本
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        string Serialize(object obj, Encoding encoding);

        /// <summary>
        /// 将接口回复的内容反序列化对象
        /// </summary>
        /// <param name="content">xml文本内容</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        object Deserialize(string content, Type objType);
    }
}
