using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示将参数名或属性名进行别名
    /// 当修饰属性时，Json或KeyValueFormatter序列化将使用此别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class AliasAsAttribute : Attribute
    {
        /// <summary>
        /// 参数的别名
        /// </summary>
        public string Alias { get; private set; }

        /// <summary>
        /// 指定参数的别名
        /// </summary>
        /// <param name="alias">参数的别名</param>
        public AliasAsAttribute(string alias)
        {
            if (string.IsNullOrWhiteSpace(alias) == true)
            {
                throw new ArgumentNullException("alias");
            }
            this.Alias = alias;
        }
    }
}
