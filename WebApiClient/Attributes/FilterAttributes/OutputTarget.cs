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
        /// 需要配置HttpApiConfig的ServiceProvider或LoggerFactory
        /// </summary>
        LoggerFactory = 0x1,

        /// <summary>
        /// 控制台窗口
        /// </summary>
        Console = 0x2,

#if !NETSTANDARD1_3
        /// <summary>
        /// Visual Studio输出窗口的调试源
        /// </summary>
        Debug = 0x4,

        /// <summary>
        /// 调试器
        /// 如果你的程序在Visual Studio下调试运行，效果等同于OutputTarget.Debug
        /// 如果你的程序在没有任何调试环境下时运行，可以使用DebugView工具捕捉输出的消息
        /// 如果消息内容长度大于4K，则自动被截断输出
        /// </summary>
        Debugger = 0x8,
#endif
    }
}
