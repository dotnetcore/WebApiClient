using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示将参数名或属性名进行别名
    /// 当修饰属性时，JsonFormatter或KeyValueFormatter序列化将使用此别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class AliasAsAttribute : Attribute
    {
        /// <summary>
        /// 获取别名
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 指定参数或属性的别名
        /// </summary>
        /// <param name="name">参数或属性的别名</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AliasAsAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name) == true)
            {
                throw new ArgumentNullException(nameof(name));
            }
            this.Name = name;
        }
    }
}
