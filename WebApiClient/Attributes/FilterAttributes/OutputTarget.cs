using System;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示输出的目标
    /// </summary>
    [Flags]
    public enum OutputTarget
    {
        /// <summary>
        /// 日志工厂
        /// </summary>
        LoggerFactory = 0x1,

        /// <summary>
        /// 调试窗口
        /// </summary>
        Debug = 0x2,

        /// <summary>
        /// 控制台
        /// </summary>
        Console = 0x4,

#if !NETSTANDARD1_3
        /// <summary>
        /// 调试器
        /// </summary>
        Debugger = 0x8
#endif
    }
}
