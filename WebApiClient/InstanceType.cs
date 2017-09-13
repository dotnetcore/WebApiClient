using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 实例获取方式
    /// </summary>
    public enum InstanceType
    {
        /// <summary>
        /// 单一实例
        /// </summary>
        SingleInstance,

        /// <summary>
        /// 每请求一个实例
        /// </summary>
        InstancePerRequest
    }
}
