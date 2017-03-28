using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    /// <summary>
    /// 定义json解析工具的接口
    /// </summary>
    public interface IJsonFormatter
    {
        /// <summary>
        /// 序列化为json
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        string Serialize(object obj);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        object Deserialize(string json, Type objType);
    }
}
