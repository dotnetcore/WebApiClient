using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// DataAnnotation
    /// 表示当JsonFormatter或KeyValueFormatter序列化对象时，此属性将忽略
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreSerializedAttribute : Attribute
    {
    }
}
