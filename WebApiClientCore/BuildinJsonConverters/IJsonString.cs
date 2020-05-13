using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义JsonString的接口
    /// </summary>
    interface IJsonString
    {
        /// <summary>
        /// 获取值
        /// </summary>
        object Value { get; }

        /// <summary>
        /// 获取值类型
        /// </summary>
        Type ValueType { get; }
    }
}
