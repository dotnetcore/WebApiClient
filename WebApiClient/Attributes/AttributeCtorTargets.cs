using System;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 特性构造函数的应用目标
    /// </summary>
    [Flags]
    public enum AttributeCtorTargets
    {
        /// <summary>
        /// 应用于接口
        /// </summary>
        Interface = 0x1,

        /// <summary>
        /// 应用于方法
        /// </summary>
        Method = 0x2,

        /// <summary>
        /// 应用于参数
        /// </summary>
        Parameter = 0x4,
    }
}
