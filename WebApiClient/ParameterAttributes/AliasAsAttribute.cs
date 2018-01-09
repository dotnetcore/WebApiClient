using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示指定参数的别名
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
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
            if (string.IsNullOrEmpty(alias) == true)
            {
                throw new ArgumentNullException();
            }
            this.Alias = alias;
        }
    }
}
