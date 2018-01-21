using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Defaults.KeyValueFormats
{
    /// <summary>
    /// 表示转换结果
    /// </summary>
    public class ConvertResult
    {
        /// <summary>
        /// 获取数据类型
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// 获取数据名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取数据的值
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// 转换结果
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="name">数据名称</param>
        /// <param name="value">数据的值</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ConvertResult(Type type, string name, string value)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Type = type;
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// 返回是否为可空类型且值为空
        /// </summary>
        /// <returns></returns>
        public bool IsNullableNullvalue()
        {
            return this.Type.IsValueType && this.Value == null;
        }
    }
}
