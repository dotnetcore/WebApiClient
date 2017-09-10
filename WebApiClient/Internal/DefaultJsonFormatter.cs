using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 默认的json解析工具
    /// </summary>
    class DefaultJsonFormatter : IStringFormatter
    {
        /// <summary>
        /// 序列化为json
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public string Serialize(object obj)
        {
            if (obj == null)
            {
                return null;
            }
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="objType">对象类型</param>
        /// <returns></returns>
        public object Deserialize(string json, Type objType)
        {
            var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
            return serializer.Deserialize(json, objType);
        }
    }
}
