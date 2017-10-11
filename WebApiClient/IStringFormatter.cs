using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义json/xml序列化/反序列化的行为
    /// </summary>
    public interface IStringFormatter
    {
        /// <summary>
        /// 将参数值序列化为json/xml文本
        /// </summary>
        /// <param name="parameter">对象</param>
        /// <param name="encoding">编码</param>
        /// <returns></returns>
        string Serialize(ApiParameterDescriptor parameter, Encoding encoding);

        /// <summary>
        /// 将接口回复的内容反序列化对象
        /// </summary>
        /// <param name="content">json/xml文本内容</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        object Deserialize(string content, Type objType);
    }
}
