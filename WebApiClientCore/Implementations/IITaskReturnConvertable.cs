namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 定义可以转换为ITask返回声明的Action执行器
    /// </summary>
    interface IITaskReturnConvertable
    {
        /// <summary>
        /// 转换为ITask返回声明的Action执行器
        /// </summary>
        /// <returns></returns>
        ApiActionInvoker ToITaskReturnActionInvoker();
    }
}
