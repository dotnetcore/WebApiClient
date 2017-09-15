using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{

    /// <summary>
    /// 定义特定是否允许在接口与方法上重复声明
    /// </summary>
    public interface IAttributeAllowMultiple
    {
        /// <summary>
        /// 获取同个类型是否允许重复
        /// </summary>
        bool AllowMultiple { get; }
    }
}
