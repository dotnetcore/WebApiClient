using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示当KeyValueFormatter序列化对象时，此属性将忽略
    /// 不可继承
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class KeyValueIgnoreAttribute : Attribute
    {
    }
}
