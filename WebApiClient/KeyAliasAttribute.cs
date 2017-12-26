using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示当KeyValueFormatter序列化对象时，指定此属性对应的键的别名
    /// 不可继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class KeyAliasAttribute : Attribute
    {
        /// <summary>
        /// 获取属性对应的键的别名
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// 指定此属性对应的键的别名
        /// </summary>
        /// <param name="alias">对应的键的别名</param>
        /// <exception cref="ArgumentNullException"></exception>
        public KeyAliasAttribute(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                throw new ArgumentNullException();
            }
            this.Alias = alias;
        }
    }
}
