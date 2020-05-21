namespace WebApiClientCore
{
    /// <summary>
    /// string扩展
    /// </summary>
    static class StringExtensions
    {
        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string Format(this string str, params object?[] args)
        { 
            return string.Format(str, args);
        }
    }
}
